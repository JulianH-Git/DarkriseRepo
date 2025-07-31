using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.U2D;

public class ActivateCutsceneObjects : MonoBehaviour
{
    [SerializeField] int cutsceneNumber;

    [Header("Cutscene 2 Settings")]
    [SerializeField] DetectorLaser dl;
    [SerializeField] BoxCollider2D door;

    [Header("Cutscene 3 Settings")]
    [SerializeField] GameObject onboarding;

    [Header("Cutscene 4 Settings")]
    [SerializeField] SpotlightPrefab redSpotlight;
    [SerializeField] GameObject cutsceneTrigger;

    [Header("Cutscene 5 Settings")]
    [SerializeField] PlayableDirector timeline;
    public bool hasBeenPlayed;

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
                dl.turnedOn = true;
                door.enabled = true;
                break;
            case 3:
                onboarding.SetActive(true);
                break;
            case 4:
                dl.turnedOn = false;
                redSpotlight.state = SpotlightStates.Off;
                cutsceneTrigger.SetActive(false);
                break;
            case 5:
                TimelineActivate(hasBeenPlayed);
                break;
        }
    }

    public void TimelineActivate(bool hasBeenPlayed) 
    {
        if ((!hasBeenPlayed))
        {
            timeline.Play();
            hasBeenPlayed = true;
        }
    }
}
