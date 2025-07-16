using Rewired.Utils.Classes.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : MonoBehaviour, IDataPersistence
{
    [SerializeField] bool selfContained;
    [SerializeField] float overloadTimer;
    [SerializeField] float flashbangDeactivationTimer;
    float timeTilReactivated;
    float timeToOverload;
    public bool powered;
    public bool overloaded;
    public bool flashbanged;
    private List<Vector2> originalSizes = new List<Vector2>();
    bool playerIsNear;
    bool runOnce = false;
    SpriteRenderer sr;
    Color lerpedColor;
    [SerializeField] List<GameObject> objectsToPower;
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
        foreach (GameObject obj in objectsToPower)
        {
            originalSizes.Add(obj.transform.localScale);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerIsNear)
        {
            timeToOverload -= Time.deltaTime;
            LerpColors();
        }
        if(flashbanged)
        {
            sr.color = Color.magenta;
            Flashbanged();

        }
        if(powered && !overloaded)
        {
            if (selfContained)
            {
                for (int i = 0; i < objectsToPower.Count; i++)
                {
                    StartCoroutine(MoveGates(objectsToPower[i], new Vector2(objectsToPower[i].transform.localScale.x,0)));
                }
            }
        }
        if(powered && overloaded)
        {
            if (selfContained)
            {
                for (int i = 0; i < objectsToPower.Count; i++)
                {
                    StartCoroutine(RevertGates(objectsToPower[i], originalSizes[i]));
                }
            }
        }
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.isTrigger && !powered && PlayerController.Instance.currentAttackType == PlayerController.AttackType.Light)
        {
            playerIsNear = true;
            powered = true;
            sr.color = Color.yellow;
            Debug.Log("Powered");
        }
        if(collision.CompareTag("Player") && collision.isTrigger && powered && PlayerController.Instance.BubbleUp)
        {
            playerIsNear = true;
            timeToOverload = timeToOverload + Time.deltaTime;
            LerpColors();
            if (timeToOverload >= overloadTimer)
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
                playerIsNear = false;

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
            runOnce = false;
            timeTilReactivated = 0;
            if (selfContained)
            {
                for (int i = 0; i < objectsToPower.Count; i++)
                {
                    StartCoroutine(RevertGates(objectsToPower[i], originalSizes[i]));
                }
            }
            return;
        }

        if (selfContained && !runOnce)
        {
            runOnce = true;
            foreach (GameObject obj in objectsToPower)
            {
                StartCoroutine(MoveGates(obj, new Vector2(obj.transform.localScale.x, 0)));
            }
        }


    }

    protected void LerpColors()
    {
        if(!powered) { sr.color = Color.white; return; }
        lerpedColor = Color.Lerp(Color.yellow, Color.red, timeToOverload / overloadTimer);
        sr.color = lerpedColor;
    }

    private IEnumerator MoveGates(GameObject gate, Vector2 spotSize)
    {
        Vector2 initialScale = gate.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < 0.1f)
        {
            if (gate != null) // Ensure the object is still valid
            {
                gate.transform.localScale = Vector2.Lerp(initialScale, spotSize, elapsedTime / 0.1f);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            else
            {
                yield break; // Exit the coroutine if the object is null
            }
        }

        if (gate != null)
        {
            gate.transform.localScale = spotSize; // Ensure exact final size
            gate.SetActive(false);
        }
    }

    private IEnumerator RevertGates(GameObject gate, Vector2 originalSize)
    {
        gate.SetActive(true);
        Vector2 initialScale = gate.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < 0.1f)
        {
            if (gate != null) // Ensure the object is still valid
            {
                gate.transform.localScale = Vector2.Lerp(initialScale, originalSize, elapsedTime / 0.1f);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            else
            {
                yield break; // Exit the coroutine if the object is null
            }
        }

        if (gate != null)
        {
            gate.transform.localScale = originalSize; // Ensure exact final size
            
        }
    }
}
