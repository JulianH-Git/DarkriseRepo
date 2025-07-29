using Rewired;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleControls : MonoBehaviour
{
    private bool hasPressed = false;

    [SerializeField]
    SpriteRenderer fade;

    float alpha = 0.0f;

    private Player player;
    private int playerId = 0;
    [SerializeField] public GameObject settingsMenuUI;
    private Animator settingsMenuAnim;
    [SerializeField] GameObject LoadGameButton;
    [SerializeField] GameObject surveyButton;
    [SerializeField] private Rewired.UI.ControlMapper.ControlMapper mapper = null;
    [SerializeField] Toggle hitStopCheck;
    public GameObject pauseFirstButton, //button that's highlighted when you pause
    optionsFirstButton, //button that's highlighted when you first open the options menu
    optionsClosedButton;
    [SerializeField] private ConfirmationPopupMenu popup;
    bool newGame;
    bool runOnce = false;
    public string url = "https://forms.gle/XVdBWxntyq46MV456";


    private void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        player = ReInput.players.GetPlayer(playerId);
        settingsMenuAnim = settingsMenuUI.GetComponent<Animator>();
        SelectButton(pauseFirstButton);

        if (DataPersistenceManager.Instance.HasGameData() == false)
        {
            LoadGameButton.GetComponent<Button>().interactable = false;
            LoadGameButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(settingsMenuUI.activeSelf != true && mapper.isOpen == true) 
        { 
            mapper.Close(true);
            SelectButton(pauseFirstButton);
        }
        if (player.GetButtonDown("UICancel"))
        {
            if (settingsMenuUI.activeSelf && !mapper.isOpen) { StartExitingSettingsMenu(); }
            else if(settingsMenuUI.activeSelf && mapper.isOpen) { StartExitingControlsMenu(); }
        }

        if (hasPressed)
        {
            ButtonStatus(false);
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alpha);
            alpha += 0.05f;
        }


        if (alpha >= 2 && !runOnce)
        {
            runOnce = true;
            StartGame();
        }
    }

    private void StartGame()
    {
        if (newGame)
        {
            DataPersistenceManager.Instance.NewGame();
            DataPersistenceManager.Instance.SaveGame();
        }
        else
        {
            DataPersistenceManager.Instance.LoadGame();
        }

        SceneManager.LoadSceneAsync("DemoLevel");
    }

    public void OnNewGameClicked()
    {
        if (hasPressed) { return; }

        if (DataPersistenceManager.Instance.HasGameData() == true)
        {
            ButtonStatus(false);
            popup.ActivateMenu(
                "Are you sure? Any existing save data will be lost.",
                () => // if the player chooses yes
                {
                    hasPressed = true;
                    newGame = true;
                },
                () => // if the player chooses no
                {
                    ButtonStatus(true);
                    hasPressed = false;
                    SelectButton(pauseFirstButton);
                });
        }
        else
        {
            hasPressed = true;
            newGame = true;
        }

    }

    public void OnLoadGameClicked()
    {
        if (hasPressed) { return; }
        hasPressed = true;
        newGame = false;
    }

    public void SettingsMenu()
    {
        if (hasPressed) { return; }
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseSelect, this.transform.position);
        Debug.Log("Loading settings menu...");
        settingsMenuUI.SetActive(true);
        optionsFirstButton.GetComponent<Button>().interactable = true;
        SelectButton(optionsFirstButton);
    }

    public void ControlsMenu()
    {
        if (hasPressed) { return; }
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseSelect, this.transform.position);
        Debug.Log("Loading controls menu...");
        EventSystem.current.SetSelectedGameObject(null);
        mapper.Open();
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
                    SelectButton(surveyButton);
                },
                () => // if the player chooses no
                {
                    ButtonStatus(true);
                    SelectButton(surveyButton);
                });

    }

    public void StartExitingSettingsMenu()
    {
        StartCoroutine(ExitSettingsMenu());
    }
    public void ToggleHitStop()
    {
        DataPersistenceManager.Instance.ToggleHitStop(hitStopCheck.isOn);
    }
    public void StartExitingControlsMenu()
    {
        Debug.Log("Exiting controls menu...");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseBack, this.transform.position);
        mapper.Close(true);
        SelectButton(optionsFirstButton);
        Debug.Log("Controls off");
    }

    public IEnumerator ExitSettingsMenu()
    {
        Debug.Log("Exiting settings menu...");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseBack, this.transform.position);
        optionsFirstButton.GetComponent<Button>().interactable = false;
        settingsMenuAnim.SetTrigger("exitSettingsMenu");
        yield return new WaitForSecondsRealtime(0.2f);
        SelectButton(optionsClosedButton);
        settingsMenuUI.SetActive(false);
        Debug.Log("Settings off");
    }

    private void ButtonStatus(bool value)
    {
        pauseFirstButton.GetComponent<Button>().interactable = value;
        LoadGameButton.GetComponent<Button>().interactable = value;
        optionsClosedButton.GetComponent<Button>().interactable = value;
        surveyButton.GetComponent<Button>().interactable = value;
    }

    private void SelectButton(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(obj);
    }
}
