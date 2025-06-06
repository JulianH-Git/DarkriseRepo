using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DarkLightUIUpdater : MonoBehaviour
{
    PlayerController player;
    [SerializeField] GameObject hintTextObj;
    TextMeshProUGUI darkEnergyText;
    TextMeshProUGUI lightEnergyText;
    TextMeshProUGUI hintText;

    float fadeTimer = 3f;
    float fadeAwayPerSecond;
    float alphaValue;


    void Awake()
    {
        player = PlayerController.Instance;
        
        hintText = hintTextObj.GetComponent<TextMeshProUGUI>();

        fadeAwayPerSecond = 1 / fadeTimer;
        alphaValue = hintText.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.Health <= 0) { TurnOff(); }

        if(fadeTimer >= 0)
        {
            alphaValue -= fadeAwayPerSecond * Time.deltaTime;
            hintText.color = new Color(hintText.color.r, hintText.color.g, hintText.color.b, alphaValue);
            fadeTimer -= Time.deltaTime;
        }

        if(fadeTimer <= 0)
        {
            hintTextObj.SetActive(false);
        }
        
    }
    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        hintText.color = new Color(hintText.color.r, hintText.color.g, hintText.color.b, 1);
        fadeTimer = 3f; 
        alphaValue = 1f;
        hintTextObj.SetActive(true);
        gameObject.SetActive(true);
    }
}
