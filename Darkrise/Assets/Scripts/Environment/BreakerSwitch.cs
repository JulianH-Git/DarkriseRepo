using UnityEngine;

public class BreakerSwitch : InteractTrigger
{
    public bool deactivated;

    [Header("Music change")]
    [SerializeField] private MusicArea area;
    Animator animator;
    BoxCollider2D trigger;

    protected override void Start()
    {
        base.Start();
        animator = this.GetComponent<Animator>();
        trigger = this.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (deactivated == true) { trigger.enabled = false; }
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
            }
        }
    }
}
