using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool canDash = true;
    private bool dashed;
    private float gravity;
    [Space(5)]

    [Header("Attack Settings")]
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float damage;
    [SerializeField] private Transform sideAttackTransform;
    [SerializeField] private Transform airAttackTransform;
    [SerializeField] private Vector2 sideAttackArea;
    [SerializeField] private Vector2 airAttackArea;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private GameObject slashEffect;
    private bool attack = false;
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
    }

    private void Update()
    {
        GetInput();
        UpdateJumpVariables();
    }

    void FixedUpdate()
    {
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
        Attack();
        Recoil();
        StartDash();
        Restart();
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

        if (!dashPressed)
        {
            dashPressed = player.GetButtonDown("Dash");
        }

        if (!jumpPressed)
        {
            jumpPressed = player.GetButtonDown("Jump");
        }

        if (jumpPressed && !doubleJumpPressed)
        {
            doubleJumpPressed = player.GetButtonDown("Jump");
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

        if (attack && timeSinceAttack >= timeBetweenAttacks)
        {
            animator.SetTrigger("attack");
            timeSinceAttack = 0;
            //trigger attack animation
            //Debug.Log("Can attack again");

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

            attack = false;
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
                ObjectsToHit[i].GetComponent<enemyBase>().EnemyHit(damage, (transform.position - ObjectsToHit[i].transform.position).normalized, _recoilStrength);
            }
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
            rb.velocity = new Vector3(rb.velocity.x, 0);

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
            else if (!Grounded() && airJumpCounter < airJumps && doubleJumpPressed) // air jump
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                pState.jumping = true;
                airJumpCounter++;
                jumpPressed = false;
                doubleJumpPressed = false;
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

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        animator.SetTrigger("TakeDamage");
        pState.recoilingX = true;
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

}
