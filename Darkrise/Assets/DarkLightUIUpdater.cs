using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DarkLightUIUpdater : MonoBehaviour
{
    PlayerController player;
    [SerializeField] GameObject darkEnergyTextObj;
    [SerializeField] GameObject lightEnergyTextObj;
    TMP_Text darkEnergyText;
    TMP_Text lightEnergyText;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        darkEnergyText = darkEnergyTextObj.GetComponent<TMP_Text>();
        lightEnergyText = lightEnergyTextObj.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        darkEnergyText.text = player.currentDarkEnergy.ToString();
        lightEnergyText.text = player.currentLightEnergy.ToString();
    }
}
