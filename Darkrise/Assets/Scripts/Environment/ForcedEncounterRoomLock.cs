using System.Collections;
using UnityEngine;

public class ForcedEncounterRoomLock : MonoBehaviour
{
    [SerializeField] Vector2 newSize;
    bool moving = false;
    float duration = 0.1f;
    [SerializeField] private Transform gateTargetPosition;

    public void RemoveLock()
    {
        moving = true;
        StartCoroutine(MoveGates(gateTargetPosition.position));
    }

    public void ActivateLock()
    {
        this.gameObject.SetActive(true);
    }

    private IEnumerator MoveGates(Vector2 spotSize)
    {
        Vector2 startPosition = transform.position;
        transform.position = Vector2.Lerp(startPosition, spotSize, duration);
        transform.localScale = Vector2.Lerp(transform.localScale, newSize, duration);
        yield return new WaitForSeconds(duration);
        this.gameObject.SetActive(false);
    }
}
