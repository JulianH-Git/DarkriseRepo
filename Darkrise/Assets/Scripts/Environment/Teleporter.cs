using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : InteractTrigger
{
    [SerializeField] protected GameObject teleportPoint;
    protected override void TriggerActivated()
    {
        indicateColor.color = Color.green;
        indicator.SetActive(true);

        if (controller.Interact())
        {
            Vector2 point = new Vector2(teleportPoint.transform.position.x, teleportPoint.transform.position.y);

            player.transform.localPosition = point;
        }
    }
}
