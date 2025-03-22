using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkModeUpgrade : MonoBehaviour
{
    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    List<GameObject> onboarding = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            controller.darkUnlocked = true;

            foreach (GameObject insturct in onboarding)
            {
                insturct.SetActive(true);
            }
        }

        this.gameObject.SetActive(false);

    }
}
