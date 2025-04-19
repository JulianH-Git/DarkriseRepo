using System.Collections;
using UnityEngine;

public class ForcedEncounterRoomLock : MonoBehaviour
{
    Vector2 originalSize;
    bool moving = false;
    float duration = 0.1f;
    float alphaValue = 1.0f;
    [SerializeField] private Transform gateTargetPosition;
    SpriteRenderer sr;

    private void Start()
    {
        originalSize = this.GetComponentInChildren<Transform>().position;
        sr = this.GetComponent<SpriteRenderer>();
    }
    public void RemoveLock()
    {
        moving = true;
        StartCoroutine(MoveGates(gateTargetPosition.position));
    }

    public void ActivateLock()
    {
        this.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(moving)
        {
            alphaValue -= (duration * 100.0f) * Time.deltaTime;
        }
    }

    private IEnumerator MoveGates(Vector2 spotSize)
    {
        Vector2 startPosition = transform.position;
        transform.position = Vector2.Lerp(startPosition, spotSize, duration);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alphaValue);
        yield return new WaitForSeconds(duration);
        this.gameObject.SetActive(false);
    }
}
