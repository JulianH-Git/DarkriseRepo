using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TriggerPlayerSpotted : MonoBehaviour
{
    // player state
    [SerializeField]
    private PlayerStateList playerState;

    // player spotted
    private bool playerSpotted = false;
    public bool PlayerSpotted { get => playerSpotted; }

    // cooldown
    [SerializeField]
    private float cooldown = 2f; // 2 seconds
    private float cooldownTimer = 0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (!playerState.dashing)
            {
                playerSpotted = true;
                cooldownTimer = cooldown;
            }
        }
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            playerSpotted = false;
        }
    }
}
