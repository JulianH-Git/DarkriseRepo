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
    [SerializeField] private LayerMask mirrorLayer;
    [SerializeField] bool isDark;
    [SerializeField] float reflectionCooldown;
    Vector3 direction;

    private float currentLifetime = 0f;
    public Animator animator;
    private bool isExploding = false;
    float reflectionCooldownTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        reflectionCooldownTimer = reflectionCooldown;
    }

    void FixedUpdate()
    {
        if (isExploding) return;

        currentLifetime += Time.deltaTime; 

        transform.position += speed * Time.deltaTime * direction;

        reflectionCooldownTimer += Time.deltaTime;

        if (currentLifetime >= maxLifetime)
        {
            Explode();

        }

        if (Physics2D.Raycast(transform.position, new Vector2(direction.x,direction.y), 0.3f, groundLayer))
        {
            Explode();
        }

        RaycastHit2D mirrorCheck = Physics2D.Raycast(transform.position, new Vector2(direction.x, direction.y), 0.35f, mirrorLayer);

        if (mirrorCheck.collider != null && mirrorCheck.collider.CompareTag("Mirror") && reflectionCooldownTimer >= reflectionCooldown)
        {
            reflectionCooldownTimer = 0f;
            MirrorPlate _mp = mirrorCheck.collider.GetComponentInParent<MirrorPlate>();
            if(_mp.currentState == MirrorPlate.PlateState.Reflect) { ReflectFireball(_mp.Rotation); }

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
                if(isDark)
                {
                    enemy.EnemyHit(damage, (collision.transform.position - transform.position).normalized, -hitForce,true);
                }
                else
                {
                    enemy.EnemyHit(damage, (collision.transform.position - transform.position).normalized, -hitForce,false);
                }
                
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

    void ReflectFireball(float _reflection)
    {
        Vector2 currentDir = direction;
        Vector2 normal = new Vector2(Mathf.Sin(-_reflection * Mathf.Deg2Rad), Mathf.Cos(_reflection * Mathf.Deg2Rad));
        Vector2 reflected = Vector2.Reflect(currentDir, normal).normalized;
        direction = new Vector3(reflected.x, reflected.y, 0);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
