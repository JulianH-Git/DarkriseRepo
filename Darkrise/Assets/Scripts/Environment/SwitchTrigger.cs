using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour, IDataPersistence
{
    [SerializeField] protected GameObject player;
    protected PlayerController controller;

    [SerializeField] protected List<GameObject> affectedSprites;

    [SerializeField] protected CutsceneCamera cutsceneManager;

    private bool hasBeenUsed = false;

    [SerializeField] protected SpriteRenderer indicateColor;

    [SerializeField] protected GameObject indicator;

    private float duration = 0.1f;

    [SerializeField] private string id;
    [ContextMenu("Generate new GUID")]

    private void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void SaveData(GameData data)
    {
        if (data.switchTriggerStatus.ContainsKey(id))
        {
            data.switchTriggerStatus.Remove(id);
        }
        data.switchTriggerStatus.Add(id, hasBeenUsed);
    }

    public void LoadData(GameData data)
    {
        data.switchTriggerStatus.TryGetValue(id, out hasBeenUsed);
        if(hasBeenUsed)
        {
            SwitchActivated(false);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        controller = player.GetComponent<PlayerController>();
        indicator.SetActive(false);
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !hasBeenUsed)
        {
            indicateColor.color = Color.green;
            indicator.SetActive(true);

            if (controller.Interact())
            {
                SwitchActivated();
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

    private void SwitchActivated(bool playCutscene = true)
    {
        indicator.SetActive(false);
        foreach (GameObject sprite in affectedSprites)
        {
            hasBeenUsed = true;
            indicateColor.color = Color.white;
            if(playCutscene) { GetComponent<CutsceneTrigger>()?.StartCutscene(); }
            StartCoroutine(MoveGates(sprite, new Vector2(sprite.transform.localScale.x, 0)));
        }
        if(playCutscene) 
        { 
            AudioManager.instance.PlayOneShot(FMODEvents.instance.pullLever, this.transform.position); 
        }
        this.GetComponent<SpriteRenderer>().flipY = true;
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
