using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class StandardBreakerSwitch : InteractTrigger
{
    public bool deactivated;
    public bool flashbanged;
    Animator animator;
    BoxCollider2D trigger;
    [SerializeField] float flashbangDeactivationTimer;
    float timeTilReactivate;

    [SerializeField] protected List<GameObject> affectedSprites;

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

                foreach (GameObject sprite in affectedSprites)
                {
                    StartCoroutine(MoveGates(sprite, new Vector2(sprite.transform.localScale.x, 0)));
                }
            }
        }
    }

    public void FlashbangDeactivation()
    {
        deactivated = true;
        animator.SetBool("turnedOff", true);

        timeTilReactivate += Time.deltaTime;

        if (timeTilReactivate >= flashbangDeactivationTimer)
        {
            deactivated = false;
            flashbanged = false;
            animator.SetBool("turnedOff", true);
            trigger.enabled = true;
        }

    }

    private IEnumerator MoveGates(GameObject gate, Vector2 spotSize)
    {
        yield return new WaitForSeconds(1);

        Vector2 initialScale = gate.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < 0.1f)
        {
            if (gate != null) // Ensure the object is still valid
            {
                gate.transform.localScale = Vector2.Lerp(initialScale, spotSize, elapsedTime / 0.1f);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            else
            {
                yield break; // Exit the coroutine if the object is null
            }
        }

        if (gate != null)
        {
            gate.transform.localScale = spotSize; // Ensure exact final size
            gate.SetActive(false);
        }
    }
}
