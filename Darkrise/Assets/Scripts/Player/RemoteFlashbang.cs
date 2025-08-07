using Rewired.Data.Mapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteFlashbang : MonoBehaviour
{
    [SerializeField] Vector2 areaOfEffect;
    [SerializeField] GameObject areaOfEffectTransform;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask techLayer;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject explodeParticles;
    GameObject _explodeParticles;
    PlayerController player;
    Animator anim;
    Rigidbody2D rb;
    [SerializeField] private float speed;
    bool detonated;
    Vector3 direction;
    float currentLifetime;
    [SerializeField] float maxLifeTime;
    ContactFilter2D ctf;


    public bool Detonated
    {
        get { return detonated; }
        set { detonated = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.velocity = new Vector2(speed * direction.x, rb.velocity.y);
        Physics2D.IgnoreLayerCollision(10, 6, true);
        Physics2D.IgnoreLayerCollision(10, 0, true);
        ctf.useTriggers = true;
        ctf.SetLayerMask(techLayer);
        _explodeParticles = Instantiate(explodeParticles, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        // check if detonated
        if (Detonated || currentLifetime >= maxLifeTime)
        {
            ActivateFlashbang();
        }
    }

    private void FixedUpdate()
    {
        // check if grounded
        if (Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer))
        {
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
        currentLifetime += Time.deltaTime;
        if (_explodeParticles != null) { _explodeParticles.transform.position = transform.position; }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaOfEffectTransform.transform.position, areaOfEffect);
    }

    void ActivateFlashbang()
    {
        rb.gravityScale = 0.0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        Destroy(_explodeParticles);
        anim.SetTrigger("explode");
        // if so, check for enemies and stun them
        List<Collider2D> enemiesInRange = CheckForEnemies(areaOfEffectTransform.transform, areaOfEffect);
        if (enemiesInRange != null && enemiesInRange.Count > 0)
        {
            StunEnemies(enemiesInRange);
        }
        // and check for tech and disable it
        CheckForTech(areaOfEffectTransform.transform, areaOfEffect);
    }

    List<Collider2D> CheckForEnemies(Transform _roomTransform, Vector2 _roomArea)
    {
        Collider2D[] ObjectsToHit = Physics2D.OverlapBoxAll(_roomTransform.position, _roomArea, 0, enemyLayer);
        List<Collider2D> enemiesInRange = new List<Collider2D>();

        for (int i = 0; i < ObjectsToHit.Length; i++)
        {
            if (ObjectsToHit[i].GetComponent<enemyBase>() != null)
            {
                enemiesInRange.Add(ObjectsToHit[i]);
            }
        }
        return enemiesInRange;
    }

    void StunEnemies(List<Collider2D> enemies)
    {
        foreach (Collider2D obj in enemies)
        {
            obj.GetComponent<enemyBase>().EnemyHit(999f, transform.position, 0.1f, false);
        }
    }

    void CheckForTech(Transform _roomTransform, Vector2 _roomArea)
    {
        Collider2D[] ObjectsToHit = new Collider2D[5];
        Physics2D.OverlapBox(_roomTransform.position, _roomArea, 0, ctf, ObjectsToHit);
        List<Collider2D> techInRange = new List<Collider2D>();

        for (int i = 0; i < ObjectsToHit.Length; i++)
        {
            if (ObjectsToHit[i] != null)
            {
                if (ObjectsToHit[i].CompareTag("Technology"))
                {
                    techInRange.Add(ObjectsToHit[i]);
                }
            }
        }

        foreach (Collider2D obj in techInRange)
        {
            if (obj.GetComponent<StandardBreakerSwitch>() != null)
            {
                obj.GetComponent<StandardBreakerSwitch>().flashbanged = true;
            }
            if (obj.GetComponent<ForcedEncounterBreakerSwitch>() != null)
            {
                obj.GetComponent<ForcedEncounterBreakerSwitch>().flashbanged = true;
            }
            if (obj.GetComponent<Alarm>() != null)
            {
                obj.GetComponent<Alarm>().flashbanged = true;
            }
            if (obj.GetComponent<FuseBox>() != null)
            {
                obj.GetComponent<FuseBox>().flashbanged = true;
            }
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

    public void OnExplodeComplete()
    {
        // then destroy myself
        Destroy(gameObject);
    }
}
