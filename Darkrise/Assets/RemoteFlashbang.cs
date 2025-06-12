using Rewired.Data.Mapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteFlashbang : MonoBehaviour
{
    [SerializeField] Vector2 areaOfEffect;
    [SerializeField] GameObject areaOfEffectTransform;
    [SerializeField] LayerMask enemyLayer;
    PlayerController player;
    bool detonated;

    public bool Detonated
    {
        get { return detonated; }
        set { detonated = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // check if detonated
        if (Detonated)
        {
            ActivateFlashbang();
        }
    }

    private void FixedUpdate()
    {
        // move

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaOfEffectTransform.transform.position, areaOfEffect);
    }

    private void ActivateFlashbang()
    {
        // if so, check for enemies and stun them
        List<Collider2D> enemiesInRange = CheckForEnemies(areaOfEffectTransform.transform, areaOfEffect);
        if (enemiesInRange != null && enemiesInRange.Count > 0)
        {
            StunEnemies(enemiesInRange);
        }

        // and check for tech and disable it

        // then destroy myself
        Destroy(gameObject);
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

    void StunEnemies(List<Collider2D> enemies)
    {
        foreach (Collider2D obj in enemies)
        {
            obj.GetComponent<enemyBase>().EnemyHit(999f,transform.position,0.1f,false);
        }
    }

    void CheckForTech(Transform _roomTransform, Vector2 _roomArea)
    {
        Collider2D[] ObjectsToHit = Physics2D.OverlapBoxAll(_roomTransform.position, _roomArea, 0, 0);
        List<Collider2D> techInRange = new List<Collider2D>();

        for (int i = 0; i < ObjectsToHit.Length; i++)
        {
            if (ObjectsToHit[i].CompareTag("Technology"))
            {
                techInRange.Add(ObjectsToHit[i]);
            }
        }

        foreach(Collider2D obj in techInRange)
        {
            if(obj.GetComponent<BreakerSwitch>() != null)
            {
                obj.GetComponent<BreakerSwitch>().flashbanged = true;
            }
            if(obj.GetComponent<Alarm>() != null)
            {
                obj.GetComponent<Alarm>().flashbanged = true;
            }
            if(obj.GetComponent<FuseBox>() != null)
            {
                obj.GetComponent<FuseBox>().flashbanged = true;
            }
        }
    }
}
