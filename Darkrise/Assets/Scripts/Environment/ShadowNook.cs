using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowNook : InteractTrigger
{
    public bool playerHiding = false;
    public bool dashedInto = false;
    bool active = true;
    [SerializeField] ShadowNook leftHidingSpot;
    [SerializeField] ShadowNook rightHidingSpot;

    protected override void Start()
    {
        base.Start();
    }


    // Update is called once per frame
    void Update()
    {
        if(playerHiding) { indicator.SetActive(false); }
    }
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dashedInto)
        {
            indicator.SetActive(false);
            playerHiding = false;
            controller.pState.hiding = false;
            controller.currentNook = null;
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
            controller.currentNook = this;
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
                HidePlayer();
            }
        }
    }

    public void HidePlayer()
    {
        indicator.SetActive(false);
        controller.currentNook = this;
        playerHiding = true;
        controller.pState.hiding = true;
        if (controller.currentAttackType != PlayerController.AttackType.Light && playerHiding)
        {
            controller.transform.position = new Vector2(transform.position.x, controller.transform.position.y); // this might not work if the player is on a slope, so keep that in mind
        }
    }

    public ShadowNook GetNextNook(float movementDirection)
    {
        if(Mathf.Sign(movementDirection) == 1) { return rightHidingSpot; }
        if(Mathf.Sign(movementDirection) == -1) { return leftHidingSpot; }

        return null;
    }
}
