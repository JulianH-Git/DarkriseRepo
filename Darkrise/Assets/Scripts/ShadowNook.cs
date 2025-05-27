using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowNook : InteractTrigger
{
    bool playerHiding = false;
    bool active = true;
    
    // Update is called once per frame
    void Update()
    {
        if(playerHiding) { indicator.SetActive(false); }
    }
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            indicator.SetActive(false);
            playerHiding = false;
            controller.pState.hiding = false;
        }
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Room Wide Effect") && collision.isTrigger)
        {
            active = false;
        }
        else if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            TriggerActivated();
        }
    }
    protected override void TriggerActivated()
    {
        if(active)
        {
            if (controller.currentAttackType != PlayerController.AttackType.Light)
            {
                indicator.SetActive(true);
            }

            if (controller.Interact() && controller.currentAttackType != PlayerController.AttackType.Light)
            {
                indicator.SetActive(false);
                playerHiding = true;
                controller.pState.hiding = true;
                if (controller.currentAttackType != PlayerController.AttackType.Light)
                {
                    controller.transform.position = new Vector2(transform.position.x, controller.transform.position.y); // this might not work if the player is on a slope, so keep that in mind
                }
            }
        }
    }

    // might come back to this to make the enemies transition a bit smoother

    /*void SetEnemiesAlerted()
    {
        Collider2D[] EnemiesToAlert = Physics2D.OverlapBoxAll(transform.position, new Vector2(3.0f,3.0f), 0, 6);

        if(EnemiesToAlert != null)
        {
            foreach(Collider2D c in EnemiesToAlert)
            {
                if(c.GetComponent<FootSolider>() != null)
                {
                    c.GetComponent<FootSolider>().Alerted(transform.position);
                }
            }
        }
    }*/
}
