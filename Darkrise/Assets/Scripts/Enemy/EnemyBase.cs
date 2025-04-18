using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Alert Settings")]
    [SerializeField] protected float alertedPatrolDistance;
    [SerializeField] protected float alertedTimer;
    protected float alertedTimerCountdown;
    protected bool alerted;
    protected bool alertedPatrol;
    protected bool alertedFlip;
    protected Vector2 alertPos;
    [Space(5)]

    [Header("Audio Settings")]
    protected bool dieOnce = false;


    protected Rigidbody2D rb;

    protected float retreatTimer = 5.0f;
    protected bool retreating;
    protected bool isDying = false;


    protected BoxCollider2D boxCollider;

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
        boxCollider = GetComponent<BoxCollider2D>();
        alertedTimerCountdown = alertedTimer;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (health <= 0)
        {
            isDying = true;
            rb.simulated = false;
            boxCollider.enabled = false;
            anim.SetTrigger("death");

            //Temporary solution for differentiating between enemy death sounds
            //Currently based on how much damage an enemy is capable of doing to the player
            //Could probably be refactored to just be based on what type of enemy it is
            //dieOnce makes sure that the death noise doesn't loop forever since its in update
            if (!dieOnce)
            {
                if (damage == 1)
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.SentryDestroyed, this.transform.position);
                }
                else if (damage == 2) 
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.SoldierDestroyed, this.transform.position);
                }
                
                dieOnce = true;
            }
            


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
        if(alertedPatrol)
        {
            alertedTimerCountdown -= Time.deltaTime;
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

    protected virtual void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && !isDying)
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
        rb.simulated = true;
        boxCollider.enabled = true;
        gameObject.SetActive(true);
        isDying = false;
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
        rb.simulated = false;

        gameObject.SetActive(false);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = 255;
        sr.color = color;
    }

    public void DeathNoise()
    {
            
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
    protected virtual void Patrol(Vector2 alertAnchor)
    {
        float distanceMoved = transform.position.x - alertAnchor.x;

        if (Mathf.Abs(distanceMoved) >= alertedPatrolDistance + 0.34f) 
        {
            rb.velocity = new Vector2(0.0f, rb.velocity.y);

            if((alertedTimerCountdown <= alertedTimer / 2) && !alertedFlip) // turn halfway through
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                alertedFlip = true;
            }
        }
        else
        {
            rb.velocity = new Vector2(speed * Mathf.Sign(transform.localScale.x), rb.velocity.y);
        }

        if(alertedTimerCountdown <= 0)
        {
            alertedTimerCountdown = alertedTimer;
            alertedPatrol = false;
            retreating = true;
            alertedFlip = false;
            Retreat();
        }
        Debug.Log(transform.position);
    }

    public void Retreat()
    {
        direction = (anchorPos - (Vector2)transform.position).normalized;

        if (direction.x != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        rb.velocity = new Vector2(direction.x * (speed * 1.2f), rb.velocity.y);

        float test = Vector2.Distance(transform.position, anchorPos);

        if (test <= 0.36f || retreatTimer <= 0)
        {
            retreating = false;
            rb.velocity = Vector2.zero;
            retreatTimer = 5.0f;
            Debug.Log("Retreat finished");
            Patrol();
        }
    }

    public virtual void Alerted(Vector2 _alertPos)
    {
        Debug.Log("Alerted");
        alerted = true;
        alertPos = _alertPos;
        direction = (_alertPos - (Vector2)transform.position).normalized;

        if (direction.x != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        rb.velocity = new Vector2(direction.x * (speed * 1.3f), rb.velocity.y);

        float test = Vector2.Distance(transform.position, _alertPos);
        Debug.Log(test);

        if (test <= 0.89f)
        {
            Debug.Log("Completed alert - patrolling");
            alerted = false;
            alertedPatrol = true;
            Patrol(alertPos);
        }
    }


}
