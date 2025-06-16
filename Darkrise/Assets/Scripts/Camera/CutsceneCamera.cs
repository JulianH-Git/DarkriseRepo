using System.Collections;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    private PlayerController player;
    private GameObject[] allVirtualCameras;
    private GameObject previouslyActiveCamera;

    /// <summary>
    /// Cuts to the specified virtual camera for a period of time.
    /// </summary>
    /// <param name="targetCutsceneCamera">Camera to switch to.</param>
    /// <param name="duration">Time in seconds before switching the camera back.</param>
    public void PlayCutscene(GameObject targetCutsceneCamera, float duration)
    {
        // Store all active cameras and deactivate them
        allVirtualCameras = GameObject.FindGameObjectsWithTag("VirtualCamera");

        foreach (GameObject cam in allVirtualCameras)
        {
            if (cam.activeInHierarchy)
            {
                previouslyActiveCamera = cam;
            }
            cam.SetActive(false);
        }

        // Disable player controller
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.enabled = false;

        // Activate the target cutscene camera
        targetCutsceneCamera.SetActive(true);

        // Start coroutine with target camera and duration
        StartCoroutine(EndCutsceneAfterDelay(targetCutsceneCamera, duration));
    }

    private IEnumerator EndCutsceneAfterDelay(GameObject targetCamera, float duration)
    {
        yield return new WaitForSeconds(duration);

        // End cutscene: deactivate cutscene cam, reactivate the last active one
        targetCamera.SetActive(false);

        if (previouslyActiveCamera != null)
        {
            previouslyActiveCamera.SetActive(true);
        }

        // Enable player controller
        player.enabled = true;
    }
}
