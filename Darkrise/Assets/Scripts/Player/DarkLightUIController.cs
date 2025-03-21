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
    [SerializeField] GameObject neutralModeDarkOnlyPrefab;
    [SerializeField] GameObject darkModeDarkOnlyPrefab;

    private GameObject[] darkLightPrefabs;
    PlayerController.AttackType currentAttackType;

    DarkLightUIUpdater[] DLUIScripts;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        darkLightPrefabs = new GameObject[5];
        InstantiateDarkLightPrefabs(neutralModePrefab, 0); // 0 = neutral
        InstantiateDarkLightPrefabs(lightModePrefab, 1); // 1 = light
        InstantiateDarkLightPrefabs(darkModePrefab, 2); // 2 = dark
        InstantiateDarkLightPrefabs(neutralModeDarkOnlyPrefab, 3); // 3 = neutral no light
        InstantiateDarkLightPrefabs(darkModeDarkOnlyPrefab, 4); // 4 = dark mode no light

        DLUIScripts = new DarkLightUIUpdater[5];

        DLUIScripts[0] = darkLightPrefabs[0].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[1] = darkLightPrefabs[1].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[2] = darkLightPrefabs[2].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[3] = darkLightPrefabs[3].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[4] = darkLightPrefabs[4].GetComponent<DarkLightUIUpdater>();

        currentAttackType = player.currentAttackType;
    }

    // Update is called once per frame
    void Update()
    {
        if (darkLightPrefabs != null)
        {
            if(player.darkUnlocked)
            {
                switch (player.currentAttackType)
                {

                    case (PlayerController.AttackType.Neutral):
                        if(player.darkUnlocked && !player.lightUnlocked)
                        {
                            DLUIScripts[3].TurnOn();
                            DLUIScripts[4].TurnOff();
                        }
                        else
                        {
                            DLUIScripts[0].TurnOn();
                            DLUIScripts[2].TurnOff();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOff();
                        }
                        break;

                    case (PlayerController.AttackType.Light):
                        DLUIScripts[0].TurnOff();
                        DLUIScripts[2].TurnOff();
                        DLUIScripts[1].TurnOn();
                        DLUIScripts[3].TurnOff();
                        DLUIScripts[4].TurnOff();
                        break;

                    case (PlayerController.AttackType.Dark):
                        if (player.darkUnlocked && !player.lightUnlocked)
                        {
                            DLUIScripts[3].TurnOff();
                           DLUIScripts[4].TurnOn();
                        }
                        else
                        {
                            DLUIScripts[0].TurnOff();
                            DLUIScripts[2].TurnOn();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOff();
                        }
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
