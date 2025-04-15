using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ForcedEncounterManager : MonoBehaviour
{
    [SerializeField] BreakerSwitch breaker;
    [SerializeField] List<GameObject> spotlights = new List<GameObject>();
    [SerializeField] List<GameObject> forcedEncounterWalls = new List<GameObject>();
    [SerializeField] List<GameObject> extraEnemySpawns = new List<GameObject>();
    [SerializeField] List<GameObject> permanentEnemySpawns = new List<GameObject>();

    [Header("Audio Settings")]
    [SerializeField] private MusicArea area;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(breaker.deactivated)
        {
            DeactivateForcedEncounter();
        }
        foreach(GameObject obj in spotlights)
        {
            if (obj.GetComponent<SpotlightPrefab>() != null && obj.GetComponent<SpotlightPrefab>().startEncounter == true)
            {
                ActivateForcedEncounter();
                break;
            }
        }
    }

    void ActivateForcedEncounter()
    {
        breaker.gameObject.SetActive(true);
        ActivateWalls();
        ActivateSpotlights();
        ActivateSpawners();
    }

    public void ActivateForcedEncounterRoom1()
    {
        breaker.gameObject.SetActive(true);
        foreach (GameObject psm in permanentEnemySpawns)
        {
            psm.SetActive(true);
        }
        ActivateWalls();
        ActivateSpotlights();
    }

    void DeactivateForcedEncounter()
    {
        area = MusicArea.DarkArea;
        AudioManager.instance.SetMusicArea(area);

        foreach (GameObject obj in forcedEncounterWalls)
        {
            if (obj.activeSelf == true && obj.GetComponent<ForcedEncounterRoomLock>() != null)
            {
                obj.GetComponent<ForcedEncounterRoomLock>().RemoveLock();
            }
        }

        foreach (GameObject esm in extraEnemySpawns)
        {
            if (esm.activeSelf == true && esm.GetComponent<EnemySpawnerManager>() != null)
            {
                esm.GetComponent<EnemySpawnerManager>().KillEnemy();
            }
        }

        foreach (GameObject spot in spotlights)
        {
            if(spot.activeSelf == true)
            {
                spot.GetComponent<SpotlightPrefab>().startEncounter = false;
                spot.GetComponent<SpotlightPrefab>().state = SpotlightStates.Off;
            }
        }
    }

    void ActivateWalls()
    {
        foreach (GameObject obj in forcedEncounterWalls)
        {
            obj.SetActive(true);
        }
    }

    void ActivateSpawners()
    {
        foreach (GameObject esm in extraEnemySpawns)
        {
            esm.SetActive(true);
        }

        foreach(GameObject psm in permanentEnemySpawns)
        {
            psm.SetActive(true);
        }
    }

    void ActivateSpotlights()
    {
        foreach(GameObject spot in spotlights)
        {
            spot.GetComponent<SpotlightPrefab>().state = SpotlightStates.ForcedEncounter;
        }
    }
}
