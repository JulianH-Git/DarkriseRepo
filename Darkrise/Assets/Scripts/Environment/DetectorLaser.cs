using UnityEngine;

public class DetectorLaser : MonoBehaviour
{
    PlayerController controller;
    [SerializeField] float downedCooldown;
    SpriteRenderer sr;
    float downedCooldownTimer;
    bool hurtPlayer = true;
    PushPlayer pushPlayer = PushPlayer.NoPush;
    PushPlayer initialEntry = PushPlayer.NoPush;

    enum PushPlayer
    {
        PushLeft,
        PushRight,
        NoPush
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = PlayerController.Instance;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        downedCooldownTimer += Time.deltaTime;
        Debug.Log(downedCooldownTimer);
        if(downedCooldownTimer >= downedCooldown)
        {
            sr.color = new Color(0.512991f, 0f, 1f, 1f);
            hurtPlayer = true;
            pushPlayer = initialEntry;
        }
        
        switch (pushPlayer)
        {
            case PushPlayer.PushLeft:
                controller.AddXForce(-100f);
                break;
            case PushPlayer.PushRight:
                controller.AddXForce(100f);
                break;
            case PushPlayer.NoPush:
                break;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if ((this.transform.position.x - collision.transform.position.x) > 0)
            {
                pushPlayer = PushPlayer.PushLeft;
                initialEntry = PushPlayer.PushLeft;
            }
            else if ((this.transform.position.x - collision.transform.position.x) < 0)
            {
                pushPlayer = PushPlayer.PushRight;
                initialEntry = PushPlayer.PushRight;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (hurtPlayer)
            {
                if (!controller.pState.invincible) { controller.TakeDamage(1); }
            }
        }
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            sr.color = new Color(0.512991f, 0f, 1f, 0.1f);
            downedCooldownTimer = 0f;
            hurtPlayer = false;
            pushPlayer = PushPlayer.NoPush;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            pushPlayer = PushPlayer.NoPush;
            initialEntry = PushPlayer.NoPush;
            hurtPlayer = true;
        }

    }
}
