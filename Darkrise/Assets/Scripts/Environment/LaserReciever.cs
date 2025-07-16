using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class LaserReciever : MonoBehaviour, IDataPersistence
{
    public enum LaserRecieverBehavior
    {
        PoweredToActivate,
        OffToActivate
    }
    // Start is called before the first frame update
    [SerializeField] LaserRecieverBehavior behavior;
    [SerializeField] List<GameObject> objectsToPower;
    SpriteRenderer sr;
    bool powered = false;
    bool active = true;
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
        if (powered && behavior == LaserRecieverBehavior.PoweredToActivate)
        {
            sr.color = Color.blue;
            PowerObjects();
        }
        else if(!active && behavior == LaserRecieverBehavior.OffToActivate)
        {
            sr.color = Color.blue;
            PowerObjects();
        }
        active = false;
    }

    public void ReceiveLaser()
    {
        if (behavior == LaserRecieverBehavior.PoweredToActivate) { powered = true; }
        active = true;
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
            if(obj.GetComponent<DarkRoom>() != null) { obj.SetActive(false); }
            else { StartCoroutine(MoveGates(obj, new Vector2(obj.transform.localScale.x, 0))); } 
        }
    }

    private IEnumerator MoveGates(GameObject gate, Vector2 spotSize)
    {
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
}
