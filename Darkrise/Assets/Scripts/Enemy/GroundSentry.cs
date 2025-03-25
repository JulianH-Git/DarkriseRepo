using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSentry : enemyBase
{
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
        if (!isRecoiling && !isDying)
        {
            Patrol();
        }
        if (Mathf.Abs((transform.position.x - anchorPos.x) + 0.34f) >= patrolDistance && !isDying)
        {
            Retreat();
        }
    }

    public void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }
}
