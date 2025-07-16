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

    [Header("Stun Settings")]
    [SerializeField] protected float stun;
    [SerializeField] protected float maxStun;
    [SerializeField] protected float stunTimer;
    protected float timeToReleaseStun;
    protected bool stunned;
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
    [SerializeField] public float patrolDistance;
    public Vector2 anchorPos;
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

    [Header("Other Settings")]
    [SerializeField] protected GameObject gotHitParticles;


    protected Rigidbody2D rb;

    protected float retreatTimer = 5.0f;
    protected bool retreating;
    protected bool isDying = false;

    protected BoxCollider2D[] boxCollider;

    protected const float RELEASEALERT = 2.0f;
    protected float releaseAlertTimer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
        anim = GetComponent<Animator>();
        anchorPos = rb.position;
        boxCollider = GetComponents<BoxCollider2D>();
        alertedTimerCountdown = alertedTimer;
        releaseAlertTimer = RELEASEALERT;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (health <= 0)
        {
            isDying = true;
            rb.simulated = false;
            for (int i = 0; i < boxCollider.Length; i++)
            {
                boxCollider[i].enabled = false;
            }
            Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), PlayerController.Instance.GetComponent<Collider2D>(),  true);
            anim.SetTrigger("death");

            //Temporary solution for differentiating between enemy death sounds
            //Currently based on how much damage an enemy is capable of doing to the player
            //Could probably be refactored to just be based on what type of enemy it is
            //dieOnce makes sure that the death noise doesn't loop forever since its in update
            if (!dieOnce)
            {
                GameObject _gotHitParticles = Instantiate(gotHitParticles, transform.position, Quaternion.identity);
                Destroy(_gotHitParticles, 1.5f);
                if (this.GetComponent<GroundSentry>())
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.sentryDestroyed, this.transform.position);
                }
                else if (this.GetComponent<FootSolider>()) 
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.soldierDestroyed, this.transform.position);
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
        if(alerted) { releaseAlertTimer -= Time.deltaTime; }
        if(stun >= maxStun)
        {
            Stunned();
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce, bool _damage)
    {
        if(stunned) { return; }
        if (_damage) { health -= _damageDone; }
        else { stun += _damageDone; }
        
        if (!isRecoiling)
        {
            anim.SetBool("isRecoiling", true);
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
        
    }

    protected virtual void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") 
            && !PlayerController.Instance.pState.invincible 
            && !PlayerController.Instance.pState.hiding 
            && !isDying 
            && !stunned
            && health > 0)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        if(PlayerController.Instance.Health > 0)
        {
            PlayerController.Instance.TakeDamage(damage);
        }
    }
    public virtual void Respawn()
    {
        health = maxHealth;
        rb.simulated = true;
        for (int i = 0; i < boxCollider.Length; i++)
        {
            boxCollider[i].enabled = true;
        }
        gameObject.SetActive(true);
        isDying = false;
        anim.ResetTrigger("death");
        anim.Play("idle");
        SetPosition(anchorPos);
        dieOnce = false;
        if(this.GetComponent<FootSolider>() != null)
        {
            this.GetComponent<FootSolider>().aggressive = false;
            this.GetComponent<FootSolider>().alerted = false;
        }
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

    public void Die()
    {
        health = 0;
    }

    public void DeathNoise()
    {
            
    }

    protected virtual void Stunned()
    {
        stunned = true;
        anim.SetBool("isStunned", true);
        timeToReleaseStun += Time.deltaTime;
        Physics2D.IgnoreCollision(boxCollider[0], PlayerController.Instance.GetComponent<Collider2D>(), true);

        if (timeToReleaseStun >= stunTimer)
        {
            stunned = false;
            anim.SetBool("isStunned", false);
            Physics2D.IgnoreCollision(boxCollider[0], PlayerController.Instance.GetComponent<Collider2D>(), false);
            timeToReleaseStun = 0;
            stun = 0;
        }
    }

    protected virtual void Patrol()
    {
        float distanceMoved = transform.position.x - anchorPos.x;
        float directionX = Mathf.Sign(transform.localScale.x);

        if (Mathf.Abs(distanceMoved) >= patrolDistance + 0.34f)
        {
            directionX = Mathf.Sign(anchorPos.x - transform.position.x);
            transform.localScale = new Vector2(directionX * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        rb.velocity = new Vector2(speed * directionX, rb.velocity.y);
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
    }

    public void Retreat()
    {
        float distanceToAnchorX = Mathf.Abs(rb.position.x - anchorPos.x);

        if (distanceToAnchorX <= 0.36f || retreatTimer <= 0)
        {
            retreating = false;
            rb.velocity = Vector2.zero;
            retreatTimer = 5.0f;
            Patrol();
            return;
        }

        direction = new Vector2(anchorPos.x - rb.position.x, 0).normalized;

        if (direction.x != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        rb.velocity = new Vector2(direction.x * (speed * 1.2f), rb.velocity.y);
        retreatTimer -= Time.deltaTime;
    }

    public virtual void Alerted(Vector2 _alertPos)
    {
        alerted = true;
        alertPos = _alertPos;
        direction = (_alertPos - (Vector2)transform.position).normalized;

        if (direction.x != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        rb.velocity = new Vector2(direction.x * (speed * 1.3f), rb.velocity.y);

        float test = Vector2.Distance(transform.position, _alertPos);

        if (test <= 0.89f || releaseAlertTimer <= 0)
        {
            releaseAlertTimer = RELEASEALERT;
            alerted = false;
            alertedPatrol = true;
            Patrol(alertPos);
        }
    }

    public void CancelAlert()
    {
        alertedTimerCountdown = alertedTimer;
        alertedPatrol = false;
        retreating = true;
        alertedFlip = false;
        Retreat();
    }

}
