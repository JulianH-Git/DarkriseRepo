using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSolider : enemyBase
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolDistance;
    [Space(5)]

    [Header("Foot Solider Attack Settings")]
    [SerializeField] private Transform footSoldierAttackTransform;
    [SerializeField] private Vector2 footSoliderAttackArea;
    [SerializeField] private Transform attackRangeTransform;
    [SerializeField] private float attackRange;
    [SerializeField] LayerMask layer;
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] float aggressionTimer;

    Vector2 direction = new Vector2(0, 0);

    float timeSinceAttack;
    bool aggressive;
    float currentAggroTimer;
    [Space(5)]

    [Header("Foot Solider Detection Settings")]
    [SerializeField] private Transform detectionRangeTransform;
    [SerializeField] private Vector2 detectionRangeArea;


    private Vector2 anchorPos;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        rb.gravityScale = 12f;
        anchorPos = rb.position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        timeSinceAttack += Time.deltaTime;
    }

    protected void FixedUpdate()
    {
        if (!isRecoiling && aggressive == false)
        {
            Patrol();
        }
        if(!isRecoiling && aggressive)
        {
            Chase();
        }
    }

    public void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }

    protected void Patrol()
    {
        float distanceMoved = 0;

        distanceMoved = transform.position.x - anchorPos.x;

        if (Mathf.Abs(distanceMoved) >= patrolDistance)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            anchorPos = transform.position;
        }
        else
        {
            rb.velocity = new Vector2(speed * Mathf.Sign(transform.localScale.x), rb.velocity.y);
        }

        Collider2D playerInRange = Physics2D.OverlapBox(detectionRangeTransform.position, detectionRangeArea, 0, layer);

        if (playerInRange != null && playerInRange.CompareTag("Player"))
        {
            aggressive = true;
            anim.SetBool("aggresive", true);
        }

    }

    public override void Respawn()
    {
        health = maxHealth;
        transform.position = anchorPos;
        gameObject.SetActive(true);
    }

    protected virtual void Chase()
    {
        currentAggroTimer -= Time.deltaTime;

        Collider2D playerInRange = Physics2D.OverlapBox(detectionRangeTransform.position, detectionRangeArea, 0, layer);

        if(playerInRange != null)
        {
            direction = (playerInRange.transform.position - transform.position).normalized;
            if (direction.x < 0)
            {
                transform.localScale = new Vector2(-(Mathf.Abs(transform.localScale.x)), transform.localScale.y);
            }
            else
            {
                transform.localScale = new Vector2((Mathf.Abs(transform.localScale.x)), transform.localScale.y);
            }
        }

        rb.velocity = new Vector2(direction.x * (speed * 1.3f), rb.velocity.y);

        if(Physics2D.Raycast(attackRangeTransform.position, direction, attackRange, layer))
        {
            Attack();
        }
        if(currentAggroTimer <= 0)
        {
            aggressive = false;
            currentAggroTimer = aggressionTimer;
            anim.SetBool("aggresive", false);
        }

    }

    protected override void Attack()
    {
        if (timeSinceAttack >= timeBetweenAttacks)
        {
            anim.ResetTrigger("attack");
            timeSinceAttack = 0;

            Collider2D[] ObjectsToHit = Physics2D.OverlapBoxAll(footSoldierAttackTransform.position, footSoliderAttackArea, 0, layer);

            foreach (var obj in ObjectsToHit)
            {
                if (obj.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
                {
                    anim.SetTrigger("attack");
                    PlayerController.Instance.TakeDamage(damage);
                }
            }

        }
    }

    void OnDrawGizmos() // comment this out when we're done placing things to keep everything visible
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(attackRangeTransform.position, new Vector2(attackRange, 0.0f));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(footSoldierAttackTransform.position, footSoliderAttackArea);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(detectionRangeTransform.position, detectionRangeArea);
    }
}
