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
    [SerializeField] Color gizmoColor;
    [SerializeField] float respawnTimer;
    [SerializeField] Vector2 enemySpawnOffset;
    [SerializeField] float patrolRadius;
    [SerializeField] float speed;
    [SerializeField] bool isFacingRight = true;
    GameObject spawnedEnemyRef;
    enemyBase spawnedEnemyRefMethods;

    bool enemySpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        spawnedEnemyRef = Instantiate(enemy, new Vector3(roomTransform.position.x + enemySpawnOffset.x,roomTransform.position.y + enemySpawnOffset.y,-1.0f), Quaternion.identity);
        spawnedEnemyRef.transform.parent = roomTransform;

        if (!isFacingRight) 
        {
            Vector3 scale = spawnedEnemyRef.transform.localScale;
            scale.x *= -1;
            spawnedEnemyRef.transform.localScale = scale;
        }

        enemySpawned = true;
        spawnedEnemyRefMethods = spawnedEnemyRef.GetComponent<enemyBase>();
        if(patrolRadius != 0)
        {
            spawnedEnemyRefMethods.patrolDistance = patrolRadius;
        }
        if (speed != 0)
        {
            spawnedEnemyRefMethods.speed = speed;
        }
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
            spawnedEnemyRefMethods.SetPosition(roomTransform.position);
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

        foreach (Collider2D obj in ObjectsToHit)
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
       
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(roomTransform.position, roomArea);
        Gizmos.DrawSphere(new Vector3(roomTransform.position.x + enemySpawnOffset.x, roomTransform.position.y + enemySpawnOffset.y, 0.0f), 0.1f);
        Vector3 patrolLine = new Vector3(roomTransform.position.x + enemySpawnOffset.x, roomTransform.position.y + enemySpawnOffset.y);
        Gizmos.DrawLine(new Vector3(patrolLine.x + patrolRadius, patrolLine.y), new Vector3(patrolLine.x - patrolRadius, patrolLine.y));
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Collider2D[] CheckForEnemy = Physics2D.OverlapBoxAll(roomTransform.position, roomArea, 0, layer);

        foreach (var obj in CheckForEnemy)
        {
            if (obj.CompareTag("Enemy"))
            {
                spawnedEnemyRefMethods.Retreat();
            }
        }
    }

    public void KillEnemy()
    {
        spawnedEnemyRefMethods.Die();
        this.gameObject.SetActive(false);
    }

    public void RespawnEnemy()
    {
        if (spawnedEnemyRefMethods != null)
        {
            spawnedEnemyRefMethods.Respawn();
            enemySpawned = true;
        }
    }

    public void ChangeBehavior(FootSolider.SoldierBehavior b)
    {
        if (spawnedEnemyRef.GetComponent<FootSolider>() != null)
        {
            spawnedEnemyRef.GetComponent<FootSolider>().behavior = b;
        }
    }

    public void CancelAlert()
    {
        spawnedEnemyRefMethods.CancelAlert();
    }

}
