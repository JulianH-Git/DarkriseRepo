using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwitchStatue : InteractTrigger
{
    [SerializeField] bool giveLight;
    [SerializeField] bool giveDark;
    [SerializeField] bool oneTimeUse;
    bool used;
    Color statueColor;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if(giveLight)
        {
            ColorUtility.TryParseHtmlString("#FFFFBE", out statueColor);
            indicateColor.color = statueColor;
        }
        else if(giveDark)
        {
            ColorUtility.TryParseHtmlString("#6F2828", out statueColor);
            indicateColor.color = statueColor;
        }
    }
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            indicator.SetActive(false);
            indicateColor.color = statueColor;
        }
    }
    protected override void TriggerActivated()
    {
        if(!used)
        {
            indicator.SetActive(true);
            indicateColor.color = Color.green;

            if (controller.Interact())
            {
                indicator.SetActive(false);
                indicateColor.color = statueColor;

                if (giveLight)
                {
                    controller.StatueModeChange(PlayerController.AttackType.Light);
                }
                if (giveDark)
                {
                    controller.StatueModeChange(PlayerController.AttackType.Dark);
                }
                controller.modeLocked = true;

                if (oneTimeUse) { used = true; }
                
            }
        }
    }
}
