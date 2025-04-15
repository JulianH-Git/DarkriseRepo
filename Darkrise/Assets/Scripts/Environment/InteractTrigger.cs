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
    [SerializeField] protected SpriteRenderer indicateColor;
    [SerializeField] protected GameObject indicator;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        controller = player.GetComponent<PlayerController>();
        indicator.SetActive(false);
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            TriggerActivated();
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            indicator.SetActive(false);
            indicateColor.color = Color.white;
        }
    }

    // Update is called once per frame
    protected virtual void TriggerActivated()
    {

    }
}
