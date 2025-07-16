using UnityEngine;

public class DarkLightUIController : MonoBehaviour
{
    PlayerController player;
    [SerializeField] Transform darkLightParent;
    private GameObject[] darkLightPrefabs;
    private LightCooldownUpdater[] lightCooldownUpdater;
    PlayerController.AttackType currentAttackType;
    PlayerController.EquippedLightSpell spell;
    [SerializeField] GameObject lightBubbleCooldown;
    [SerializeField] GameObject remoteFlashbangCooldown;
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
        darkLightPrefabs = new GameObject[2];
        lightCooldownUpdater = new LightCooldownUpdater[2];

        InstantiateDarkLightPrefabs(lightBubbleCooldown, 0);
        InstantiateDarkLightPrefabs(remoteFlashbangCooldown, 1);

        currentAttackType = player.currentAttackType;
        spell = player.currentLightSpell;

        textSR = text.GetComponent<SpriteRenderer>();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.currentAttackType != currentAttackType || player.currentLightSpell != spell)
        {
            UpdateUI();
            currentAttackType = player.currentAttackType; // Update stored attack type
            spell = player.currentLightSpell;
        }
    }

    void UpdateUI()
    {
        currentAttackType = player.currentAttackType; // Update stored attack type
        spell = player.currentLightSpell;
        switch (currentAttackType)
        {
            case PlayerController.AttackType.Neutral:
                textSR.sprite = neutralText;
                EnableCooldownMenu(false);
                break;
            case PlayerController.AttackType.Dark:
                textSR.sprite = darkText;
                EnableCooldownMenu(false);
                break;
            case PlayerController.AttackType.Light:
                textSR.sprite = lightText;
                EnableCooldownMenu(true);
                break;
        }

        lightCooldownUpdater[0].UpdateRefills();
        lightCooldownUpdater[1].UpdateRefills();
    }

    void EnableCooldownMenu(bool yn)
    {
        if(yn)
        {
            if(spell == PlayerController.EquippedLightSpell.LightBubble)
            {
                lightCooldownUpdater[0].TurnOn();
                lightCooldownUpdater[1].TurnOff();
            }
            else
            {
                lightCooldownUpdater[0].TurnOff();
                lightCooldownUpdater[1].TurnOn();
            }
        }
        else
        {
            lightCooldownUpdater[0].TurnOff();
            lightCooldownUpdater[1].TurnOff();
        }

    }
    void InstantiateDarkLightPrefabs(GameObject _UIPrefab, int index)
    {
        GameObject temp = Instantiate(_UIPrefab);
        temp.transform.SetParent(darkLightParent, false);
        darkLightPrefabs[index] = temp;
        darkLightPrefabs[index].SetActive(false);
        lightCooldownUpdater[index] = temp.GetComponentInChildren<LightCooldownUpdater>();
    }
}
