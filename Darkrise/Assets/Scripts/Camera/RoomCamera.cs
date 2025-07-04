using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    public GameObject virtualCamera;
    private Room room;

    private void Awake()
    {
        // This room's Room component
        room = GetComponent<Room>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCamera.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCamera.SetActive(false);

            // Respawn all enemies in this room when player leaves
            if (room != null)
            {
                room.RespawnAllEnemies();
            }
        }
    }
}