using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class StandardBreakerSwitch : InteractTrigger, IDataPersistence
{
    public bool deactivated;
    public bool flashbanged;
    Animator animator;
    BoxCollider2D trigger;
    [SerializeField] float flashbangDeactivationTimer;
    float timeTilReactivate;
    [SerializeField] FuseBox fb;
    private bool hasBeenCalled = false;
    [SerializeField] protected List<GameObject> affectedSprites;
    [SerializeField] private string id;
    [ContextMenu("Generate new GUID")]

    private void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }


    public void SaveData(GameData data)
    {
        if (data.fbStatus.ContainsKey(id))
        {
            data.fbStatus.Remove(id);
        }
        data.fbStatus.Add(id, deactivated);
    }

    public void LoadData(GameData data)
    {
        data.fbStatus.TryGetValue(id, out deactivated);
    }

    protected override void Start()
    {
        base.Start();
        animator = this.GetComponent<Animator>();
        trigger = this.GetComponent<BoxCollider2D>();
        hasBeenCalled = false;
    }

    private void Update()
    {
        if (deactivated == true) { trigger.enabled = false; }
        if (flashbanged == true || (fb != null && fb.flashbanged)) { FlashbangDeactivation(); }
        if (fb != null && fb.overloaded && !fb.flashbanged)
        {
            deactivated = true;
            animator.SetBool("turnedOff", true);

            FlipAffectedSprites();
        }
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
                ActivateCutscene();

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

        FlipAffectedSprites();

        timeTilReactivate += Time.deltaTime;

        if (timeTilReactivate >= flashbangDeactivationTimer)
        {
            deactivated = false;
            flashbanged = false;
            animator.SetBool("turnedOff", true);
            trigger.enabled = true;

            foreach (GameObject sprite in affectedSprites)
            {
                sprite.SetActive(!sprite.activeSelf);
            }
            hasBeenCalled = false;
        }

    }

    public void FlipAffectedSprites() 
    {
        if (!hasBeenCalled)
        {
            foreach (GameObject sprite in affectedSprites)
            {
                sprite.SetActive(!sprite.activeSelf);
                hasBeenCalled = true;
            }
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

    void ActivateCutscene(bool playCutscene = true)
    {
        if (playCutscene) { GetComponent<CutsceneTrigger>()?.StartCutscene(); }
        if (GetComponent<ActivateCutsceneObjects>() != null) { GetComponent<ActivateCutsceneObjects>().StartActivate(); }
    }
}
