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

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        darkLightPrefabs = new GameObject[3];
        InstantiateDarkLightPrefabs(neutralModePrefab, 0); // 0 = neutral
        InstantiateDarkLightPrefabs(lightModePrefab, 1); // 1 = light
        InstantiateDarkLightPrefabs(darkModePrefab, 2); // 2 = dark
    }

    // Update is called once per frame
    void Update()
    {
        if (darkLightPrefabs != null)
        {
            switch (player.currentAttackType)
            {
                case (PlayerController.AttackType.Neutral):
                    darkLightPrefabs[0].SetActive(true);
                    darkLightPrefabs[1].SetActive(false);
                    darkLightPrefabs[2].SetActive(false);
                    break;

                case (PlayerController.AttackType.Light):
                    darkLightPrefabs[0].SetActive(false);
                    darkLightPrefabs[1].SetActive(true);
                    darkLightPrefabs[2].SetActive(false);
                    break;

                case (PlayerController.AttackType.Dark):
                    darkLightPrefabs[0].SetActive(false);
                    darkLightPrefabs[1].SetActive(false);
                    darkLightPrefabs[2].SetActive(true);
                    break;
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
