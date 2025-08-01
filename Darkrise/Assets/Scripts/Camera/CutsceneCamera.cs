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
    [SerializeField] private GameObject hud;

    private CinemachineBrain cinemachineBrain;
    private CinemachineBlendDefinition originalBlend;

    private void Start()
    {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        originalBlend = cinemachineBrain.m_DefaultBlend;
    }

    public void PlayCutscene(
        GameObject targetCutsceneCamera,
        float duration,
        bool hideHUD = true,
        bool showFade = true,
        Vector3? newPlayerPosition = null,
        GameObject[] objectsToActivate = null,
        GameObject[] objectsToDeactivate = null,
        System.Action onCutsceneTransition = null)
    {
        StartCoroutine(CutsceneSequence(targetCutsceneCamera, duration, hideHUD, showFade, newPlayerPosition, objectsToActivate, objectsToDeactivate, onCutsceneTransition));
    }

    private IEnumerator CutsceneSequence(
        GameObject targetCamera,
        float duration,
        bool hideHUD,
        bool showFade,
        Vector3? newPlayerPosition,
        GameObject[] objectsToActivate,
        GameObject[] objectsToDeactivate,
        System.Action onCutsceneTransition)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.CanMove = false;

        // Fast path: no fade, just switch cameras directly
        if (!showFade)
        {
            // Switch cameras
            allVirtualCameras = GameObject.FindGameObjectsWithTag("VirtualCamera");
            foreach (GameObject cam in allVirtualCameras)
            {
                if (cam.activeInHierarchy)
                {
                    previouslyActiveCamera = cam;
                }
                cam.SetActive(false);
            }

            targetCamera.SetActive(true);
            cinemachineBrain.ManualUpdate();

            if (hideHUD && hud != null)
                hud.SetActive(false);

            if (newPlayerPosition.HasValue)
                player.transform.position = newPlayerPosition.Value;

            if (objectsToActivate != null)
                foreach (GameObject go in objectsToActivate)
                    if (go != null) go.SetActive(true);

            if (objectsToDeactivate != null)
                foreach (GameObject go in objectsToDeactivate)
                    if (go != null) go.SetActive(false);

            onCutsceneTransition?.Invoke();

            yield return new WaitForSeconds(duration);

            // Switch back
            targetCamera.SetActive(false);
            if (previouslyActiveCamera != null)
                previouslyActiveCamera.SetActive(true);

            cinemachineBrain.ManualUpdate();

            if (hideHUD && hud != null)
                hud.SetActive(true);

            player.CanMove = true;
            yield break;
        }

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f));

        if (hideHUD && hud != null)
            hud.SetActive(false);

        if (newPlayerPosition.HasValue)
            player.transform.position = newPlayerPosition.Value;

        if (objectsToActivate != null)
            foreach (GameObject go in objectsToActivate)
                if (go != null) go.SetActive(true);

        if (objectsToDeactivate != null)
            foreach (GameObject go in objectsToDeactivate)
                if (go != null) go.SetActive(false);

        onCutsceneTransition?.Invoke();

        // Camera transition
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        allVirtualCameras = GameObject.FindGameObjectsWithTag("VirtualCamera");
        foreach (GameObject cam in allVirtualCameras)
        {
            if (cam.activeInHierarchy)
            {
                previouslyActiveCamera = cam;
            }
            cam.SetActive(false);
        }

        targetCamera.SetActive(true);
        cinemachineBrain.ManualUpdate();
        yield return new WaitForSeconds(0.05f);
        cinemachineBrain.m_DefaultBlend = originalBlend;

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));

        yield return new WaitForSeconds(duration);

        yield return StartCoroutine(Fade(0f, 1f));

        // Return camera
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        targetCamera.SetActive(false);

        if (previouslyActiveCamera != null)
            previouslyActiveCamera.SetActive(true);

        cinemachineBrain.ManualUpdate();
        yield return new WaitForSeconds(0.05f);
        cinemachineBrain.m_DefaultBlend = originalBlend;

        if (hideHUD && hud != null)
            hud.SetActive(true);

        player.CanMove = true;

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
