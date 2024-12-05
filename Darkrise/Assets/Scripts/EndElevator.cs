using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndElevator : MonoBehaviour
{
    [SerializeField]
    private PlayerController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            controller.health = 0;
        }

    }
}
