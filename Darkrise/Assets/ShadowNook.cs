using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowNook : InteractTrigger
{
    bool playerHiding = false;
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

    protected override void TriggerActivated()
    {
        indicator.SetActive(true);

        if(controller.Interact())
        {
            indicator.SetActive(false);
            playerHiding = true;
            controller.pState.hiding = true;
            if(controller.currentAttackType != PlayerController.AttackType.Light)
            {
                controller.transform.position = new Vector2(transform.position.x, controller.transform.position.y); // this might not work if the player is on a slope, so keep that in mind
            }
        }
    }

    void SetEnemiesAlerted()
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
    }
}
