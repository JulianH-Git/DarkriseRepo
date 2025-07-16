using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LightCooldownUpdater : MonoBehaviour
{
    PlayerController player;
    [SerializeField] GameObject hintTextObj;
    [SerializeField] Image lightBubbleRefill;
    [SerializeField] Image remoteFlashbangRefill;
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
        if (player.Health <= 0) { TurnOff(); }
        if (fadeTimer >= 0)
        {
            alphaValue -= fadeAwayPerSecond * Time.deltaTime;
            hintText.color = new Color(hintText.color.r, hintText.color.g, hintText.color.b, alphaValue);
            fadeTimer -= Time.deltaTime;
        }
        if (fadeTimer <= 0)
        {
            hintTextObj.SetActive(false);
        }
        UpdateRefills();
    }

    public void UpdateRefills()
    {
        lightBubbleRefill.fillAmount = player.LightBubbleCooldown;
        remoteFlashbangRefill.fillAmount = player.RemoteFlashbangCooldown;
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
