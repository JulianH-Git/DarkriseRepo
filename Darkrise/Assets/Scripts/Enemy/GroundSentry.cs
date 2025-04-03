using UnityEngine;

public class GroundSentry : enemyBase
{
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 12f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected void FixedUpdate()
    {
        if (isRecoiling || isDying) return;

        if (alerted)
        {
            Alerted(alertPos);
        }
        else if (alertedPatrol)
        {
            Patrol(alertPos);
        }
        else if (!alerted && Vector2.Distance(transform.position, anchorPos) >= patrolDistance && !isDying)
        {
            Retreat();
        }
        else
        {
            Patrol();
        }
    }
}
