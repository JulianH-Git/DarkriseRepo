using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBase : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [Space(5)]

    [Header("Recoil Settings")]
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling;
    [Space(5)]

    [Header("Attack Settings")]
    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;

    protected float recoilTimer;
    protected Animator anim;

    [Header("Patrol Settings")]
    [SerializeField] protected float patrolDistance;
    protected Vector2 anchorPos;
    protected Vector2 direction;
    [Space(5)]

    protected Rigidbody2D rb;

    protected float retreatTimer = 5.0f;
    protected bool retreating;

    // Start is called before the first frame update
    void Start()
    {

    }

    protected virtual void Awake()
    {
        Debug.Log("Enemy awake called");
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
        anim = GetComponent<Animator>();
        anchorPos = rb.position;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (health <= 0)
        {
            anim.SetTrigger("death");
            //gameObject.SetActive(false);
        }
        if (isRecoiling)
        {
            recoilTimer += Time.deltaTime;
            if (recoilTimer >= recoilLength)
            {
                isRecoiling = false;
                recoilTimer = 0;
                anim.SetBool("isRecoiling", false);
            }
        }
        if (retreating)
        {
            retreatTimer -= Time.deltaTime;
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;

        if (!isRecoiling)
        {
            anim.SetBool("isRecoiling", true);
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
    public virtual void Respawn()
    {
        health = maxHealth;
        gameObject.SetActive(true);
        anim.ResetTrigger("death");
        anim.Play("idle");
        SetPosition(anchorPos);
    }

    public virtual void SetPosition(Vector2 pos)
    {
        rb.position = pos;
    }

    public void OnDeathComplete()
    {
        Debug.Log("enemy dead");
        gameObject.SetActive(false);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = 255;
        sr.color = color;
    }

    protected virtual void Patrol()
    {
        float distanceMoved = transform.position.x - anchorPos.x;

        if (Mathf.Abs(distanceMoved) >= patrolDistance + 0.34f)
            {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

        rb.velocity = new Vector2(speed * Mathf.Sign(transform.localScale.x), rb.velocity.y);

    }

    public void Retreat()
    {
        direction = (anchorPos - (Vector2)transform.position).normalized;

        if (direction.x != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        rb.velocity = new Vector2(direction.x * (speed * 1.3f), rb.velocity.y);

        float test = Vector2.Distance(transform.position, anchorPos);

        if (test <= 0.34f || retreatTimer <= 0)
        {
            retreating = false;
            rb.velocity = Vector2.zero;
            retreatTimer = 5.0f;
            Debug.Log("Retreat finished");
            Patrol();
        }
    }


}
