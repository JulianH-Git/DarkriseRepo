using UnityEngine;
using System;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("References")]
    public CutsceneCamera cutsceneCameraManager;

    [Header("Cutscene Settings")]
    [Tooltip("Camera to switch to during the cutscene.")]
    public GameObject cutsceneCamera;

    [Tooltip("Duration of the cutscene in seconds.")]
    public float cutsceneDuration = 3f;

    [Tooltip("Hide HUD during the cutscene.")]
    public bool hideHUD = true;

    [Tooltip("Show fade transitions before and after the cutscene.")]
    public bool showFade = true;

    [Tooltip("Whether to teleport the player to newPlayerPosition at cutscene start.")]
    public bool teleportPlayer = false;

    [Tooltip("If teleportPlayer is true, set the new position for the player.")]
    public Vector3 newPlayerPosition;

    [Tooltip("Objects to activate.")]
    public GameObject[] objectsToActivate = null;

    [Tooltip("Objects to deactivate.")]
    public GameObject[] objectsToDeactivate = null;

    [Tooltip("Optional callback invoked on cutscene start.")]
    public Action onCutsceneTransition;

    /// <summary>
    /// Start the cutscene using the provided parameters.
    /// </summary>
    public void StartCutscene()
    {
        if (cutsceneCameraManager == null || cutsceneCamera == null)
        {
            Debug.LogWarning("CutsceneTrigger: Missing required references.");
            return;
        }

        cutsceneCameraManager.PlayCutscene(
            targetCutsceneCamera: cutsceneCamera,
            duration: cutsceneDuration,
            hideHUD: hideHUD,
            showFade: showFade,
            newPlayerPosition: teleportPlayer ? newPlayerPosition : (Vector3?)null,
            objectsToActivate: objectsToActivate,
            objectsToDeactivate: objectsToDeactivate,
            onCutsceneTransition: onCutsceneTransition
        );
    }
}
