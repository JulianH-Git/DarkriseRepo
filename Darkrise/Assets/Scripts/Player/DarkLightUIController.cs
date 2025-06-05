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
    [SerializeField] GameObject lightModeLightOnlyPrefab;
    [SerializeField] GameObject neutralModeLightOnlyPrefab;
    [SerializeField] GameObject darkOnlyPrefab;
    [SerializeField] GameObject lightOnlyPrefab;

    private GameObject[] darkLightPrefabs;
    PlayerController.AttackType currentAttackType;

    DarkLightUIUpdater[] DLUIScripts;

    [SerializeField] GameObject text;
    SpriteRenderer textSR;
    [SerializeField] Sprite neutralText;
    [SerializeField] Sprite lightText;
    [SerializeField] Sprite darkText;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        player.onEnergyChangedCallback += UpdateUI;
        darkLightPrefabs = new GameObject[9];
        InstantiateDarkLightPrefabs(neutralModePrefab, 0); // 0 = neutral
        InstantiateDarkLightPrefabs(lightModePrefab, 1); // 1 = light
        InstantiateDarkLightPrefabs(darkModePrefab, 2); // 2 = dark
        InstantiateDarkLightPrefabs(neutralModeDarkOnlyPrefab, 3); // 3 = neutral no light
        InstantiateDarkLightPrefabs(darkModeDarkOnlyPrefab, 4); // 4 = dark mode no light
        InstantiateDarkLightPrefabs(lightModeLightOnlyPrefab, 5); // 5 = light mode no dark
        InstantiateDarkLightPrefabs(neutralModeLightOnlyPrefab, 6); // 6 = neutral mode no dark
        InstantiateDarkLightPrefabs(darkOnlyPrefab, 7); // 7 = dark only
        InstantiateDarkLightPrefabs(lightOnlyPrefab, 8); // 8 = light only

        DLUIScripts = new DarkLightUIUpdater[9];

        DLUIScripts[0] = darkLightPrefabs[0].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[1] = darkLightPrefabs[1].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[2] = darkLightPrefabs[2].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[3] = darkLightPrefabs[3].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[4] = darkLightPrefabs[4].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[5] = darkLightPrefabs[5].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[6] = darkLightPrefabs[6].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[7] = darkLightPrefabs[7].GetComponent<DarkLightUIUpdater>();
        DLUIScripts[8] = darkLightPrefabs[8].GetComponent<DarkLightUIUpdater>();

        currentAttackType = player.currentAttackType;

        textSR = text.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.currentAttackType != currentAttackType)
        {
            UpdateUI();
            currentAttackType = player.currentAttackType; // Update stored attack type
        }
    }

    void UpdateUI()
    {
        if (darkLightPrefabs != null)
        {
            if(!player.darkUnlocked && !player.lightUnlocked)
            {
                DLUIScripts[0].TurnOff();
                DLUIScripts[1].TurnOff();
                DLUIScripts[2].TurnOff();
                DLUIScripts[3].TurnOff();
                DLUIScripts[4].TurnOff();
                DLUIScripts[5].TurnOff();
                DLUIScripts[6].TurnOff();
                DLUIScripts[7].TurnOff();
                DLUIScripts[8].TurnOff();
                textSR.sprite = neutralText;
            }
            else
            {
                text.SetActive(true);
                switch (player.currentAttackType)
                {
                    case (PlayerController.AttackType.Neutral):
                        if (player.darkUnlocked && !player.lightUnlocked)
                        {
                            DLUIScripts[0].TurnOff();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[2].TurnOff();
                            DLUIScripts[3].TurnOn();
                            DLUIScripts[4].TurnOff();
                            DLUIScripts[5].TurnOff();
                            DLUIScripts[6].TurnOff();
                            DLUIScripts[7].TurnOff();
                            DLUIScripts[8].TurnOff();
                            textSR.sprite = neutralText;
                        }
                        else if (player.lightUnlocked && !player.darkUnlocked)
                        {
                            DLUIScripts[6].TurnOn();

                            DLUIScripts[0].TurnOff();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[2].TurnOff();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOff();
                            DLUIScripts[5].TurnOff();
                            DLUIScripts[7].TurnOff();
                            DLUIScripts[8].TurnOff();
                            textSR.sprite = neutralText;
                        }
                        else
                        {
                            DLUIScripts[0].TurnOn();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[2].TurnOff();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOff();
                            DLUIScripts[5].TurnOff();
                            DLUIScripts[6].TurnOff();
                            DLUIScripts[7].TurnOff();
                            DLUIScripts[8].TurnOff();
                            textSR.sprite = neutralText;
                        }
                        break;

                    case (PlayerController.AttackType.Light):
                        if (player.lightUnlocked && player.modeLocked)
                        {
                            DLUIScripts[0].TurnOff();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[2].TurnOff();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOff();
                            DLUIScripts[5].TurnOff();
                            DLUIScripts[6].TurnOff();
                            DLUIScripts[7].TurnOff();
                            DLUIScripts[8].TurnOn();
                            textSR.sprite = lightText;
                        }
                        else if (player.lightUnlocked && !player.darkUnlocked)
                        {
                            DLUIScripts[5].TurnOn();

                            DLUIScripts[0].TurnOff();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[2].TurnOff();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOff();
                            DLUIScripts[6].TurnOff();
                            DLUIScripts[7].TurnOff();
                            DLUIScripts[8].TurnOff();
                            textSR.sprite = lightText;
                        }
                        else
                        {
                            DLUIScripts[0].TurnOff();
                            DLUIScripts[1].TurnOn();
                            DLUIScripts[2].TurnOff();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOff();
                            DLUIScripts[5].TurnOff();
                            DLUIScripts[6].TurnOff();
                            DLUIScripts[7].TurnOff();
                            DLUIScripts[8].TurnOff();
                            textSR.sprite = lightText;
                        }
                        break;

                    case (PlayerController.AttackType.Dark):
                        if (player.darkUnlocked && player.modeLocked)
                        {
                            DLUIScripts[0].TurnOff();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[2].TurnOff();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOff();
                            DLUIScripts[5].TurnOff();
                            DLUIScripts[6].TurnOff();
                            DLUIScripts[7].TurnOn();
                            DLUIScripts[8].TurnOff();
                            textSR.sprite = darkText;
                        }
                        else if (player.darkUnlocked && !player.lightUnlocked)
                        {
                            DLUIScripts[0].TurnOff();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[2].TurnOff();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOn();
                            DLUIScripts[5].TurnOff();
                            DLUIScripts[6].TurnOff();
                            DLUIScripts[7].TurnOff();
                            DLUIScripts[8].TurnOff();
                            textSR.sprite = darkText;
                        }
                        else
                        {
                            DLUIScripts[0].TurnOff();
                            DLUIScripts[1].TurnOff();
                            DLUIScripts[2].TurnOn();
                            DLUIScripts[3].TurnOff();
                            DLUIScripts[4].TurnOff();
                            DLUIScripts[5].TurnOff();
                            DLUIScripts[6].TurnOff();
                            DLUIScripts[7].TurnOff();
                            DLUIScripts[8].TurnOff();
                            textSR.sprite = darkText;
                        }
                        break;
                }
            }
        }
    }

    void InstantiateDarkLightPrefabs(GameObject _UIPrefab, int index)
    {
        GameObject temp = Instantiate(_UIPrefab);
        temp.transform.SetParent(darkLightParent, false);
        darkLightPrefabs[index] = temp;
        darkLightPrefabs[index].SetActive(false);
    }
}
