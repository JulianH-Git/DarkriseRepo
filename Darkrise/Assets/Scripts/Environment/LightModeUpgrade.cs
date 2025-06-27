using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightModeUpgrade : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    List<GameObject> onboarding = new List<GameObject>();
    bool activated = false;

    [SerializeField] private string id;
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
            ActivateLightMode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            ActivateLightMode();
        }
    }

    private void ActivateLightMode()
    {
        controller.lightUnlocked = true;
        controller.onEnergyChangedCallback.Invoke();

        foreach (GameObject insturct in onboarding)
        {
            insturct.SetActive(true);
        }

        activated = true;
        this.gameObject.SetActive(false);
    }
}
