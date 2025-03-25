using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb; // player rigid body
    private Animator animator;

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
    [SerializeField] private int maxLightEnergy;
    public int currentLightEnergy;
    [SerializeField] private int maxDarkEnergy;
    public int currentDarkEnergy;
    [SerializeField] int fireballCost;
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
    [Header("Other Objects")]
    [SerializeField] SpriteRenderer fade;
    float alpha = 1.0f;
    [SerializeField] float timeBetweenGlances;
    float countUptoGlance = 0;
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

    [Space(5)]
    public PlayerStateList pState; // this will be expanded a lot more after the MVI

    private Player player; // The Rewired Player
    public int playerId = 0; // The Rewired player id of this character
                             // Single player game so this is always 0, but good practice not to hardcode it

    public static PlayerController Instance;

    public GhostEffect ghost;


    private bool dashPressed;
    private bool jumpPressed;
    private bool doubleJumpPressed;
    private bool restartPressed;
    private bool switchAttackTypeLeftPressed;
    private bool switchAttackTypeRightPressed;
    private bool interactPressed;
    private bool idleGlancePlaying = false;

    private bool stopFading = false;

    public enum AttackType
    {
        Neutral,
        Light,
        Dark
    }

    public AttackType currentAttackType = AttackType.Neutral;

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

        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
    }

    private void Update()
    {
        if (alpha >= 0.0f && !stopFading)
        {
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alpha);
            alpha -= 0.05f;

            if (alpha < 0.0f) { stopFading = true; }
        }

        if (health > 0)
        {
            GetInput();
            UpdateJumpVariables();
        }
        else
        {
            animator.SetTrigger("isDead");
            while (alpha < 1.1f)
            {
                fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alpha);
                alpha += 0.05f;
            }
            StartCoroutine(WaitTillEnd());
        }

    }

    private IEnumerator WaitTillEnd()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("GameOverScreen");
    }

    void FixedUpdate()
    {
        countUptoGlance += Time.deltaTime;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle") && !idleGlancePlaying && countUptoGlance >= timeBetweenGlances)
        {
            StartCoroutine(WaitForIdleGlance());
            countUptoGlance = 0f;
        }
        else if(!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
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
        Flip();
        Move();
        Jump();
        SwitchAttackTypes();
        Attack();
        CastSpell();
        Recoil();
        StartDash();
        Restart();
        UpdateSound();
        // animation update
        animator.SetBool("isGrounded", Grounded());
        animator.SetFloat("yVel", rb.velocity.y);
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
        xAxis = player.GetAxis("Move Horizontal");
        yAxis = player.GetAxis("Move Vertical");

        if (!attack)
        {
            attack = player.GetButtonDown("Attack");
        }

        if(!castSpell)
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
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                doubleJumpPressed = !Grounded();
            }
        }
        if (Grounded())
        {
            doubleJumpPressed = false;
        }

        if (!restartPressed)
        {
            restartPressed = player.GetButtonDown("Restart");
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
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
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
                if (currentDarkEnergy > 0)
                {
                    if (attack && timeSinceAttack >= timeBetweenDarkAttacks)
                    {
                        PrepareToHit();
                        attack = false;
                    }
                }
                else
                {
                    if (attack && timeSinceAttack >= timeBetweenNeutralAttacks)
                    {
                        PrepareToHit();
                        attack = false;
                    }
                }
                break;
            case (AttackType.Light):
                if (currentLightEnergy > 0)
                {
                    if (attack && timeSinceAttack >= timeBetweenLightAttacks)
                    {
                        PrepareToHit();
                        attack = false;
                    }
                }
                else
                {
                    if (attack && timeSinceAttack >= timeBetweenNeutralAttacks)
                    {
                        PrepareToHit();
                        attack = false;
                    }
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
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        // The way this function works is it grabs every object within the area and then sorts out what is and isnt hittable. this could maybe optimized after MVI.

        Collider2D[] ObjectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (ObjectsToHit.Length > 0)
        {
            _recoilDir = true;
        }

        for (int i = 0; i < ObjectsToHit.Length; i++)
        {
            if (ObjectsToHit[i].GetComponent<enemyBase>() != null)
            {
                
                AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyHit, this.transform.position);
                switch (currentAttackType)
                {
                    case (AttackType.Neutral):
                        ObjectsToHit[i].GetComponent<enemyBase>().EnemyHit(damage, (transform.position - ObjectsToHit[i].transform.position).normalized, _recoilStrength);
                        onEnergyChangedCallback.Invoke();
                        break;
                    case (AttackType.Light):
                        if (currentLightEnergy > 0)
                        {
                            ObjectsToHit[i].GetComponent<enemyBase>().EnemyHit(damage + (-damage / 2.0f), (transform.position - ObjectsToHit[i].transform.position).normalized, _recoilStrength + (_recoilStrength));
                            currentLightEnergy--;
                            onEnergyChangedCallback.Invoke();
                        }
                        else
                        {
                            ObjectsToHit[i].GetComponent<enemyBase>().EnemyHit(damage, (transform.position - ObjectsToHit[i].transform.position).normalized, _recoilStrength);
                            onEnergyChangedCallback.Invoke();
                        }
                        break;

                    case (AttackType.Dark):
                        if (currentDarkEnergy > 0)
                        {
                            ObjectsToHit[i].GetComponent<enemyBase>().EnemyHit(damage * 2.0f, (transform.position - ObjectsToHit[i].transform.position).normalized, _recoilStrength * 2);
                            currentDarkEnergy--;
                            onEnergyChangedCallback.Invoke();
                        }
                        else
                        {
                            ObjectsToHit[i].GetComponent<enemyBase>().EnemyHit(damage, (transform.position - ObjectsToHit[i].transform.position).normalized, _recoilStrength);
                            onEnergyChangedCallback.Invoke();
                        }
                        break;
                }

            }
        }

    }

    void CastSpell()
    {
        if(castSpell && timeSinceCast >= timeToCast)
        {
            switch(currentAttackType)
            {
                case AttackType.Dark:
                    if(currentDarkEnergy >= fireballCost)
                    {
                        currentDarkEnergy -= fireballCost;
                        pState.casting = true;
                        timeSinceCast = 0;
                        CastFireball();
                    }
                break;

                case AttackType.Light:
                    if (currentLightEnergy >= fireballCost)
                    {
                        currentLightEnergy -= fireballCost;
                        pState.casting = true;
                        timeSinceCast = 0;
                        CastFireball();
                    }
                    break;
            }
        }
        else
        {
            timeSinceCast += Time.deltaTime;
            castSpell = false;
        }
    }

    void CastFireball() // this should become a coroutine later when we implement a casting animation.
    {
        //anim.SetBool("casting", true); - for if/when we get a fireball casting animation

        if (yAxis == 0 || yAxis < 0 && Grounded())
        {
            GameObject fireball = null;

            if (currentAttackType == AttackType.Light)
            {
                fireball = Instantiate(lightFireball, fireballTransform.position, Quaternion.identity);
            }
                
            else if (currentAttackType == AttackType.Dark)
            {
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
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }

    void Restart()
    {
        if (restartPressed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    void SwitchAttackTypes()
    {
        //determine if swapping "left" or "right" (basically are we going forward or back on the wheel)
        if(lightUnlocked && darkUnlocked)
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
                }
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
                }
            }
        }
        else if(darkUnlocked && !lightUnlocked)
        {
            SwitchAttackTypesDarkOnly();
        }
        else
        {
            currentAttackType = AttackType.Neutral;
            switchAttackTypeLeftPressed = false;
            switchAttackTypeRightPressed = false;
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
        currentDarkEnergy = maxDarkEnergy;
        currentLightEnergy = maxLightEnergy;
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
}
