using System.Collections;
using UnityEngine;
using Cinemachine;

public class CutsceneCamera : MonoBehaviour
{
    private PlayerController player;
    private GameObject[] allVirtualCameras;
    private GameObject previouslyActiveCamera;

    [SerializeField] private SpriteRenderer fade;
    private float fadeSpeed = 2f;

    private CinemachineBrain cinemachineBrain;
    private CinemachineBlendDefinition originalBlend;

    private void Start()
    {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        originalBlend = cinemachineBrain.m_DefaultBlend; // Save Cinemachine blending
    }

    /// <summary>
    /// Cuts to the specified virtual camera for a period of time.
    /// </summary>
    /// <param name="targetCutsceneCamera">Camera to switch to.</param>
    /// <param name="duration">Time in seconds before switching the camera back.</param>
    public void PlayCutscene(GameObject targetCutsceneCamera, float duration)
    {
        StartCoroutine(CutsceneSequence(targetCutsceneCamera, duration));
    }

    private IEnumerator CutsceneSequence(GameObject targetCamera, float duration)
    {
        // Disable player controller
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.CanMove = false;

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f));

        // Force instant cut
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

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

        // Activate the target cutscene camera
        targetCamera.SetActive(true);

        // Force Cinemachine to update immediately
        cinemachineBrain.ManualUpdate();

        // Short safety delay
        yield return new WaitForSeconds(0.05f);

        // Restore original blending
        cinemachineBrain.m_DefaultBlend = originalBlend;

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));

        // Wait for the cutscene duration
        yield return new WaitForSeconds(duration);

        // Fade out again
        yield return StartCoroutine(Fade(0f, 1f)); // Fully fade out

        // Force instant cut
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

        // Switch back to previous camera
        targetCamera.SetActive(false);

        if (previouslyActiveCamera != null)
        {
            previouslyActiveCamera.SetActive(true);
        }

        // Force Cinemachine to update immediately
        cinemachineBrain.ManualUpdate();

        // Safety delay
        yield return new WaitForSeconds(0.05f);

        // Restore blending
        cinemachineBrain.m_DefaultBlend = originalBlend;

        // Enable player controller
        player.CanMove = true;

        // Fade back in to gameplay
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fade.color;

        while (Mathf.Abs(fade.color.a - endAlpha) > 0.01f)
        {
            elapsedTime += Time.deltaTime * fadeSpeed;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime);
            fade.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }

        fade.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
