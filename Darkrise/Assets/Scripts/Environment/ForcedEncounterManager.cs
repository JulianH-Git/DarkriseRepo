using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ForcedEncounterManager : MonoBehaviour
{
    [SerializeField] ForcedEncounterBreakerSwitch breaker;
    [SerializeField] List<GameObject> feSpotlights = new List<GameObject>();
    [SerializeField] List<GameObject> lasers = new List<GameObject>();
    [SerializeField] List<GameObject> forcedEncounterWalls = new List<GameObject>();
    [SerializeField] List<GameObject> extraEnemySpawns = new List<GameObject>();
    [SerializeField] List<GameObject> permanentEnemySpawns = new List<GameObject>();
    [SerializeField] ForcedEncounterManager chainNextEncounter;
    [SerializeField] private enemyBase enemy;
    [Header("Audio Settings")]
    [SerializeField] private bool deactivateOnce = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (breaker.deactivated)
        {
            DeactivateForcedEncounter();
            if (chainNextEncounter != null)
            {
                chainNextEncounter.ActivateForcedEncounterMidSecurity();
            }
        }
        foreach(GameObject obj in feSpotlights)
        {
            if (obj.GetComponent<SpotlightPrefab>() != null && obj.GetComponent<SpotlightPrefab>().startEncounter == true)
            {
                ActivateForcedEncounter();
                break;
            }
        }
    }

    public void ActivateForcedEncounter()
    {
        breaker.gameObject.SetActive(true);
        ActivateWalls();
        ActivateSpotlights();
        ActivateLasers();
        ActivateSpawners();
    }

    public void ActivateForcedEncounterMidSecurity()
    {
        breaker.gameObject.SetActive(true);
        foreach (GameObject psm in permanentEnemySpawns)
        {
            psm.SetActive(true);
        }
        ActivateSpotlights();
        ActivateLasers();
    }

    public void ActivateForcedEncounterTutorial()
    {
        breaker.gameObject.SetActive(true);
        foreach (GameObject psm in permanentEnemySpawns)
        {
            psm.SetActive(true);
        }
        ActivateSpotlights();
        ActivateLasers();
        ActivateWalls();
    }

    void DeactivateForcedEncounter()
    {
        if (!deactivateOnce)
        {
            
            AudioManager.instance.PlayOneShot(FMODEvents.instance.encounterPanel, this.transform.position);
            AudioManager.instance.SetMusicArea(MusicArea.DarkArea);
            deactivateOnce = true;
        }
            
        

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

        foreach (GameObject spot in feSpotlights)
        {
            if(spot.activeSelf == true)
            {
                spot.GetComponent<SpotlightPrefab>().startEncounter = false;
                spot.GetComponent<SpotlightPrefab>().state = SpotlightStates.Off;
            }
        }

        foreach (GameObject laser in lasers)
        {
            if (laser.activeSelf == true)
            {
                laser.GetComponent<SpotlightPrefab>().startEncounter = false;
                laser.GetComponent<SpotlightPrefab>().state = SpotlightStates.Off;
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
        foreach(GameObject spot in feSpotlights)
        {
            spot.GetComponent<SpotlightPrefab>().state = SpotlightStates.ForcedEncounter;
        }
    }

    void ActivateLasers()
    {
        foreach(GameObject laser in lasers)
        {
            laser.GetComponent<SpotlightPrefab>().state = SpotlightStates.Laser;
        }
    }
}
