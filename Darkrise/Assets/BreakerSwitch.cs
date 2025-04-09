using System.Collections.Generic;
using UnityEngine;

public class BreakerSwitch : InteractTrigger
{
    [SerializeField] List<GameObject> encounterRoomLocks = new List<GameObject>();
    [SerializeField] List<GameObject> extraEnemySpawners = new List<GameObject>();
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            indicateColor.color = Color.green;
            indicator.SetActive(true);

            if (controller.Interact())
            {
                DeactivateForcedEncounter();
            }
        }
    }

    void DeactivateForcedEncounter()
    {
        foreach(GameObject obj in encounterRoomLocks)
        {
            if (obj.GetComponent<ForcedEncounterRoomLock>() != null)
            {
                obj.GetComponent<ForcedEncounterRoomLock>().RemoveLock();
            }
        }

        foreach (GameObject esm in extraEnemySpawners)
        {
            if (esm.GetComponent<EnemySpawnerManager>() != null)
            {
                esm.GetComponent<EnemySpawnerManager>().KillEnemy();
            }
        }

        this.gameObject.SetActive(false);
    }
}
