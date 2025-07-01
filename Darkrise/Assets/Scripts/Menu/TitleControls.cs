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
    [SerializeField] GameObject LoadGameButton;
    [SerializeField] private Rewired.UI.ControlMapper.ControlMapper mapper = null;
    public GameObject pauseFirstButton, //button that's highlighted when you pause
    optionsFirstButton, //button that's highlighted when you first open the options menu
    optionsClosedButton;
    [SerializeField] private ConfirmationPopupMenu popup;
    bool newGame;
    bool runOnce = false;


    private void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        player = ReInput.players.GetPlayer(playerId);
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);

        if (DataPersistenceManager.Instance.HasGameData() == false)
        {
            LoadGameButton.GetComponent<Button>().interactable = false;
            LoadGameButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("UICancel"))
        {
            if (settingsMenuUI.activeSelf == true) { StartExitingSettingsMenu(); }
        }

        if (hasPressed)
        {
            pauseFirstButton.GetComponent<Button>().interactable = false;
            LoadGameButton.GetComponent<Button>().interactable = false;
            optionsClosedButton.GetComponent<Button>().interactable = false;
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

        SceneManager.LoadSceneAsync("StartLevel");
    }

    public void OnNewGameClicked()
    {
        if (hasPressed) { return; }

        if (DataPersistenceManager.Instance.HasGameData() == true)
        {
            pauseFirstButton.GetComponent<Button>().interactable = false;
            LoadGameButton.GetComponent<Button>().interactable = false;
            optionsClosedButton.GetComponent<Button>().interactable = false;
            popup.ActivateMenu(
                "Are you sure you want to make a new game? Any existing save data will be lost.",
                () => // if the player chooses yes
                {
                    hasPressed = true;
                    newGame = true;
                },
                () => // if the player chooses no
                {
                    pauseFirstButton.GetComponent<Button>().interactable = true;
                    LoadGameButton.GetComponent<Button>().interactable = true;
                    optionsClosedButton.GetComponent<Button>().interactable = true;
                    hasPressed = false;
                    EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
                    EventSystem.current.SetSelectedGameObject(pauseFirstButton);
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
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void ControlsMenu()
    {
        if (hasPressed) { return; }
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseSelect, this.transform.position);
        Debug.Log("Loading controls menu...");
        mapper.Open();
    }

    public void StartExitingSettingsMenu()
    {
        StartCoroutine(ExitSettingsMenu());
    }

    public void StartExitingControlsMenu()
    {
        StartCoroutine(ExitControlsMenu());
    }

    public IEnumerator ExitControlsMenu()
    {
        Debug.Log("Exiting controls menu...");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseBack, this.transform.position);
        yield return new WaitForSecondsRealtime(0.3f);
        mapper.Close(true);
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
        Debug.Log("Controls off");
    }

    public IEnumerator ExitSettingsMenu()
    {
        Debug.Log("Exiting settings menu...");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseBack, this.transform.position);
        yield return new WaitForSecondsRealtime(0.3f);
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
        settingsMenuUI.SetActive(false);
        Debug.Log("Settings off");
    }
}
