using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FootSolider : enemyBase
{
    public enum SoldierBehavior
    {
        Patrol,
        Guard
    }
    [SerializeField] public SoldierBehavior behavior;
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
        if (isRecoiling || stunned || isDying) return;
        Collider2D playerInRange = PlayerCheck();
        switch (behavior)
        {
            case SoldierBehavior.Patrol:
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
            case SoldierBehavior.Guard:
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
            if (!chaseOnce)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.soldierDetected, this.transform.position);
                chaseOnce = true;
            }
            SetAggressive(true);
        }
    }

    protected void Patrol(Collider2D playerInRange, Vector2 _alertPos)
    {
        anim.SetBool("idle", true);
        base.Patrol(_alertPos);

        if (playerInRange != null && playerInRange.CompareTag("Player"))
        {
            SetAggressive(true);
        }
    }

    protected virtual void Chase(Collider2D playerInRange)
    {
        currentAggroTimer -= Time.deltaTime;
        retreating = false;
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
                SetAggressive(false);
                chaseOnce = false;
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
                PlayerController.Instance.HitStopTime(0.1f, 2, 0.5f);
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
            SetAggressive(true);
            
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
                if (playerInRange[i].CompareTag("Player") && !playerInRange[i].isTrigger)
                {
                    if(!PlayerController.Instance.pState.hiding)
                    {
                        return playerInRange[i];
                    }
                }
            }
        }
        return null;
    }

    public void GuardRetreat(Collider2D playerInRange)
    {
        if (aggressive)
        {
            retreating = false;
            return;
        }
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
            SetAggressive(true);
        }
    }

    protected void Guard(Collider2D _playerInRange)
    {
        anim.SetBool("idle", true);

        if (_playerInRange != null && _playerInRange.CompareTag("Player"))
        {
            SetAggressive(true);
        }
    }

    protected void Retreat(Collider2D _playerInRange)
    {
        if (aggressive)
        {
            retreating = false;
            return;
        }
        anim.SetBool("idle", false);
        base.Retreat();
        if (_playerInRange != null && _playerInRange.CompareTag("Player"))
        {
            SetAggressive(true);
        }
    }

    void SetAggressive(bool value)
    {
        aggressive = value;
        anim.SetBool("aggresive", value);
        anim.SetBool("idle", !value);
        if (!chaseOnce && value)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.soldierDetected, this.transform.position);
            chaseOnce = true;
        }
        else if(!value && chaseOnce)
        {
            chaseOnce = false;
        }
        if (value)
        {
            currentAggroTimer = aggressionTimer;
            retreating = false;
        }
    }
}
