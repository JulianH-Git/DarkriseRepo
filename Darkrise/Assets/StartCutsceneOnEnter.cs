using Rewired;
using UnityEngine;

public class StartCutsceneOnEnter : MonoBehaviour
{
    private bool hasBeenUsed = false;
    [SerializeField] private string id;
    [ContextMenu("Generate new GUID")]

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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !hasBeenUsed)
        {
            hasBeenUsed = true;
            ActivateCutscene();
        }
    }

    void ActivateCutscene(bool playCutscene = true)
    {
        if (playCutscene) { GetComponent<CutsceneTrigger>()?.StartCutscene(); }
        if (GetComponent<ActivateCutsceneObjects>() != null) { GetComponent<ActivateCutsceneObjects>().StartActivate(); }
    }

}
