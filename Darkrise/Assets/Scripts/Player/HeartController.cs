using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    PlayerController player;

    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParent;
    public GameObject heartContainerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        int heartCount = PlayerController.Instance.maxHealth / 4;
        heartContainers = new GameObject[heartCount];
        heartFills = new Image[heartCount];
        InstantiateHeartContainers();
        UpdateHeartsHUD();
        PlayerController.Instance.onHealthChangedCallback += UpdateHeartsHUD;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < PlayerController.Instance.maxHealth / 4)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void SetFilledHearts()
    {
        int health = PlayerController.Instance.Health;
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (health >= 4)
            {
                heartFills[i].fillAmount = 1;
                health -= 4;
            }
            else
            {
                heartFills[i].fillAmount = health / 4f;
                health = 0;
            }
        }
    }

    void InstantiateHeartContainers()
    {
        for (int i = 0; i < PlayerController.Instance.maxHealth / 4; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("Heart_Fill").GetComponent<Image>();
        }
    }

    void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }
}
