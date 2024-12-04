using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField] Transform roomTransform;
    [SerializeField] GameObject enemy;
    [SerializeField] Vector2 roomArea;
    [SerializeField] LayerMask layer;
    GameObject spawnedEnemyRef;
    enemyBase spawnedEnemyRefMethods;

    bool enemySpawned = false;
    float respawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        spawnedEnemyRef = Instantiate(enemy, roomTransform.position, Quaternion.identity);
        enemySpawned = true;
        spawnedEnemyRefMethods = spawnedEnemyRef.GetComponent<enemyBase>();
        respawnTimer = 5f;
    }

    // Update is called once per frame
    bool timerReset = false; 

void Update()
{
    if (CheckForPlayer(roomTransform, roomArea) == false && enemySpawned == false)
    {
        if (respawnTimer <= 0)
        {
            spawnedEnemyRefMethods.Respawn();
            enemySpawned = true;
            timerReset = false;
        }
        else
        {
            respawnTimer -= Time.deltaTime;
        }
    }

    if (spawnedEnemyRef != null && spawnedEnemyRef.activeSelf == false)
    {
        enemySpawned = false;

        if (!timerReset)
        {
            respawnTimer = 5f;
            timerReset = true; 
        }
    }
}

    bool CheckForPlayer(Transform _roomTransform, Vector2 _roomArea)
    {
        Collider2D[] ObjectsToHit = Physics2D.OverlapBoxAll(_roomTransform.position, _roomArea, 0, layer);

        foreach (var obj in ObjectsToHit)
        {
            if (obj.CompareTag("Player"))
            {
                return true; 
            }
        }

        return false;
    }

    void OnDrawGizmos() // comment this out when we're done placing things to keep everything visible
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(roomTransform.position, roomArea);
    }
}
