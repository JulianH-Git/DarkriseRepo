using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class DashUpgrade : MonoBehaviour
{
    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    List<SpotlightPrefab> lowerSpotlights = new List<SpotlightPrefab>();

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
            controller.canDash = true;
            foreach (SpotlightPrefab spot in lowerSpotlights) 
            {
                spot.state = SpotlightStates.Yellow;
            }

            foreach (GameObject insturct in onboarding)
            {
                insturct.SetActive(true);
            }
        }

        this.gameObject.SetActive(false);

    }
}
