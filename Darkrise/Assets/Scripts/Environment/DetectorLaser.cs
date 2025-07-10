using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DetectorLaser : MonoBehaviour
{
    public DetectorLaserState state;
    [SerializeField] float pushForce = 100f;
    [SerializeField] Light2D lampLight;
    [SerializeField] Light2D detectorLight;
    ParticleSystem spotlightParticles;
    ParticleSystem.EmissionModule emission;
    SpriteRenderer sr;
    PlayerController controller;
    bool hurtPlayer = true;
    PushPlayer pushPlayer = PushPlayer.NoPush;
    PushPlayer initialEntry = PushPlayer.NoPush;

    [Header("Detect Settings")]
    [SerializeField] float downedCooldown;
    float downedCooldownTimer;

    [Header("Explicit Settings")]
    [SerializeField] public bool turnedOn;


    public enum DetectorLaserState
    {
        Detect,
        Explicit
    }
    public enum PushPlayer
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

        spotlightParticles = GetComponentInChildren<ParticleSystem>();
        emission = spotlightParticles.emission;
    }

    // Update is called once per frame
    void Update()
    {
        downedCooldownTimer += Time.deltaTime;
        Debug.Log(downedCooldownTimer);

        if(state == DetectorLaserState.Detect || state == DetectorLaserState.Explicit && turnedOn)
        {
            if (downedCooldownTimer >= downedCooldown)
            {
                TurnOn();
            }

            if (pushPlayer != PushPlayer.NoPush && hurtPlayer)
            {
                float force = pushPlayer == PushPlayer.PushLeft ? -pushForce : pushForce;
                controller.AddXForce(force);
            }
        }
        else
        {
            TurnOff();
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
        if (state == DetectorLaserState.Detect && collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            TurnOff();
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

    void TurnOff()
    {
        sr.color = new Color(0.512991f, 0f, 1f, 0.1f);
        downedCooldownTimer = 0f;
        hurtPlayer = false;
        pushPlayer = PushPlayer.NoPush;
        detectorLight.enabled = false;
        lampLight.enabled = false;
        emission.enabled = false;
    }

    void TurnOn()
    {
        sr.color = new Color(0.512991f, 0f, 1f, 1f);
        hurtPlayer = true;
        pushPlayer = initialEntry;
        detectorLight.enabled = true;
        lampLight.enabled = true;
        emission.enabled = true;
    }
}
