using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb; // player rigid body

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
    [SerializeField] private int airJumps = 0; // keep this at 0 for now
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
    private bool attack = false;
    private float timeSinceAttack;

    [Header("Recoil Settings")]
    [SerializeField] private int recoilStepsX = 5;
    [SerializeField] private int recoilStepsY = 5;
    [SerializeField] private float recoilSpeedX = 100;
    [SerializeField] private float recoilSpeedY = 100;
    private int stepsRecoiledX;
    private int stepsRecoiledY;

    PlayerStateList pState; // this will be expanded a lot more after the MVI

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        Debug.Log("Instance created");
    }

        // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();

        rb = GetComponent<Rigidbody2D>();

        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        UpdateJumpVariables();
        if (pState.dashing) return; // if the player is dashing, don't get more movements
        Flip();
        Move();
        Jump();
        Attack();
        Recoil();
        StartDash();
    }

    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal"); //default left/right keys are the arrow keys or A and D
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetMouseButtonDown(0); // default attack button is mouse left click
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (attack && Grounded())
        {
            Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        }
        else if (attack && !Grounded())
        {
            Gizmos.DrawWireCube(airAttackTransform.position, airAttackArea);
        }
    }
    void Flip() // this will be useful for animation stuff later
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-(Mathf.Abs(transform.localScale.x)), transform.localScale.y);
            pState.lookingRight = false;
        }
        else if(xAxis > 0)
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
    }

    void StartDash()
    {
        if(Input.GetButtonDown("Dash") && canDash && !dashed) // dash key right now is left shift
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

        if(attack && timeSinceAttack >= timeBetweenAttacks)
        {
            timeSinceAttack = 0;
            //trigger attack animation
            //Debug.Log("Can attack again");

            if(Grounded())
            {
                if(pState.lookingRight) // lowkey a hotfix if statement lol. gotta look at this again after MVI
                {
                    Hit(sideAttackTransform, sideAttackArea, ref pState.recoilingX, recoilSpeedX);
                }
                else
                {
                    Hit(sideAttackTransform, sideAttackArea, ref pState.recoilingX, -recoilSpeedX);
                }
                
            }
            else if(!Grounded())
            {
                Hit(airAttackTransform, airAttackArea, ref pState.recoilingY, recoilSpeedY);
            }
        }
    }
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        // The way this function works is it grabs every object within the area and then sorts out what is and isnt hittable. this could maybe optimized after MVI.

        Collider2D[] ObjectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if(ObjectsToHit.Length > 0)
        {
            _recoilDir = true;
        }

        for(int i = 0;  i < ObjectsToHit.Length; i++)
        {
            if (ObjectsToHit[i].GetComponent<enemyBase>() != null)
            {
                ObjectsToHit[i].GetComponent<enemyBase>().EnemyHit(damage, (transform.position - ObjectsToHit[i].transform.position.normalized), _recoilStrength);
            }
        }

    }

    void Recoil()
    {
        if(pState.recoilingX)
        {
            if(pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilSpeedX, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilSpeedX, 0);
            }
        }
        if(pState.recoilingY)
        {
            if(!Grounded())
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
        if(pState.recoilingX && stepsRecoiledX < recoilStepsX)
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
        if(Grounded())
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
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0) // jump button is spacebar
        {
            rb.velocity = new Vector3(rb.velocity.x, 0);

            pState.jumping = false;
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
            else if (!Grounded() && airJumpCounter < airJumps && Input.GetButtonDown("Jump")) // air jump
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                pState.jumping = true;
                airJumpCounter++;
            }
        }
    }
    void UpdateJumpVariables()
    {
        if(Grounded())
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
        if (Input.GetButtonDown("Jump")) // jump button is spacebar
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - Time.deltaTime);
        }
    }
}
