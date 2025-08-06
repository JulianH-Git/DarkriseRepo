using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : InteractTrigger
{
    [SerializeField] protected GameObject teleportPoint;
    [SerializeField] private SpriteRenderer fade;
    [SerializeField] bool goingUp; // true = going up, false = going down
    private float fadeSpeed = 2f;
    private PlayerController playerController;

    protected override void TriggerActivated()
    {
        indicateColor.color = Color.green;
        indicator.SetActive(true);

        if (controller.Interact())
        {
            StartCoroutine(TeleportSequence());
        }
    }

    private IEnumerator TeleportSequence() 
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerController.CanMove = false;
        if (goingUp) // sound that plays when going up
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.goingUp, this.transform.position);
        }
        else // sound that plays when going down
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.goingDown, this.transform.position);
        }
        yield return StartCoroutine(Fade(0.0f, 1.0f));

        yield return new WaitForSeconds(0.3f);

        Vector2 point = new Vector2(teleportPoint.transform.position.x, teleportPoint.transform.position.y);

        player.transform.localPosition = point;

        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(Fade(1.0f, 0.0f));

        playerController.CanMove = true;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fade.color;

        while (Mathf.Abs(fade.color.a - endAlpha) > 0.01f)
        {
            elapsedTime += Time.deltaTime * fadeSpeed;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime);
            fade.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }

        fade.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
