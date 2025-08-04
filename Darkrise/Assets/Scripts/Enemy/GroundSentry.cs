using UnityEngine;

public class GroundSentry : enemyBase
{
    // Start is called before the first frame update
    private GameObject head;

    protected override void Awake()
    {
        base.Awake();
        head = transform.Find("Head").gameObject;
        rb.gravityScale = 12f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected void FixedUpdate()
    {
        if (isRecoiling || isDying || stunned) 
        {
            head.SetActive(false);
            return; 
        }

        head.SetActive(true);

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
