using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightSentry : GroundSentry
{
    [Header("Bright Sentry Settings")]
    [SerializeField] CircleCollider2D circleCollider;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            PlayerController.Instance.pState.safeFromRoomwide = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            PlayerController.Instance.pState.safeFromRoomwide = false;
        }
    }

    protected override void ObstacleCheck()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.right * Mathf.Sign(transform.localScale.x) * 0.75f;
        Vector2 direction = Vector2.right * Mathf.Sign(transform.localScale.x);
        float distance = 0.75f;

        Debug.DrawLine(origin, origin + direction * distance, Color.blue);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleMask);

        if (hit.collider != null && hit.collider.gameObject != gameObject && hit.collider != circleCollider)
        {
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy") || Physics2D.Raycast(origin, direction, distance, groundLayer))
            {
                timeTillForceTurn += Time.deltaTime;
                if (timeTillForceTurn >= forceTurnTimer)
                {
                    shouldFlip = true;
                    timeTillForceTurn = 0f;
                }
            }
            else
            {
                timeTillForceTurn = 0f;
            }
        }
        else
        {
            timeTillForceTurn = 0f;
        }
    }

}
