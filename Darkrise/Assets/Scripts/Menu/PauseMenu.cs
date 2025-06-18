using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public static bool inSettings = false;
    [SerializeField] public GameObject pauseMenuUI;
    [SerializeField] public GameObject settingsMenuUI;
    [SerializeField] private Rewired.UI.ControlMapper.ControlMapper mapper = null;
    private Animator pauseAnim;
    private Animator settingsAnim;
    private Player player;
    private int playerId = 0;
    private List<ActionElementMap> controllerActionID = new List<ActionElementMap>();
    private List<ActionElementMap> keyboardActionID = new List<ActionElementMap>();
    public GameObject pauseFirstButton, //button that's highlighted when you pause
        optionsFirstButton, //button that's highlighted when you first open the options menu
        optionsClosedButton;

    private void Awake()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);
        pauseAnim = pauseMenuUI.GetComponent<Animator>();
        settingsAnim = settingsMenuUI.GetComponent<Animator>();
        //enable the jump bind for controllers
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Joystick, "Jump", false, controllerActionID);
        controllerActionID.ForEach(m => m.enabled = true);

        //enable the jump bind for keyboards
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Keyboard, "Jump", false, keyboardActionID);
        keyboardActionID.ForEach(m => m.enabled = true);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("Pause"))
        {
            if (GamePaused) { StartResume(); }
            else { Pause(); }
        }
        if(player.GetButtonDown("UICancel"))
        {
            if (GamePaused && settingsMenuUI.activeSelf == false) { StartResume(); }
            else if(GamePaused && settingsMenuUI.activeSelf == true) { StartExitingSettingsMenu(); }
        }
        if(GamePaused && player.GetButtonDown("UIVertical") && EventSystem.current.currentSelectedGameObject == null)
        {
            if(settingsMenuUI.activeSelf == true)
            {
                EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
                EventSystem.current.SetSelectedGameObject(optionsFirstButton);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
                EventSystem.current.SetSelectedGameObject(pauseFirstButton);
            }
        }
        if(player.GetButtonDown("UISubmit"))
        {
            Debug.Log("button down");
        }
    }

    public void StartResume()
    {
        StartCoroutine(Resume());
    }
    public IEnumerator Resume()
    {
        pauseAnim.SetTrigger("exitMenu");
        yield return new WaitForSecondsRealtime(0.1f);
        pauseMenuUI.SetActive(false);
        mapper.Close(true);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1.0f;

        //enable the jump bind for controllers
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Joystick, "Jump", false, controllerActionID);
        controllerActionID.ForEach(m => m.enabled = true);

        //enable the jump bind for keyboards
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Keyboard, "Jump", false, keyboardActionID);
        keyboardActionID.ForEach(m => m.enabled = true);

        player.controllers.maps.SetMapsEnabled(true, "Gameplay");
        player.controllers.maps.SetMapsEnabled(false, "UI");

        GamePaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        GamePaused = true;
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);

        //disable the jump bind for controllers
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Joystick, "Jump", true, controllerActionID);
        controllerActionID.ForEach(m => m.enabled = false);

        //disable the jump bind for keyboards
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Keyboard, "Jump", true, keyboardActionID);
        keyboardActionID.ForEach(m => m.enabled = false);

        player.controllers.maps.SetMapsEnabled(false, "Gameplay");
        player.controllers.maps.SetMapsEnabled(true, "UI");

    }

    public void LoadMenu(string menuName)
    {
        Time.timeScale = 1.0f;
        GamePaused = false;

        //enable the jump bind for controllers
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Joystick, "Jump", false, controllerActionID);
        controllerActionID.ForEach(m => m.enabled = true);

        //enable the jump bind for keyboards
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Keyboard, "Jump", false, keyboardActionID);
        keyboardActionID.ForEach(m => m.enabled = true);

        player.controllers.maps.SetMapsEnabled(true, "Gameplay");
        player.controllers.maps.SetMapsEnabled(false, "UI");


        Debug.Log($"Loading {menuName}...");
        SceneManager.LoadScene(menuName);
    }

    public void SettingsMenu()
    {
        inSettings = true;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseSelect, this.transform.position);
        Debug.Log("Loading settings menu...");
        settingsMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void ControlsMenu()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pauseSelect, this.transform.position);
        Debug.Log("Loading controls menu...");
        mapper.Open();
    }

    public void StartExitingSettingsMenu()
    {
        StartCoroutine(ExitSettingsMenu());
        inSettings = false;
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
