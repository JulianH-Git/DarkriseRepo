using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwitchStatue : InteractTrigger
{
    enum ModeGiven
    {
        Neutral,
        Dark,
        Light
    }
    [SerializeField] ModeGiven modeGiven;
    [SerializeField] bool oneTimeUse;
    bool used;
    Color statueColor;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        switch(modeGiven)
        {
            case ModeGiven.Neutral:
                break;
            case ModeGiven.Dark:
                ColorUtility.TryParseHtmlString("#6F2828", out statueColor);
                indicateColor.color = statueColor;
                break;
            case ModeGiven.Light:
                ColorUtility.TryParseHtmlString("#FFFFBE", out statueColor);
                indicateColor.color = statueColor;
                break;
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

                switch (modeGiven)
                {
                    case ModeGiven.Neutral:
                        controller.StatueModeChange(PlayerController.AttackType.Neutral);
                        break;
                    case ModeGiven.Dark:
                        controller.StatueModeChange(PlayerController.AttackType.Dark);
                        break;
                    case ModeGiven.Light:
                        controller.StatueModeChange(PlayerController.AttackType.Light);
                        break;
                }
                controller.modeLocked = true;

                if (oneTimeUse) { used = true; }
                if(this.GetComponent<ActivateOtherObjects>() != null) { this.GetComponent<ActivateOtherObjects>().Activate(); }
                
            }
        }
    }
}
