using System.Collections.Generic;
using UnityEngine;

public class LaserReciever : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<GameObject> objectsToPower;
    SpriteRenderer sr;
    bool powered = false;

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



    void PowerObjects()
    {
        foreach (GameObject obj in objectsToPower)
        {
            obj.SetActive(true); // this can be expanded later 
        }
    }

}
