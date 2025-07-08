using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ActivateCutsceneObjects : MonoBehaviour
{
    [SerializeField] int cutsceneNumber;

    [Header("Cutscene 2 Settings")]
    [SerializeField] SpotlightPrefab[] spotlightPrefabs;
    [SerializeField] BoxCollider2D door;

    [Header("Cutscene 3 Settings")]
    [SerializeField] GameObject onboarding;

    public void StartActivate()
    {
        StartCoroutine(Activate());
    }

    private IEnumerator Activate()
    {
        yield return new WaitForSeconds(1);

        switch (cutsceneNumber)
        {
            case 2:
                spotlightPrefabs[0].state = SpotlightStates.Red;
                spotlightPrefabs[1].state = SpotlightStates.Laser;
                door.enabled = true;
                break;
            case 3:
                onboarding.SetActive(true);
                break;
            case 4:
                spotlightPrefabs[0].state = SpotlightStates.Off;
                spotlightPrefabs[1].state = SpotlightStates.Off;
                break;
        }
    }
}
