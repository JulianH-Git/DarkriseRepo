using Rewired;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    [SerializeField] public GameObject pauseMenuUI;
    [SerializeField] public GameObject settingsMenuUI;
    private Animator pauseAnim;
    private Animator settingsAnim;
    private Player player;
    private int playerId = 0;
    private List<ActionElementMap> controllerActionID = new List<ActionElementMap>();
    private List<ActionElementMap> keyboardActionID = new List<ActionElementMap>();
    public GameObject pauseFirstButton, //button that's highlighted when you pause
        optionsFirstButton, //button that's highlighted when you first open the options menu
        optionsClosedButton; //button that's highlighted after you close the options menu

    private void Awake()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);
        pauseAnim = pauseMenuUI.GetComponent<Animator>();
        settingsAnim = settingsMenuUI.GetComponent<Animator>();
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
        //resume music here!
        Time.timeScale = 1.0f;

        //enable the jump bind for controllers
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Joystick, 1, false, controllerActionID);
        controllerActionID.ForEach(m => m.enabled = true);

        //enable the jump bind for keyboards
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Keyboard, 1, false, keyboardActionID);
        keyboardActionID.ForEach(m => m.enabled = true);
        GamePaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        //pause music here!
        GamePaused = true;
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);

        //disable the jump bind for controllers
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Joystick, 1, true, controllerActionID);
        controllerActionID.ForEach(m => m.enabled = false);

        //disable the jump bind for keyboards
        player.controllers.maps.GetButtonMapsWithAction(ControllerType.Keyboard, 1, true, keyboardActionID);
        keyboardActionID.ForEach(m => m.enabled = false);

    }

    public void LoadMenu(string menuName)
    {
        Time.timeScale = 1.0f;
        Debug.Log($"Loading {menuName}...");
        SceneManager.LoadScene(menuName);
    }

    public void SettingsMenu()
    {
        Debug.Log("Loading settings menu...");
        settingsMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); // ALWAYS clear this before choosing a new object
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void StartExitingSettingsMenu()
    {
        StartCoroutine(ExitSettingsMenu());
    }

    public IEnumerator ExitSettingsMenu()
    {
        Debug.Log("Exiting settings menu...");
        settingsAnim.SetTrigger("exitSettingsMenu");
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
