using UnityEngine;

public class FootSolider : enemyBase
{
    [Header("Foot Soldier Attack Settings")]
    [SerializeField] private Transform footSoldierAttackTransform;
    [SerializeField] private Vector2 footSoldierAttackArea;
    [SerializeField] private Transform attackRangeTransform;
    [SerializeField] private float attackRange;
    [SerializeField] LayerMask layer;
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] float aggressionTimer;

    float timeSinceAttack;
    bool aggressive;
    float currentAggroTimer;

    [Header("Foot Soldier Detection Settings")]
    [SerializeField] private Transform detectionRangeTransform;
    [SerializeField] private Vector2 detectionRangeArea;
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

        Collider2D playerInRange = Physics2D.OverlapBox(detectionRangeTransform.position, detectionRangeArea, 0, layer);

        if(alerted)
        {
            Alerted(alertPos);
        }
        else if (retreating)
        {
            Retreat();
        }
        else if (alertedPatrol)
        {
            Patrol(playerInRange, alertPos);
        }
        else if (!aggressive)
        {
            Patrol(playerInRange);
        }
        else
        {
            Chase(playerInRange);
        }

    }

    protected void Patrol(Collider2D playerInRange)
    {
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
        base.Patrol(_alertPos);

        if (playerInRange != null && playerInRange.CompareTag("Player"))
        {
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

        rb.velocity = new Vector2(direction.x * (speed * 1.3f), rb.velocity.y);

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(attackRangeTransform.position, new Vector2(attackRange, 0.0f)); // attack range

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(footSoldierAttackTransform.position, footSoldierAttackArea); // attack hitbox

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(detectionRangeTransform.position, detectionRangeArea); // detection range for aggro mode
    }
}
