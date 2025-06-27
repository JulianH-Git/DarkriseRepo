using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkModeUpgrade : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    List<GameObject> onboarding = new List<GameObject>();
    [SerializeField] private string id;
    [ContextMenu("Generate new GUID")]

    private void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }
    bool activated = false;

    public void SaveData(GameData data)
    {
        if(data.upgradeStatus.ContainsKey(id))
        {
            data.upgradeStatus.Remove(id);
        }
        data.upgradeStatus.Add(id, activated);
    }

    public void LoadData(GameData data)
    {
        data.upgradeStatus.TryGetValue(id, out activated);
        if(activated)
        {
            ActivateDarkMode();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            ActivateDarkMode();
        }
    }

    public void ActivateDarkMode()
    {
        controller.darkUnlocked = true;
        controller.onEnergyChangedCallback.Invoke();

        foreach (GameObject insturct in onboarding)
        {
            insturct.SetActive(true);
        }
        activated = true;
        this.gameObject.SetActive(false);
    }
}
