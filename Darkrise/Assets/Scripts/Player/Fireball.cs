using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float hitForce;
    [SerializeField] private float speed;
    [SerializeField] private float maxLifetime = 1.0f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isDark;
    Vector3 direction;

    private float currentLifetime = 0f;
    public Animator animator;
    private bool isExploding = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (isExploding) return;

        currentLifetime += Time.deltaTime; 

        transform.position += speed * Time.deltaTime * direction;

        if (currentLifetime >= maxLifetime)
        {
            Explode();

        }

        if (Physics2D.Raycast(transform.position, new Vector2(direction.x,direction.y), 0.3f, groundLayer))
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploding) return;
        if (collision.CompareTag("Player")) return;

        if (collision.CompareTag("Enemy"))
        {
            enemyBase enemy = collision.GetComponent<enemyBase>();
            if (enemy != null)
            {
                enemy.EnemyHit(damage, (collision.transform.position - transform.position).normalized, -hitForce);
            }
            Explode();
        }
    }

    public void SetDirection(bool isFacingRight)
    {
        direction = isFacingRight ? Vector3.right : Vector3.left;
        transform.localScale = new Vector3(
            isFacingRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void Explode()
    {
        isExploding = true;
        animator.SetTrigger("explosion");
        float explosionDuration = animator.GetCurrentAnimatorStateInfo(0).length * 2;
        if (isDark)
        {
            transform.localScale = new Vector3(transform.localScale.x * 2, transform.localScale.y * 2, transform.localScale.z * 2);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.darkExplode, this.transform.position);
        }
        else
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.lightExplode, this.transform.position);
        }
        Destroy(gameObject, explosionDuration);
    }
}
