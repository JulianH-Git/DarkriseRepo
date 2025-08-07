using UnityEngine;

public class ForcedEncounterBreakerSwitch : InteractTrigger
{
    public bool deactivated;
    public bool flashbanged;
    bool disableAnim = false;
    Animator animator;
    BoxCollider2D trigger;
    [SerializeField] float flashbangDeactivationTimer;
    float timeTilReactivate;

    protected override void Start()
    {
        base.Start();
        animator = this.GetComponent<Animator>();
        trigger = this.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (deactivated == true) { trigger.enabled = false; }
        if (flashbanged == true) { FlashbangDeactivation(); }
        if(disableAnim) { animator.SetBool("turnedOff", true); trigger.enabled = false; }
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
                animator.SetBool("turnedOff", true);

                CutsceneTrigger cutsceneTrigger = GetComponent<CutsceneTrigger>();
                if (cutsceneTrigger != null)
                {
                    cutsceneTrigger.StartCutscene();
                }
            }
        }
    }

    public void FlashbangDeactivation()
    {
        deactivated = true;
        animator.SetBool("turnedOff", true);

        timeTilReactivate += Time.deltaTime;

        if(timeTilReactivate >=  flashbangDeactivationTimer)
        {
            deactivated = false;
            flashbanged = false;
            animator.SetBool("turnedOff", true);
            trigger.enabled = true;
        }

    }

    public void DisableBreaker()
    {
        deactivated = true;
        disableAnim = true;
    }
}
