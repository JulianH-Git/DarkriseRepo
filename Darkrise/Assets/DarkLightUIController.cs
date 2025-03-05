using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DarkLightUIController : MonoBehaviour
{
    PlayerController player;
    [SerializeField] Transform darkLightParent;

    [SerializeField] GameObject neutralModePrefab;
    [SerializeField] GameObject darkModePrefab;
    [SerializeField] GameObject lightModePrefab;
    private GameObject[] darkLightPrefabs;
    PlayerController.AttackType currentAttackType;

    DarkLightUIUpdater neutralDLUIScript;
    DarkLightUIUpdater darkDLUIScript;
    DarkLightUIUpdater lightDLUIScript;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        darkLightPrefabs = new GameObject[3];
        InstantiateDarkLightPrefabs(neutralModePrefab, 0); // 0 = neutral
        InstantiateDarkLightPrefabs(lightModePrefab, 1); // 1 = light
        InstantiateDarkLightPrefabs(darkModePrefab, 2); // 2 = dark

        neutralDLUIScript = darkLightPrefabs[0].GetComponent<DarkLightUIUpdater>();
        lightDLUIScript = darkLightPrefabs[1].GetComponent<DarkLightUIUpdater>();
        darkDLUIScript = darkLightPrefabs[2].GetComponent<DarkLightUIUpdater>();

        currentAttackType = player.currentAttackType;

        switch (player.currentAttackType)
        {

            case (PlayerController.AttackType.Neutral):
                neutralDLUIScript.TurnOn();
                darkDLUIScript.TurnOff();
                lightDLUIScript.TurnOff();
                break;

            case (PlayerController.AttackType.Light):
                neutralDLUIScript.TurnOff();
                darkDLUIScript.TurnOff();
                lightDLUIScript.TurnOn();
                break;

            case (PlayerController.AttackType.Dark):
                neutralDLUIScript.TurnOff();
                darkDLUIScript.TurnOn();
                lightDLUIScript.TurnOff();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (darkLightPrefabs != null)
        {
            if(currentAttackType != player.currentAttackType)
            {
                switch (player.currentAttackType)
                {

                    case (PlayerController.AttackType.Neutral):
                        neutralDLUIScript.TurnOn();
                        darkDLUIScript.TurnOff();
                        lightDLUIScript.TurnOff();
                        break;

                    case (PlayerController.AttackType.Light):
                        neutralDLUIScript.TurnOff();
                        darkDLUIScript.TurnOff();
                        lightDLUIScript.TurnOn();
                        break;

                    case (PlayerController.AttackType.Dark):
                        neutralDLUIScript.TurnOff();
                        darkDLUIScript.TurnOn();
                        lightDLUIScript.TurnOff();
                        break;
                }
            }
        }
        currentAttackType = player.currentAttackType;
    }

    void InstantiateDarkLightPrefabs(GameObject _UIPrefab, int index)
    {
        GameObject temp = Instantiate(_UIPrefab);
        temp.transform.SetParent(darkLightParent, false);
        darkLightPrefabs[index] = temp;
        darkLightPrefabs[index].SetActive(false);
    }
}
