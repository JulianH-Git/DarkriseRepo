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
}
