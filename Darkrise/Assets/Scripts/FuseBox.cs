using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : MonoBehaviour
{
    [SerializeField] float overloadTimer;
    [SerializeField] float flashbangDeactivationTimer;
    float timeTilReactivated;
    float timeToOverload;
    public bool powered;
    public bool overloaded;
    public bool flashbanged;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(powered && !overloaded)
        {
            sr.color = Color.yellow;
        }
        if(powered && overloaded)
        {
            sr.color = Color.red;
        }
        if(powered && overloaded && flashbanged)
        {
            sr.color = Color.red;

        }
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.isTrigger && !powered && PlayerController.Instance.currentAttackType == PlayerController.AttackType.Light)
        {
            powered = true;
            sr.color = Color.yellow;
            Debug.Log("Powered");
        }
        if(collision.CompareTag("Player") && collision.isTrigger && powered && PlayerController.Instance.BubbleUp)
        {
            timeToOverload = timeToOverload + Time.deltaTime;
            if(timeToOverload >= overloadTimer)
            {
                sr.color = Color.red;
                overloaded = true;
                Debug.Log("Overloaded");
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(powered && !overloaded)
            {
                timeToOverload -= Time.deltaTime;
            }
        }
    }

    protected void Flashbanged()
    {
        timeTilReactivated += Time.deltaTime;

        if (timeTilReactivated >= flashbangDeactivationTimer)
        {
            flashbanged = false;
            overloaded = false;
        }
    }
}
