using FMOD.Studio;
using Rewired;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb; // player rigid body
    private Animator animator;
    private enemyBase enemy;
    private SpriteRenderer sr;

    public SpriteRenderer SR
    {
        get { return sr; }
        set { sr = value; }
    }

    [Header("Debug Settings")]
    [SerializeField] private bool DebugMode;
    private bool debugran = false;

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed;
    private float xAxis;
    private float yAxis;
    [Space(5)]

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    private float jumpBufferCounter;
    [SerializeField] private float jumpBuffer;
    private float coyoteTimeCounter;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter;
    [SerializeField] private int airJumps;
    [SerializeField] private float maxVelocity;
    private float fallMultiplier = 0.9f;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckX;
    [SerializeField] private float groundCheckY;
    [SerializeField] private LayerMask groundLayer;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    public bool canDash = false;
    private bool dashed;
    private float gravity;
    [Space(5)]

    [Header("Attack Settings")]
    [SerializeField] private float timeBetweenNeutralAttacks;
    [SerializeField] private float timeBetweenLightAttacks;
    [SerializeField] private float timeBetweenDarkAttacks;
    [SerializeField] private float damage;
    [SerializeField] private Transform sideAttackTransform;
    [SerializeField] private Transform airAttackTransform;
    [SerializeField] private Vector2 sideAttackArea;
    [SerializeField] private Vector2 airAttackArea;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private GameObject slashEffect;
    private bool attack = false;
    private bool castSpell;
    private float timeSinceAttack;

    [Header("Recoil Settings")]
    [SerializeField] private int recoilStepsX = 5;
    [SerializeField] private int recoilStepsY = 5;
    [SerializeField] private float recoilSpeedX = 100;
    [SerializeField] private float recoilSpeedY = 100;
    private int stepsRecoiledX;
    private int stepsRecoiledY;

    [Space(5)]
    [Header("Health Settings")]
    public int health;
    [SerializeField] public int maxHealth;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;

    [Space(5)]
    [Header("Audio Settings")]
    [SerializeField]
    private EventInstance playerFootsteps;

    [Space(5)]
    [Header("Dark/Light Attack Settings")]
    [SerializeField] public float maxEnergy;
    public float currentEnergy;
    [SerializeField] float recoveryTimer;
    [SerializeField] float recoveryRate;
    float timeSinceSpellUsed = 0f;
    bool spellUsedRecently;

    int fireballCost;
    [SerializeField] float timeToCast;
    [SerializeField] private Transform fireballTransform;
    [SerializeField] GameObject lightFireball;
    [SerializeField] GameObject darkFireball;
    float timeSinceCast;
    public bool lightUnlocked = false;
    public bool darkUnlocked = false;
    public delegate void OnEnergyChangedDelegate();
    [HideInInspector] public OnEnergyChangedDelegate onEnergyChangedCallback;

    [Space(5)]
    [Header("Dark/Light Environment Settings")]
    [SerializeField] float hidingEnergyLossRate;
    [SerializeField] float bubbleEnergyLossRate;
    [SerializeField] SpriteMask lightModeBubble;
    bool bubbleUp = false;
    public bool BubbleUp
    {
        get { return bubbleUp; }
    }

    [Space(5)]
    [Header("Other Objects")]
    [SerializeField] SpriteRenderer fade;
    float alpha = 1.0f;
    [SerializeField] float timeBetweenGlances;
    [SerializeField] public GameObject playerArrowIndicator;
    float countUptoGlance = 0;
    float releaseStaleInputs = 0.1f;
    float releaseStateInputsIncrement = 0.0f;
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }

            }
        }
    }

    public float CurrentEnergy
    {
        get { return currentEnergy; }
        set
        {
            if (currentEnergy != value)
            {
                currentEnergy = Mathf.Clamp(value, 0, maxEnergy);

                if(onEnergyChangedCallback != null)
                {
                    onEnergyChangedCallback.Invoke();
                }
            }
        }
    }

    [Space(5)]
    public PlayerStateList pState;

    private Player player; // The Rewired Player
    public int playerId = 0; // The Rewired player id of this character
                             // Single player game so this is always 0, but good practice not to hardcode it

    public static PlayerController Instance;

    public GhostEffect ghost;


    private bool dashPressed;
    private bool jumpPressed;
    private bool doubleJumpPressed;
    private bool switchAttackTypeLeftPressed;
    private bool switchAttackTypeRightPressed;
    private bool interactPressed;
    private bool idleGlancePlaying = false;
    private bool spellSwapPressed;

    private bool stopFading = false;

    public enum AttackType
    {
        Neutral,
        Light,
        Dark
    }

    public AttackType currentAttackType = AttackType.Neutral;

    public enum EquippedLightSpell
    {
        Fireball,
        LightBubble
    }

    public EquippedLightSpell currentLightSpell = EquippedLightSpell.Fireball;

    public enum EquippedDarkSpell
    {
        Fireball
    }

    public EquippedDarkSpell currentDarkSpell = EquippedDarkSpell.Fireball;

    private void Awake()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        Health = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();

        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        gravity = rb.gravityScale;

        sr = GetComponent<SpriteRenderer>();

        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);

        fireballCost = (int)maxEnergy;
    }

    private void Update()
    {
        if (DebugMode == true && debugran == false)
        {
            debugran = true;
            DebugModeUnlocks();
        }
        if (alpha >= 0.0f && !stopFading)
        {
            fade.sortingOrder = 10;
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alpha);
            alpha -= 0.05f;

            if (alpha < 0.0f) { stopFading = true; }
        }

        if (health > 0)
        {
            GetInput();
            UpdateJumpVariables();
            ClearStaleInputs();
        }
        else
        {
            StartCoroutine(WaitTillEnd());
        }

    }

    private IEnumerator WaitTillEnd()
    {
        animator.SetBool("isDead", true);
        pState.invincible = true;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        sr.sortingOrder = 10; // render above everything
        while (alpha < 1.1f)
        {
            fade.sortingOrder = 6; // render above everything except the player
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alpha);
            alpha += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("GameOverScreen");
    }

    void FixedUpdate()
    {
        if (health <= 0) { return; }

        countUptoGlance += Time.deltaTime;

        if (pState.hiding) { Hiding(); return; }

        if(pState.shadowWalking) { ShadowBlockMovement(); return; }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle") && !idleGlancePlaying && countUptoGlance >= timeBetweenGlances)
        {
            StartCoroutine(WaitForIdleGlance());
            countUptoGlance = 0f;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            countUptoGlance = 0f;
        }

        if (pState.dashing)
        {
            ghost.makeGhost = true;
            animator.SetBool("isDashing", true);
            return; // if the player is dashing, don't get more movements
        }
        else
        {
            ghost.makeGhost = false;
            animator.SetBool("isDashing", false);
        }

        if (bubbleUp)
        {
            if (CurrentEnergy > 0)
            {
                CurrentEnergy -= bubbleEnergyLossRate * Time.deltaTime;
            }
            else if(CurrentEnergy == 0)
            {
                bubbleUp = false;
                lightModeBubble.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                spellUsedRecently = true;
            }
        }

        if(spellUsedRecently)
        {
            timeSinceSpellUsed = timeSinceSpellUsed + Time.deltaTime;
            if(timeSinceSpellUsed >= recoveryTimer)
            {
                timeSinceSpellUsed = 0f;
                spellUsedRecently = false;
                pState.recovering = true;
            }
        }

        if(pState.recovering)
        {
            currentEnergy += recoveryRate;
            if(currentEnergy >= maxEnergy)
            {
                pState.recovering = false;
            }
        }

        if (pState.casting)
        {
            return;
        }
        
        Flip();
        Move();
        Jump();
        SwitchAttackTypes();
        SwapSpell();
        Attack();
        CastSpell();
        Recoil();
        StartDash();
        UpdateSound();
        // animation update
        animator.SetBool("isGrounded", Grounded());
        animator.SetFloat("yVel", rb.velocity.y);
        animator.SetFloat("xVel", rb.velocity.x);
        if (rb.velocity.x > 0.1 || rb.velocity.x < -0.1)
        {
            animator.SetBool("isWalking", true);

        }
        else
        {
            animator.SetBool("isWalking", false);
            UpdateSound();
        }

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);

    }

    void GetInput()
    {
        if (PauseMenu.GamePaused == true) { return; }

        xAxis = player.GetAxis("Move Horizontal");
        yAxis = player.GetAxis("Move Vertical");

        if (!attack)
        {
            attack = player.GetButtonDown("Attack");
        }

        if (!castSpell)
        {
            castSpell = player.GetButtonDown("Cast Spell");
        }

        if (!dashPressed && canDash)
        {
            dashPressed = player.GetButtonDown("Dash");
        }

        if (!jumpPressed)
        {
            jumpPressed = player.GetButtonDown("Jump");
        }

        if (!switchAttackTypeLeftPressed)
        {
            switchAttackTypeLeftPressed = player.GetButtonDown("Switch Attack Type L");
        }

        if (!switchAttackTypeRightPressed)
        {
            switchAttackTypeRightPressed = player.GetButtonDown("Switch Attack Type R");
        }

        if (!interactPressed)
        {
            interactPressed = player.GetButtonDown("Interact");
        }

        if (jumpPressed)
        {
            if (Grounded() || !doubleJumpPressed)
            {
                animator.SetTrigger("jumpSquat");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                doubleJumpPressed = !Grounded();
            }
        }
        if (Grounded())
        {
            doubleJumpPressed = false;
        }

        if (!spellSwapPressed)
        {
            spellSwapPressed = player.GetButtonDown("SpellSwap");
        }


        if (Input.GetKey(KeyCode.LeftArrow) && (Input.GetKey(KeyCode.RightArrow))
        || Input.GetKey(KeyCode.A) && (Input.GetKey(KeyCode.D)))
        {
            xAxis = 0;
        }
    }

    void ClearStaleInputs()
    {
        if (interactPressed)
        {
            releaseStateInputsIncrement += Time.deltaTime;

            if (releaseStateInputsIncrement >= releaseStaleInputs)
            {
                interactPressed = false;
                releaseStateInputsIncrement = 0.0f;
            }
        }
        else
        {
            releaseStateInputsIncrement = 0.0f;
        }

    }

    void Flip() // this will be useful for animation stuff later
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-(Mathf.Abs(transform.localScale.x)), transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2((Mathf.Abs(transform.localScale.x)), transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        //trigger dash animation
        rb.gravityScale = 0; // this allows the dash to keep the player from falling if they do so in the air
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        //Physics2D.IgnoreLayerCollision(0, 6, true); this line is what activates passing through enemies
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        //Physics2D.IgnoreLayerCollision(0, 6, false); this line is what deactivates passing through enemies
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        dashPressed = false;
    }

    void StartDash()
    {
        if (dashPressed && canDash && !dashed)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.dash, this.transform.position);
            StartCoroutine(Dash());
            dashed = true;
        }
        if (Grounded())
        {
            dashed = false;
        }
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;

        switch (currentAttackType)
        {
            case (AttackType.Neutral):
                if (attack && timeSinceAttack >= timeBetweenNeutralAttacks)
                {
                    PrepareToHit();
                    attack = false;
                }
                break;

            case (AttackType.Dark):
                if (attack && timeSinceAttack >= timeBetweenDarkAttacks)
                {
                    PrepareToHit();
                    attack = false;
                }
                break;

            case (AttackType.Light):
                if (attack && timeSinceAttack >= timeBetweenLightAttacks)
                {
                    PrepareToHit();
                    attack = false;
                }
                break;
        }
    }

    void PrepareToHit()
    {
        timeSinceAttack = 0;
        animator.SetTrigger("attack");
        if (currentAttackType == AttackType.Neutral)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.normalSlash, this.transform.position);
        }
        if (currentAttackType == AttackType.Dark)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.darkSlash, this.transform.position);
        }
        if (currentAttackType == AttackType.Light)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.lightSlash, this.transform.position);
        }


        if (Grounded())
        {
            Hit(sideAttackTransform, sideAttackArea, ref pState.recoilingX, recoilSpeedX);
            Instantiate(slashEffect, sideAttackTransform);
        }
        else if (!Grounded())
        {
            Hit(airAttackTransform, airAttackArea, ref pState.recoilingY, recoilSpeedY);
            SlashEffectAtAngle(slashEffect, 0, airAttackTransform);
        }
    }   

    bool EnemyDirection(enemyBase _enemy)
    {
        if(transform.localScale.x > 0 && _enemy.transform.localScale.x > 0)
        {
            return true;
        }
        else if(transform.localScale.x < 0 && _enemy.transform.localScale.x < 0)
        {
            return true;
        }
        else if(_enemy.transform.localScale.x == 0)
        {
            return true;
        }

        return false;
    }

    float GetAttackDamage()
    {
        switch (currentAttackType)
        {
            case (AttackType.Neutral):
                return damage;
            case (AttackType.Light):
                return damage + (-damage / 2.0f);
            case (AttackType.Dark):
                return damage * 2.0f;
        }

        return damage;
    }

    float GetRecoil(float recoilSpeed)
    {
        switch (currentAttackType)
        {
            case (AttackType.Neutral):
                return recoilSpeed;
            case (AttackType.Light):
                return recoilSpeed + (-recoilSpeed / 2);
            case (AttackType.Dark):
                return recoilSpeed * 2;
        }

        return recoilSpeed;
    }
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float recoilSpeed)
    {
        // The way this function works is it grabs every object within the area and then sorts out what is and isnt hittable.

        Collider2D[] ObjectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (ObjectsToHit.Length > 0)
        {
            _recoilDir = true;
        }

        for (int i = 0; i < ObjectsToHit.Length; i++)
        {
            if (ObjectsToHit[i].GetComponent<enemyBase>() != null && ObjectsToHit[i].isTrigger)
            {
                enemy = ObjectsToHit[i].GetComponent<enemyBase>();

                if(EnemyDirection(enemy))
                {
                    if (enemy.GetComponent<FootSolider>() != null)
                    {
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.soldierHurt, this.transform.position);
                    }
                    if (enemy.GetComponent<GroundSentry>() != null)
                    {
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.sentryHurt, this.transform.position);
                    }

                    float _damage = GetAttackDamage();
                    float _recoilStrength = GetRecoil(recoilSpeed);

                    if (currentAttackType == AttackType.Light) { enemy.EnemyHit(_damage, (transform.position - ObjectsToHit[i].transform.position).normalized, _recoilStrength, false); }
                    else { enemy.EnemyHit(_damage, (transform.position - ObjectsToHit[i].transform.position).normalized, _recoilStrength, true); }
                }
                
            }
        }

    }

    void CastSpell()
    {
        if (castSpell && timeSinceCast >= timeToCast)
        {
            switch (currentAttackType)
            {
                case AttackType.Dark:
                    CastDarkSpell();
                    break;

                case AttackType.Light:
                    CastLightSpell();
                    break;
                case AttackType.Neutral:
                    timeSinceCast += Time.deltaTime;
                    castSpell = false;
                    break;
            }
        }
        else
        {
            timeSinceCast += Time.deltaTime;
            castSpell = false;
        }
    }

    void SwapSpell()
    {
        if (spellSwapPressed)
        {
            switch (currentAttackType)
            {
                case AttackType.Dark:
                    currentDarkSpell = EquippedDarkSpell.Fireball; // add others in later
                    Debug.Log($"Current dark spell - {currentDarkSpell}");
                    spellSwapPressed = false;
                    break;
                case AttackType.Light:
                    if (currentLightSpell == EquippedLightSpell.Fireball) 
                    { 
                        currentLightSpell = EquippedLightSpell.LightBubble; 
                        Debug.Log($"Current light spell - {currentLightSpell}");
                        spellSwapPressed = false;
                    }
                    else 
                    { 
                        currentLightSpell = EquippedLightSpell.Fireball;
                        bubbleUp = false;
                        lightModeBubble.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                        Debug.Log($"Current light spell - {currentLightSpell}");
                        spellSwapPressed = false;
                    }
                    break;
            }
        }
    }

    void CastDarkSpell()
    {
        switch (currentDarkSpell)
        {
            case EquippedDarkSpell.Fireball:
                FireballTime();
                break;
        }

    }

    void CastLightSpell()
    {
        switch(currentLightSpell)
        {
            case EquippedLightSpell.Fireball:
                FireballTime();
                break;
            case EquippedLightSpell.LightBubble:
                LightBubbleTime();
                break;
        }
    }

    void FireballTime()
    {
        if (CurrentEnergy >= fireballCost)
        {
            CurrentEnergy -= fireballCost;
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastFireball());
            if (currentAttackType == AttackType.Dark)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.darkShot, this.transform.position);
            }
            else
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.lightShot, this.transform.position);
            }
            spellUsedRecently = true;

        }
        else
        {
            castSpell = false;
        }
    }

    void LightBubbleTime()
    {
        if(CurrentEnergy > 0 && !bubbleUp && castSpell)
        {
            bubbleUp = true;
            timeSinceCast = 0;
            animator.SetTrigger("lightFireballShoot");
            castSpell = false;
            lightModeBubble.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
        }
        else if ((bubbleUp && castSpell))
        {
            bubbleUp = false;
            lightModeBubble.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
            castSpell = false;
            spellUsedRecently = true;
        }
    }

    IEnumerator CastFireball()
    {
        if (yAxis == 0 || yAxis < 0 && Grounded())
        {
            GameObject fireball = null;
            pState.casting = true;

            if (currentAttackType == AttackType.Light)
            {
                rb.velocity = new Vector2(0.0f, rb.velocity.y);
                animator.SetTrigger("lightFireballShoot");
                yield return new WaitForSeconds(timeToCast);
                fireball = Instantiate(lightFireball, fireballTransform.position, Quaternion.identity);
            }

            else if (currentAttackType == AttackType.Dark)
            {
                rb.velocity = new Vector2(0.0f, rb.velocity.y);
                animator.SetTrigger("darkFireballShoot");
                yield return new WaitForSeconds(timeToCast);
                fireball = Instantiate(darkFireball, fireballTransform.position, Quaternion.identity);
            }

            fireball.GetComponent<Fireball>().SetDirection(pState.lookingRight);

            pState.casting = false;
            castSpell = false;
        }
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilSpeedX, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilSpeedX, 0);
            }
        }
        if (pState.recoilingY)
        {
            if (!Grounded())
            {
                rb.gravityScale = 0;
                rb.velocity = new Vector2(rb.velocity.x, recoilSpeedY);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilSpeedY);
            }
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (pState.recoilingX && stepsRecoiledX < recoilStepsX)
        {
            stepsRecoiledX++;

        }
        else
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepsRecoiledY < recoilStepsY)
        {
            stepsRecoiledY++;

        }
        else
        {
            StopRecoilY();
        }
        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsRecoiledX = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepsRecoiledY = 0;
        pState.recoilingY = false;
    }

    public bool Grounded()
    {
        /*
         * variables in order of appearance
         * 1) origin of the raycast
         * 2) the direction the raycast is going
         * 3) how far the ray will travel
         * 4) the layer that the raycast is trying to detect
         * 
         * the other two checks add and subtract the groundCheckX variable to make sure that this works when the player is on the edge of a platform as well.
         */

        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer))
        {
            //Debug.Log("Grounded");
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (jumpPressed && rb.velocity.y > 0)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerJump, this.transform.position);
            //rb.velocity = new Vector3(rb.velocity.x, 0);

            pState.jumping = false;

            jumpPressed = false;
        }

        if (!pState.jumping)
        {
            // player pressed jump button less than JumpBufferCounter frames ago and is on the ground
            // (coyote time is the time after the player has left the ground that they can still jump)
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                pState.jumping = true;
            }
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * fallMultiplier, 0);
        }
    }


    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter = Mathf.Max(0, coyoteTimeCounter - Time.deltaTime);
        }

        // buffer for the jump button
        if (player.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - Time.deltaTime);
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    public void TakeDamage(float _damage)
    {
        CamShake.Instance.Shake();
        if (!DebugMode)
        {
            Health -= Mathf.RoundToInt(_damage);
            StartCoroutine(StopTakingDamage());
        }
        else if (Health! <= 0)
        {
            Health -= Mathf.RoundToInt(_damage);
            StartCoroutine(StopTakingDamage());
        }
    }
    void SwitchAttackTypes()
    {
        //determine if swapping "left" or "right" (basically are we going forward or back on the wheel)
        if (lightUnlocked && darkUnlocked)
        {
            if (switchAttackTypeLeftPressed)
            {
                switchAttackTypeLeftPressed = false;

                if (currentAttackType == AttackType.Neutral)
                {
                    currentAttackType = AttackType.Dark;
                }
                else
                {
                    currentAttackType--;
                    DarkRoomBubble();
                }
                AudioManager.instance.PlayOneShot(FMODEvents.instance.powerSelect, this.transform.position);
            }

            if (switchAttackTypeRightPressed)
            {
                switchAttackTypeRightPressed = false;

                if (currentAttackType == AttackType.Dark)
                {
                    currentAttackType = AttackType.Neutral;
                }
                else
                {
                    currentAttackType++;
                    DarkRoomBubble();
                }
                AudioManager.instance.PlayOneShot(FMODEvents.instance.powerSelect, this.transform.position);
            }
        }
        else if (darkUnlocked && !lightUnlocked)
        {
            SwitchAttackTypesDarkOnly();
        }
        else
        {
            currentAttackType = AttackType.Neutral;
            switchAttackTypeLeftPressed = false;
            switchAttackTypeRightPressed = false;
            DarkRoomBubble();
            return;
        }
    }

    void SwitchAttackTypesDarkOnly()
    {
        if (switchAttackTypeLeftPressed)
        {
            switchAttackTypeLeftPressed = false;

            if (currentAttackType == AttackType.Neutral)
            {
                currentAttackType = AttackType.Dark;
            }
            else
            {
                currentAttackType = AttackType.Neutral;
            }
            AudioManager.instance.PlayOneShot(FMODEvents.instance.powerSelect, this.transform.position);
        }

        if (switchAttackTypeRightPressed)
        {
            switchAttackTypeRightPressed = false;

            if (currentAttackType == AttackType.Dark)
            {
                currentAttackType = AttackType.Neutral;
            }
            else
            {
                currentAttackType = AttackType.Dark;
            }
            AudioManager.instance.PlayOneShot(FMODEvents.instance.powerSelect, this.transform.position);
        }
    }

    public bool Interact()
    {
        if (interactPressed)
        {
            interactPressed = false;
            return true;
        }

        interactPressed = false;
        return false;
    }
    public void StatueRecharge()
    {
        health = maxHealth;
        CurrentEnergy = maxEnergy;
        onHealthChangedCallback.Invoke();
        //if we add anything else later we can add it here
    }

    IEnumerator WaitForIdleGlance()
    {
        idleGlancePlaying = true;
        yield return new WaitForSeconds(5);
        animator.SetTrigger("playGlance");
        idleGlancePlaying = false;
    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        animator.SetTrigger("TakeDamage");
        pState.recoilingX = true;
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
        if (health <= 0)
        {
            maxVelocity = 0.0f;
        }
    }
    private void UpdateSound()
    {
        // start footsteps event if the player has an x velocity and is on the ground
        if (rb.velocity.x != 0 && Grounded())
        {
            // get the playback state
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        // otherwise, stop the footsteps
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void Hiding()
    {
        if (currentAttackType == AttackType.Light || CurrentEnergy == 0)
        {
            Physics2D.IgnoreLayerCollision(0, 6, false);
            sr.sortingOrder = 1;
            pState.hiding = false;
            animator.SetBool("hiding", false);
            return;
        }

        if (pState.hiding)
        {
            StopRecoilX();
            StopRecoilY();
            rb.velocity = Vector2.zero;
            Physics2D.IgnoreLayerCollision(0, 6, true);
            sr.sortingOrder = -1;
            animator.SetBool("hiding", true);
            switch (currentAttackType)
            {
                case (AttackType.Neutral):
                    CurrentEnergy -= hidingEnergyLossRate * Time.deltaTime;
                    break;
                case (AttackType.Dark):
                    CurrentEnergy -= (hidingEnergyLossRate / 2) * Time.deltaTime;
                    break;
            }

            if (CurrentEnergy == 0)
            {
                Physics2D.IgnoreLayerCollision(0, 6, false);
                sr.sortingOrder = 1;
                animator.SetBool("hiding", false);
                pState.hiding = false;
                spellUsedRecently = true;
            }
        }

        if (dashPressed || jumpPressed || Interact())
        {
            Physics2D.IgnoreLayerCollision(0, 6, false);
            sr.sortingOrder = 1;
            animator.SetBool("hiding", false);
            pState.hiding = false;
            spellUsedRecently = true;
        }
    }

    private void DarkRoomBubble()
    {
        switch (currentAttackType)
        {
            case AttackType.Neutral:
                lightModeBubble.gameObject.SetActive(false);
                break;
            case AttackType.Dark:
                lightModeBubble.gameObject.SetActive(false);
                break;
            case AttackType.Light:
                lightModeBubble.gameObject.SetActive(true);
                lightModeBubble.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                break;
        }
    }

    private void ShadowBlockMovement()
    {
        if (currentAttackType == AttackType.Light || CurrentEnergy == 0)
        {
            Physics2D.IgnoreLayerCollision(0, 6, false);
            sr.sortingOrder = 1;
            rb.gravityScale = gravity;
            pState.shadowWalking = false;
            return;
        }

        if (pState.shadowWalking)
        {
            StopRecoilX();
            StopRecoilY();
            rb.gravityScale = 0;
            Physics2D.IgnoreLayerCollision(0, 6, true);
            sr.sortingOrder = -1;
            ShadowWalk();
            CurrentEnergy -= hidingEnergyLossRate * Time.deltaTime;

            if (CurrentEnergy == 0)
            {
                Physics2D.IgnoreLayerCollision(0, 6, false);
                sr.sortingOrder = 1;
                rb.gravityScale = gravity;
                pState.shadowWalking = false;
                spellUsedRecently = true;
            }
        }

        if (dashPressed || jumpPressed)
        {
            Physics2D.IgnoreLayerCollision(0, 6, false);
            sr.sortingOrder = 1;
            rb.gravityScale = gravity;
            pState.shadowWalking = false;
            spellUsedRecently = true;
        }
    }

    private void ShadowWalk()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, walkSpeed * yAxis);
    }

    private void DebugModeUnlocks()
    {
        canDash = true;
        health = maxHealth;
        lightUnlocked = true;
        darkUnlocked = true;
        fireballCost = 0;
        hidingEnergyLossRate = 0;
        bubbleEnergyLossRate = 0;
        onEnergyChangedCallback();
    }
}
