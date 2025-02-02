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
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
                anim.SetBool("isRecoiling", false);
            }
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

    }

    public void Retreat()
    {
        direction = (anchorPos - new Vector2(transform.position.x, transform.position.y)).normalized; // which direction is the anchor?

        if (direction.x < 0)
        {
            transform.localScale = new Vector2(-(Mathf.Abs(transform.localScale.x)), transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2((Mathf.Abs(transform.localScale.x)), transform.localScale.y);
        }

        rb.velocity = new Vector2(direction.x * (speed * 1.3f), rb.velocity.y);

        if (new Vector2(transform.position.x, transform.position.y) == anchorPos)
        {
            Patrol();
        }
    }
}
