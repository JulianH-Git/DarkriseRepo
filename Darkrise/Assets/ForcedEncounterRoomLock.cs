using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedEncounterRoomLock : MonoBehaviour
{
    public void RemoveLock()
    {
        this.gameObject.SetActive(false);
    }
}
