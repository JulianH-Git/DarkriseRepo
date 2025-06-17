using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    [SerializeField] float alarmTimer;
    float timeToAlarm;
    [SerializeField] FuseBox fb;
    [SerializeField] Transform enemyAlertTransform;
    [SerializeField] Vector2 enemyAlertRadius;
    [SerializeField] LayerMask enemyLayer;
    SpriteRenderer sr;
    [SerializeField] float flashbangDeactivationTimer;
    float timeTilReactivated;
    public bool flashbanged;
     
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(flashbanged)
        {
            sr.color = Color.grey;
            Flashbanged();
            return;
        }
        if (fb != null && fb.powered && !fb.overloaded)
        {
            sr.color = Color.red;
            timeToAlarm += Time.deltaTime;
            if(timeToAlarm >= alarmTimer)
            {
                timeToAlarm = 0;
                List<Collider2D> enemiesInRange = CheckForEnemies(enemyAlertTransform, enemyAlertRadius);
                if (enemiesInRange != null && enemiesInRange.Count > 0)
                {
                    SignalEnemies(enemiesInRange);
                }
            }
        }
        if (fb != null && fb.overloaded)
        {
            sr.color = Color.grey;
        }
    }

    List<Collider2D> CheckForEnemies(Transform _roomTransform, Vector2 _roomArea)
    {
        Collider2D[] ObjectsToHit = Physics2D.OverlapBoxAll(_roomTransform.position, _roomArea, 0, enemyLayer);
        List<Collider2D> enemiesInRange = new List<Collider2D>();

        for (int i = 0; i < ObjectsToHit.Length; i++)
        {
            if (ObjectsToHit[i].GetComponent<enemyBase>() != null)
            {
                enemiesInRange.Add(ObjectsToHit[i]);
            }
        }
        return enemiesInRange;
    }

    void SignalEnemies(List<Collider2D> enemies)
    {
        foreach (Collider2D obj in enemies)
        {
            if (obj.GetComponent<FootSolider>() != null)
            {
                Collider2D playerInRange = obj.GetComponent<FootSolider>().PlayerCheck();
                if (playerInRange != null && !obj.GetComponent<FootSolider>().aggressive)
                {
                    obj.GetComponent<FootSolider>().Alerted(playerInRange, enemyAlertTransform.position);
                }
            }
            obj.GetComponent<enemyBase>().Alerted(enemyAlertTransform.position);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(enemyAlertTransform.position, enemyAlertRadius);
    }

    void Flashbanged()
    {
        timeTilReactivated += Time.deltaTime;

        if (timeTilReactivated >= flashbangDeactivationTimer)
        {
            flashbanged = false;
        }
    }
}
