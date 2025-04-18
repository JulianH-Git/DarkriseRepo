using UnityEngine;

public class FootSolider : enemyBase
{
    public enum Behavior
    {
        Patrol,
        Guard
    }
    [SerializeField] public Behavior behavior;
    [Header("Foot Soldier Attack Settings")]
    [SerializeField] private Transform footSoldierAttackTransform;
    [SerializeField] private Vector2 footSoldierAttackArea;
    [SerializeField] private Transform attackRangeTransform;
    [SerializeField] private float attackRange;
    [SerializeField] public LayerMask layer;
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] float aggressionTimer;

    float timeSinceAttack;
    public bool aggressive;
    float currentAggroTimer;

    [Header("Foot Soldier Detection Settings")]
    [SerializeField] public Transform detectionRangeTransform;
    [SerializeField] public Vector2 detectionRangeArea;

    bool chaseOnce = false;
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        rb.gravityScale = 12f;
        anchorPos = rb.position;
    }

    protected override void Update()
    {
        base.Update();
        timeSinceAttack += Time.deltaTime;
    }

    protected void FixedUpdate()
    {
        if (isRecoiling) return;
        Collider2D playerInRange = PlayerCheck();

        switch (behavior)
        {
            case Behavior.Patrol:
                if (aggressive)
                {
                    Chase(playerInRange);
                    //Should probably be adjusted later to just trigger when the Soldier sees the player but the moment he detects you wasn't clear
                    if(!chaseOnce)
                    {
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.SoldierDetected, this.transform.position);
                        chaseOnce = true;
                    }
                    
                }
                if (alerted && !aggressive)
                {
                    Alerted(playerInRange, alertPos);
                }
                else if (retreating)
                {
                    Retreat(playerInRange);
                }
                else if (alertedPatrol && !aggressive)
                {
                    Patrol(playerInRange, alertPos);
                }
                else if (!aggressive)
                {
                    Patrol(playerInRange);
                }
            break;
            case Behavior.Guard:
                if (aggressive)
                {
                    Chase(playerInRange);
                }
                if (alerted && !aggressive)
                {
                    Alerted(playerInRange, alertPos);
                }
                else if (retreating)
                {
                    GuardRetreat(playerInRange);
                }
                else if (alertedPatrol && !aggressive)
                {
                    Patrol(playerInRange, alertPos);
                }
                else if (!aggressive)
                {
                    Guard(playerInRange);
                }
                break;

        }

    }

    protected void Patrol(Collider2D playerInRange)
    {
        anim.SetBool("idle", false);
        base.Patrol();

        if (playerInRange != null && playerInRange.CompareTag("Player"))
        {
            aggressive = true;
            currentAggroTimer = aggressionTimer;
            anim.SetBool("aggresive", true);
        }
    }

    protected void Patrol(Collider2D playerInRange, Vector2 _alertPos)
    {
        anim.SetBool("idle", true);
        base.Patrol(_alertPos);

        if (playerInRange != null && playerInRange.CompareTag("Player"))
        {
            anim.SetBool("idle", false);
            aggressive = true;
            currentAggroTimer = aggressionTimer;
            anim.SetBool("aggresive", true);
        }
    }

    protected virtual void Chase(Collider2D playerInRange)
    {
        currentAggroTimer -= Time.deltaTime;
        if (playerInRange != null)
        {
            currentAggroTimer = aggressionTimer;

            direction = (playerInRange.transform.position - transform.position).normalized;

            if ((direction.x < 0 && transform.localScale.x > 0) || (direction.x > 0 && transform.localScale.x < 0))
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }

        rb.velocity = new Vector2(direction.x * (speed * 2f), rb.velocity.y);

        if (timeSinceAttack >= timeBetweenAttacks && Physics2D.Raycast(attackRangeTransform.position, direction, attackRange, layer))
        {
            Attack();
        }

        if (playerInRange == null || currentAggroTimer <= 0)
        {
            if (!retreating)
            {
                aggressive = false;
                currentAggroTimer = aggressionTimer;
                anim.SetBool("aggresive", false);
                retreating = true;
            }
        }
    }

    protected override void Attack()
    {
        if (timeSinceAttack < timeBetweenAttacks) return;

        timeSinceAttack = 0;

        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(footSoldierAttackTransform.position, footSoldierAttackArea, 0, layer);

        foreach (var obj in objectsToHit)
        {
            if (obj.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
            {
                anim.SetTrigger("attack");
                PlayerController.Instance.TakeDamage(damage);
            }
        }
    }

    public void Alerted(Collider2D _playerInRange, Vector2 _alertPos)
    {
        anim.SetBool("idle", false);
        if (!aggressive)
        {
            base.Alerted(_alertPos);
        }
        

        if (_playerInRange != null && _playerInRange.CompareTag("Player"))
        {
            aggressive = true;
            currentAggroTimer = aggressionTimer;
            anim.SetBool("aggresive", true);
            
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(attackRangeTransform.position, new Vector2(attackRange, 0.0f)); // attack range

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(footSoldierAttackTransform.position, footSoldierAttackArea); // attack hitbox

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(detectionRangeTransform.position, detectionRangeArea); // detection range for aggro mode
    }

    public Collider2D PlayerCheck()
    {
        Collider2D[] playerInRange = Physics2D.OverlapBoxAll(detectionRangeTransform.position, detectionRangeArea, 0, layer);
        if (playerInRange != null)
        {
            for (int i = 0; i < playerInRange.Length; i++)
            {
                if (playerInRange[i].CompareTag("Player"))
                {
                    return playerInRange[i];
                }
            }
        }
        return null;
    }

    public void GuardRetreat(Collider2D playerInRange)
    {
        anim.SetBool("idle", false);
        direction = (anchorPos - (Vector2)transform.position).normalized;

        if (direction.x != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        rb.velocity = new Vector2(direction.x * (speed * 1.3f), rb.velocity.y);

        float test = Vector2.Distance(transform.position, anchorPos);

        if (test <= 0.36f || retreatTimer <= 0)
        {
            retreating = false;
            rb.velocity = Vector2.zero;
            retreatTimer = 5.0f;
            Guard(playerInRange);
        }

        if (playerInRange != null && playerInRange.CompareTag("Player"))
        {
            anim.SetBool("idle", false);
            aggressive = true;
            currentAggroTimer = aggressionTimer;
            anim.SetBool("aggresive", true);
        }
    }

    protected void Guard(Collider2D _playerInRange)
    {
        anim.SetBool("idle", true);

        if (_playerInRange != null && _playerInRange.CompareTag("Player"))
        {
            anim.SetBool("idle", false);
            aggressive = true;
            currentAggroTimer = aggressionTimer;
            anim.SetBool("aggresive", true);
        }
    }

    protected void Retreat(Collider2D _playerInRange)
    {
        base.Retreat();
        if (_playerInRange != null && _playerInRange.CompareTag("Player"))
        {
            anim.SetBool("idle", false);
            aggressive = true;
            currentAggroTimer = aggressionTimer;
            anim.SetBool("aggresive", true);
        }
    }
}
