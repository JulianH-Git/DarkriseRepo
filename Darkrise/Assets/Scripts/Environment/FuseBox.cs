using Rewired.Utils.Classes.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : MonoBehaviour, IDataPersistence
{
    [SerializeField] float overloadTimer;
    [SerializeField] float flashbangDeactivationTimer;
    float timeTilReactivated;
    float timeToOverload;
    public bool powered;
    public bool overloaded;
    public bool flashbanged;
    SpriteRenderer sr;
    [SerializeField] private string id;
    [ContextMenu("Generate new GUID")]

    private void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void SaveData(GameData data)
    {
        if (data.fbStatus.ContainsKey(id))
        {
            data.fbStatus.Remove(id);
        }
        data.fbStatus.Add(id, powered);
    }

    public void LoadData(GameData data)
    {
        data.fbStatus.TryGetValue(id, out powered);
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(flashbanged)
        {
            sr.color = Color.red;
            Flashbanged();
        }
        if(powered && !overloaded)
        {
            sr.color = Color.yellow;
        }
        if(powered && overloaded)
        {
            sr.color = Color.red;
        }
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.isTrigger && !powered && PlayerController.Instance.currentAttackType == PlayerController.AttackType.Light)
        {
            powered = true;
            sr.color = Color.yellow;
            Debug.Log("Powered");
        }
        if(collision.CompareTag("Player") && collision.isTrigger && powered && PlayerController.Instance.BubbleUp)
        {
            timeToOverload = timeToOverload + Time.deltaTime;
            if(timeToOverload >= overloadTimer)
            {
                sr.color = Color.red;
                overloaded = true;
                Debug.Log("Overloaded");
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(powered && !overloaded)
            {
                timeToOverload -= Time.deltaTime;
            }
        }
    }

    protected void Flashbanged()
    {
        timeTilReactivated += Time.deltaTime;
        overloaded = true;

        if (timeTilReactivated >= flashbangDeactivationTimer)
        {
            flashbanged = false;
            overloaded = false;
            timeTilReactivated = 0;
        }
    }
}
