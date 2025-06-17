using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOtherObjects : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<GameObject> objectsToActivate = new List<GameObject>();

    public void Activate()
    {
        foreach(GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }

    }
}
