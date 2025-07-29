using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverControls : MonoBehaviour
{
    private bool hasPressed = false;
    private bool runOnce = false;
    private Player player;
    private int playerId = 0;
    float alpha = 0f;
    [SerializeField] SpriteRenderer fade;
    [SerializeField] private ConfirmationPopupMenu popup;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject quitButton;
    [SerializeField] GameObject surveyButton;
    public string url = "https://forms.gle/XVdBWxntyq46MV456";

    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(restartButton);
    }
    void Update()
    {
        if (hasPressed)
        {
            ButtonStatus(false);
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alpha);
            alpha += 0.05f;
        }

        if (alpha >= 1f && !runOnce)
        {
            runOnce = true;
            DataPersistenceManager.Instance.LoadGame();
            SceneManager.LoadSceneAsync("DemoLevel");
        }
    }

    public void OnRestartClicked()
    {
        if (!hasPressed)
        {
            hasPressed = true;
        }
    }

    public void OnQuitButtonClicked()
    {
        ButtonStatus(false);
        popup.ActivateMenu(
               "Are you sure? Any unsaved progress will be lost.",
               () => // if the player chooses yes
               {
                   Application.Quit();
               },
               () => // if the player chooses no
               {
                   ButtonStatus(true);
                   EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
                   EventSystem.current.SetSelectedGameObject(restartButton);
               });
    }

    public void OnSurveyClick()
    {
        ButtonStatus(false);
        popup.ActivateMenu(
                "This will open the survey in your browser.",
                () => // if the player chooses yes
                {
                    Application.OpenURL(url);
                    ButtonStatus(true);
                    EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
                    EventSystem.current.SetSelectedGameObject(surveyButton);
                },
                () => // if the player chooses no
                {
                    ButtonStatus(true);
                    EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
                    EventSystem.current.SetSelectedGameObject(surveyButton);
                });

    }

    private void ButtonStatus(bool value)
    {
        restartButton.GetComponent<Button>().interactable = value;
        quitButton.GetComponent<Button>().interactable = value;
        surveyButton.GetComponent<Button>().interactable = value;
    }
}
