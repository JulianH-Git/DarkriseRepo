using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ForcedEncounterManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] ForcedEncounterBreakerSwitch breaker;
    [SerializeField] bool turnSpotlightsToLasers;
    [SerializeField] List<GameObject> feSpotlights = new List<GameObject>();
    [SerializeField] List<GameObject> lasers = new List<GameObject>();
    [SerializeField] List<GameObject> forcedEncounterWalls = new List<GameObject>();
    [SerializeField] List<GameObject> extraEnemySpawns = new List<GameObject>();
    [SerializeField] List<GameObject> permanentEnemySpawns = new List<GameObject>();
    [SerializeField] ForcedEncounterManager chainNextEncounter;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] GameObject extraLasersParent;
    List<GameObject> extraLasers = new List<GameObject>();
    bool runOnce = false;
    [Header("Audio Settings")]
    [SerializeField] private bool deactivateOnce = false;

    [SerializeField] private string id;
    [ContextMenu("Generate new GUID")]

    private void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void SaveData(GameData data)
    {
        if (data.FEMStatus.ContainsKey(id))
        {
            data.FEMStatus.Remove(id);
        }
        data.FEMStatus.Add(id, deactivateOnce);
    }

    public void LoadData(GameData data)
    {
        data.FEMStatus.TryGetValue(id, out deactivateOnce);
        if(deactivateOnce)
        {
            DeactivateForcedEncounter();
        }
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
            if (obj.GetComponentInChildren<SpotlightPrefab>() != null && obj.GetComponentInChildren<SpotlightPrefab>().startEncounter == true && !runOnce)
            {
                runOnce = true;
                ActivateForcedEncounter();
                break;
            }
        }
    }

    public void ActivateForcedEncounter()
    {
        breaker.gameObject.SetActive(true);
        ActivateWalls();
        ActivateLasers();
        ActivateSpawners();
        if (turnSpotlightsToLasers) { SpotlightsToLasers(); }
        else { ActivateSpotlights(); }
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
                spot.GetComponentInChildren<SpotlightPrefab>().startEncounter = false;
                spot.GetComponentInChildren<SpotlightPrefab>().state = SpotlightStates.Off;
            }
        }

        foreach (GameObject laser in lasers)
        {
            if (laser.activeSelf == true)
            {
                laser.GetComponentInChildren<SpotlightPrefab>().startEncounter = false;
                laser.GetComponentInChildren<SpotlightPrefab>().state = SpotlightStates.Off;
            }
        }

        if(extraLasers.Count > 0)
        {
            foreach (GameObject laser in extraLasers)
            {
                if (laser.activeSelf == true)
                {
                    laser.GetComponentInChildren<SpotlightPrefab>().startEncounter = false;
                    laser.GetComponentInChildren<SpotlightPrefab>().state = SpotlightStates.Off;
                }
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
            psm.GetComponent<EnemySpawnerManager>().ChangeBehavior(FootSolider.SoldierBehavior.Patrol);
        }
    }

    void ActivateSpotlights()
    {
        foreach(GameObject spot in feSpotlights)
        {
            spot.GetComponentInChildren<SpotlightPrefab>().state = SpotlightStates.ForcedEncounter;
        }
    }

    void SpotlightsToLasers()
    {
        int loopCount = feSpotlights.Count;
        for(int i = 0; i < loopCount; i++)
        {
            feSpotlights[i].SetActive(false);
            GameObject newLaser = Instantiate(laserPrefab, new Vector3(feSpotlights[i].transform.position.x, feSpotlights[i].transform.position.y - 0.15f),Quaternion.identity,extraLasersParent.transform);
            extraLasers.Add(newLaser);
            newLaser.SetActive(true);
            newLaser.transform.localScale = new Vector3(0.21f, 0.24f);
        }
    }

    void ActivateLasers()
    {
        foreach(GameObject laser in lasers)
        {
            laser.GetComponentInChildren<SpotlightPrefab>().state = SpotlightStates.Laser;
        }
    }
}
