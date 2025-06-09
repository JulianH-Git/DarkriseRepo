using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    [SerializeField] protected GameObject player;
    protected PlayerController controller;

    [SerializeField] protected List<GameObject> affectedSprites;

    [SerializeField] protected CutsceneCamera cutsceneManager;

    private bool hasBeenUsed = false;

    [SerializeField] protected SpriteRenderer indicateColor;

    [SerializeField] protected GameObject indicator;

    private float duration = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        controller = player.GetComponent<PlayerController>();
        indicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !hasBeenUsed)
        {
            indicateColor.color = Color.green;
            indicator.SetActive(true);

            if (controller.Interact())
            {
                indicator.SetActive(false);
                foreach (GameObject sprite in affectedSprites)
                {
                    hasBeenUsed = true;
                    indicateColor.color = Color.white;
                    cutsceneManager.PlayCutscene();
                    StartCoroutine(MoveGates(sprite, new Vector2(sprite.transform.localScale.x, 0)));
                }
                AudioManager.instance.PlayOneShot(FMODEvents.instance.pullLever, this.transform.position);
                this.GetComponent<SpriteRenderer>().flipY = true;
            }
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            indicator.SetActive(false);
            indicateColor.color = Color.white;
        }
    }

    private IEnumerator MoveGates(GameObject gate, Vector2 spotSize)
    {
        yield return new WaitForSeconds(1);

        Vector2 initialScale = gate.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (gate != null) // Ensure the object is still valid
            {
                gate.transform.localScale = Vector2.Lerp(initialScale, spotSize, elapsedTime / duration);
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
