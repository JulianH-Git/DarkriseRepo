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
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(health <= 0)
        {
            anim.SetTrigger("death");
            gameObject.SetActive(false);
        }
        if(isRecoiling)
        {
            if(recoilTimer < recoilLength)
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

    public void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;

        if(!isRecoiling)
        {
            anim.SetBool("isRecoiling", true);
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
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
    }
}
