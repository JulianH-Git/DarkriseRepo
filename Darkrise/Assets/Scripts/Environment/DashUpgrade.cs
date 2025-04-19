using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class DashUpgrade : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private ForcedEncounterManager FEM;
    [SerializeField] List<GameObject> onboarding = new List<GameObject>();


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
            FEM.ActivateForcedEncounterTutorial();

            foreach (GameObject instruct in onboarding)
            {
                instruct.SetActive(true);
            }
        }

        this.gameObject.SetActive(false);

    }
}
