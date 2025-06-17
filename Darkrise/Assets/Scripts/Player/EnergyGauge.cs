using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyGauge : MonoBehaviour
{
    Image bar;
    void Start()
    {
        bar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float barPercentage = (PlayerController.Instance.currentEnergy / PlayerController.Instance.maxEnergy);
        bar.fillAmount = barPercentage;
    }
}
