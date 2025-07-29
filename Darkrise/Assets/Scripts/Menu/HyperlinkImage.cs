using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HyperlinkImage : MonoBehaviour, IPointerClickHandler
{
    public string url = "https://forms.gle/XVdBWxntyq46MV456"; // Set your desired URL here
    [SerializeField] private ConfirmationPopupMenu popup;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject quitButton;
    [SerializeField] GameObject surveyButton;

    public void OnPointerClick(PointerEventData eventData)
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