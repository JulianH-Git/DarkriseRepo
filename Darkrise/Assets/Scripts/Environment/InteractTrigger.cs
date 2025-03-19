using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using Rewired;

public class InteractTrigger : MonoBehaviour
{
    [SerializeField] protected GameObject player;
    protected PlayerController controller;

    [SerializeField] protected GameObject teleportPoint;

    [SerializeField] protected SpriteRenderer indicateColor;

    [SerializeField] protected bool isGap = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        controller = player.GetComponent<PlayerController>();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (isGap)
            {
                Vector2 point = new Vector2(teleportPoint.transform.position.x, teleportPoint.transform.position.y);

                player.transform.localPosition = point;
                controller.TakeDamage(1);
            }
            else 
            {
                indicateColor.color = Color.green;

                if (controller.Interact())
                {
                    Vector2 point = new Vector2(teleportPoint.transform.position.x, teleportPoint.transform.position.y);

                    player.transform.localPosition = point;
                }
            }

        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isGap)
        {
            indicateColor.color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
