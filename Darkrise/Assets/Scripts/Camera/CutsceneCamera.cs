using System.Collections;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    public GameObject cutsceneVirtualCamera;
    public float cutsceneDuration = 3f;

    private GameObject[] allVirtualCameras;
    private GameObject previouslyActiveCamera;

    public void PlayCutscene()
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

        // Activate the cutscene camera
        cutsceneVirtualCamera.SetActive(true);

        // Start coroutine to end the cutscene after a delay
        StartCoroutine(EndCutsceneAfterDelay());
    }

    private IEnumerator EndCutsceneAfterDelay()
    {
        yield return new WaitForSeconds(cutsceneDuration);

        // End cutscene: deactivate cutscene cam, reactivate the last active one
        cutsceneVirtualCamera.SetActive(false);

        if (previouslyActiveCamera != null)
        {
            previouslyActiveCamera.SetActive(true);
        }
    }
}
