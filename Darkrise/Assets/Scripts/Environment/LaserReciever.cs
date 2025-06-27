using System;
using System.Collections.Generic;
using UnityEngine;

public class LaserReciever : MonoBehaviour, IDataPersistence
{
    // Start is called before the first frame update
    [SerializeField] List<GameObject> objectsToPower;
    SpriteRenderer sr;
    bool powered = false;
    [SerializeField] private string id;
    [ContextMenu("Generate new GUID")]

    private void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (powered)
        {
            sr.color = Color.blue;
            PowerObjects();
        }
    }

    public void ReceiveLaser()
    {
        powered = true;
    }

    public void SaveData(GameData data)
    {
        if (data.laserRecieverStatus.ContainsKey(id))
        {
            data.laserRecieverStatus.Remove(id);
        }
        data.laserRecieverStatus.Add(id, powered);
    }

    public void LoadData(GameData data)
    {
        data.laserRecieverStatus.TryGetValue(id, out powered);
    }

    void PowerObjects()
    {
        foreach (GameObject obj in objectsToPower)
        {
            obj.SetActive(true); // this can be expanded later 
        }
    }

}
