using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBase : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling;

    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;

    float recoilTimer;

    protected Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected virtual void Awake()
    {
        Debug.Log("Enemy awake called");
        rb = GetComponent<Rigidbody2D>();
        //player = PlayerController.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
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
            }
        }
    }

    public void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;

        if(!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }
}
