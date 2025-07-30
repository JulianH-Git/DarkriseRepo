using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public GameObject cutsceneCamera;               // camera to cut to
    public float cutsceneDuration = 3f;             // seconds before cutting back
    public CutsceneCamera cutsceneCameraManager;    // cutscene manager object

    /// <summary>
    /// Plays the cutscene defined by this script.
    /// </summary>
    public void StartCutscene()
    {
        cutsceneCameraManager.PlayCutscene(
            targetCutsceneCamera: cutsceneCamera,
            duration: 3f,
            hideHUD: true,
            newPlayerPosition: null,
            onCutsceneTransition: () => Debug.Log("playing cutscene")
        );
    }
}
