using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueInteraction : InteractTrigger
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            indicator.SetActive(true);
            indicateColor.color = Color.green;

            if (controller.Interact())
            {
                controller.StatueRecharge();
            }
        }
    }
}
