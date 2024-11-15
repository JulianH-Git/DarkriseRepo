using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSentry : enemyBase
{
    [SerializeField] private float patrolDistance;

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
    }

    protected void FixedUpdate()
    {
        if (!isRecoiling)
        {
            Patrol();
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

    }

    protected override void Respawn()
    {
        health = maxHealth;
        transform.position = anchorPos;
        gameObject.SetActive(true);
    }
}
