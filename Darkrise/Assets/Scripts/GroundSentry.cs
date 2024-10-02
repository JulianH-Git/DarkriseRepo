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
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(!isRecoiling)
        {
            //transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
    }

    public void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }
    void OnDrawGizmos()
    {
        float healthRatio = health / maxHealth;

        // Adjust position so the health bar decreases from the left
        Vector3 barPosition = new Vector3(rb.position.x - (0.5f * (1f - healthRatio)), rb.position.y + 1f, 1);

        // Draw the filled part of the health bar
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(barPosition, new Vector3(1f * healthRatio, 0.3f, 1f));

        // Draw the outline of the health bar
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(rb.position.x, rb.position.y + 1f, 1), new Vector3(1f, 0.3f, 1f));
    }
}
