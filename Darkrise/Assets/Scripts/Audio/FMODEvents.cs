using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }

    [field: Header("Menu SFX")]
    [field: SerializeField] public EventReference pauseNavigate { get; private set; }
    [field: SerializeField] public EventReference pauseSelect { get; private set; }
    [field: SerializeField] public EventReference pauseBack { get; private set; }


    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference dash { get; private set; }

    [field: SerializeField] public EventReference playerFootsteps { get; private set; }

    [field: SerializeField] public EventReference normalSlash { get; private set; }

    [field: SerializeField] public EventReference darkSlash { get; private set; }

    [field: SerializeField] public EventReference lightSlash { get; private set; }

    [field: SerializeField] public EventReference playerJump { get; private set; }

    [field: SerializeField] public EventReference lightShot { get; private set; }

    [field: SerializeField] public EventReference lightExplode { get; private set; }

    [field: SerializeField] public EventReference darkShot { get; private set; }

    [field: SerializeField] public EventReference darkExplode { get; private set; }

    [field: SerializeField] public EventReference powerSelect { get; private set; }


    [field: Header("Enemy SFX")]
    [field: SerializeField] public EventReference soldierHurt { get; private set; }

    [field: SerializeField] public EventReference soldierDestroyed { get; private set; }

    [field: SerializeField] public EventReference soldierSpawns { get; private set; }

    [field: SerializeField] public EventReference soldierDetected { get; private set; }

    [field: SerializeField] public EventReference sentryHurt { get; private set; }

    [field: SerializeField] public EventReference sentrySpawns { get; private set; }

    [field: SerializeField] public EventReference sentryDestroyed { get; private set; }


    [field: Header("Spotlight SFX")]
    [field: SerializeField] public EventReference redAlarm { get; private set; }

    [field: SerializeField] public EventReference blueAlarm { get; private set; }


    [field: Header("Interactable SFX")]
    [field: SerializeField] public EventReference encounterPanel { get; private set; }

    [field: SerializeField] public EventReference pullLever { get; private set; }

    [field: SerializeField] public EventReference useStatue { get; private set; }


    public static FMODEvents instance { get;private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in this scene.");
        }
        instance = this;
    }
}
