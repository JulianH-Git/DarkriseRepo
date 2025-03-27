using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }

    [field: Header("Player SFX")]

    [field: SerializeField] public EventReference dash { get; private set; }

    [field: SerializeField] public EventReference playerFootsteps { get; private set; }

    [field: SerializeField] public EventReference normalSlash { get; private set; }

    [field: SerializeField] public EventReference darkSlash { get; private set; }

    [field: SerializeField] public EventReference lightSlash { get; private set; }

    [field: SerializeField] public EventReference playerJump { get; private set; }

    [field: SerializeField] public EventReference enemyHit { get; private set; }

    [field: SerializeField] public EventReference lightShot { get; private set; }

    [field: SerializeField] public EventReference darkShot { get; private set; }

    [field: SerializeField] public EventReference powerSelect { get; private set; }

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
