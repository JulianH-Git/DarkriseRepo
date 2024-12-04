using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpotlightStates 
{
    Off,
    Red,
    Yellow
}

public class SpotlightPrefab : MonoBehaviour
{
    [SerializeField]
    GameObject lamp;
    [SerializeField]
    List<Sprite> lampSprites;
    [SerializeField]
    List<Sprite> spotlightSprites;

    public SpotlightStates state;

    [SerializeField]
    private PlayerStateList playerState;
    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    List<GameObject> gates = new List<GameObject>();

    private List<Vector2> originalSizes = new List<Vector2>();

    [SerializeField]
    List<Vector2> spottedSizes = new List<Vector2>();


    // player spotted
    private bool playerSpotted = false;
    public bool PlayerSpotted { get => playerSpotted; }

    // cooldown
    [SerializeField]
    private float cooldown = 2f; // 2 seconds
    private float cooldownTimer = 0f;

    private float duration = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < gates.Count; i++)
        {
            originalSizes.Add(gates[i].transform.localScale);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (!playerState.dashing)
            {
                playerSpotted = true;
                cooldownTimer = cooldown;
                if(state == SpotlightStates.Red) 
                {
                    for (int i = 0; i < gates.Count; i++)
                    {
                        StartCoroutine(MoveGates(gates[i], spottedSizes[i]));
                    }
                }
                else if(state == SpotlightStates.Yellow) 
                {
                    controller.TakeDamage(1);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) 
        {
            case SpotlightStates.Off:
                lamp.GetComponent<SpriteRenderer>().sprite = lampSprites[0];
                this.GetComponent<BoxCollider2D>().enabled = false;
                this.GetComponent<SpriteRenderer>().enabled = false;
                break;

            case SpotlightStates.Red:
                lamp.GetComponent<SpriteRenderer>().sprite = lampSprites[1];
                this.GetComponent<BoxCollider2D>().enabled = true;
                this.GetComponent<SpriteRenderer>().enabled = true;
                this.GetComponent<SpriteRenderer>().sprite = spotlightSprites[0];
                break;

            case SpotlightStates.Yellow:
                lamp.GetComponent<SpriteRenderer>().sprite = lampSprites[2];
                this.GetComponent<BoxCollider2D>().enabled = true;
                this.GetComponent<SpriteRenderer>().enabled = true;
                this.GetComponent<SpriteRenderer>().sprite = spotlightSprites[1];
                break;
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            playerSpotted = false;
            if(state == SpotlightStates.Red) 
            {
                for (int i = 0; i < gates.Count; i++)
                {
                    StartCoroutine(RevertGates(gates[i], originalSizes[i]));
                }
            }
        }
    }

    private IEnumerator MoveGates(GameObject gate, Vector2 spotSize)
    {
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
        }
    }

    private IEnumerator RevertGates(GameObject gate, Vector2 originalSize)
    {
        Vector2 initialScale = gate.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (gate != null) // Ensure the object is still valid
            {
                gate.transform.localScale = Vector2.Lerp(initialScale, originalSize, elapsedTime / duration);
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
            gate.transform.localScale = originalSize; // Ensure exact final size
        }
    }
}
