using Rewired;
using UnityEngine;

public class StartCutsceneOnEnter : MonoBehaviour, IDataPersistence
{
    private bool hasBeenUsed = false;
    float cutsceneLength;
    float timeTilCutsceneIsOver;
    bool runOnce = false;
    [SerializeField] int cutsceneNumber;
    [SerializeField] private string id;
    [ContextMenu("Generate new GUID")]
    private void Start()
    {
        cutsceneLength = GetComponent<CutsceneTrigger>().cutsceneDuration;
    }
    private void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void SaveData(GameData data)
    {
        if (data.cutsceneTriggerStatus.ContainsKey(id))
        {
            data.cutsceneTriggerStatus.Remove(id);
        }
        data.cutsceneTriggerStatus.Add(id, hasBeenUsed);
    }

    public void LoadData(GameData data)
    {
        data.cutsceneTriggerStatus.TryGetValue(id, out hasBeenUsed);
        if(hasBeenUsed)
        {
            ActivateCutscene(false);
        }
    }

    private void Update()
    {
        if (hasBeenUsed && !runOnce && timeTilCutsceneIsOver >= cutsceneLength)
        {
            this.gameObject.SetActive(false);
            runOnce = true;
        }
        else if(hasBeenUsed && !(timeTilCutsceneIsOver >= cutsceneLength))
        {
            timeTilCutsceneIsOver += Time.deltaTime;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !hasBeenUsed)
        {
            hasBeenUsed = true;
            ActivateCutscene(true);
        }
    }

    void ActivateCutscene(bool playCutscene = true)
    {
        if (playCutscene) { GetComponent<CutsceneTrigger>()?.StartCutscene(); }
        if (GetComponent<ActivateCutsceneObjects>() != null) 
        {
            if(cutsceneNumber == 5)
            {
                GetComponent<ActivateCutsceneObjects>().TimelineActivate(playCutscene);
            }
            else
            {
                //Debug.Log(cutsceneNumber);
                GetComponent<ActivateCutsceneObjects>().StartActivate();
            }
        }
    }

}
