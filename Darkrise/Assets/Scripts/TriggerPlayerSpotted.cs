using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayerSpotted : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            Debug.Log("Player spotted");
        }
    }
}
