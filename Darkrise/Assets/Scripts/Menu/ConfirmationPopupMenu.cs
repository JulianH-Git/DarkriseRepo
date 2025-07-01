using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmationPopupMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject menuBG;

    public void ActivateMenu(string questionText, UnityAction confirmAction, UnityAction cancelAction)
    {
        this.gameObject.SetActive(true);
        menuBG.SetActive(true);
        this.displayText.text = questionText;

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            confirmAction();
        });

        cancelButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            cancelAction();
        });

    }

    public void DeactivateMenu()
    {
        menuBG.SetActive(false);
        this.gameObject.SetActive(false);
    }

}
