using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class FallDownTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    GameObject teleportPoint;

    [SerializeField]
    bool isGap = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            player.transform.localPosition = teleportPoint.transform.localPosition;

            if (isGap) 
            {
                controller.TakeDamage(1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
