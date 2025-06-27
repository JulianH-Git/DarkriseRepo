using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class DashUpgrade : MonoBehaviour, IDataPersistence
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private ForcedEncounterManager FEM;
    [SerializeField] List<GameObject> onboarding = new List<GameObject>();
    [SerializeField] private string id;
    bool activated = false;
    [ContextMenu("Generate new GUID")]

    private void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void SaveData(GameData data)
    {
        if (data.upgradeStatus.ContainsKey(id))
        {
            data.upgradeStatus.Remove(id);
        }
        data.upgradeStatus.Add(id, activated);
    }

    public void LoadData(GameData data)
    {
        data.upgradeStatus.TryGetValue(id, out activated);
        if (activated)
        {
            ActivateDash();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            ActivateDash();
        }
    }

    public void ActivateDash()
    {
        controller.canDash = true;
        FEM.ActivateForcedEncounterTutorial();

        foreach (GameObject instruct in onboarding)
        {
            instruct.SetActive(true);
        }
        activated = true;
        this.gameObject.SetActive(false);
    }
}
