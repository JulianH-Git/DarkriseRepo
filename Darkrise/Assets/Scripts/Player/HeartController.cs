using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    [SerializeField] GameObject[] heartContainers;
    [SerializeField] SpriteRenderer playerIconSR;
    [SerializeField] Sprite normalHealthIcon;
    [SerializeField] Sprite lowHealthIcon;


    // Start is called before the first frame update
    void Start()
    {
        UpdateHeartsHUD();
        PlayerController.Instance.onHealthChangedCallback += UpdateHeartsHUD;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetHealth()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < PlayerController.Instance.health)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void UpdateHeartsHUD()
    {
        SetHealth();
        UpdatePlayerIcon();
    }

    void UpdatePlayerIcon()
    {
        if (PlayerController.Instance.health <= 1)
        {
            playerIconSR.sprite = lowHealthIcon;
        }
        else
        {
            playerIconSR.sprite = normalHealthIcon;
        }
    }

}
