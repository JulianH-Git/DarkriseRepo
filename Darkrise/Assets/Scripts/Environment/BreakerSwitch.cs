using System.Collections.Generic;
using UnityEngine;

public class BreakerSwitch : InteractTrigger
{
    public bool deactivated;

    [Header("Music change")]
    [SerializeField] private MusicArea area;

    protected override void Start()
    {
        base.Start();
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            indicateColor.color = Color.green;
            indicator.SetActive(true);

            if (controller.Interact())
            {
                deactivated = true;
                this.gameObject.SetActive(false);
            }
        }
    }
}
