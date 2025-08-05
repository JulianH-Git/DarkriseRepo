using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSoundMixer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audio;
    [SerializeField]
    private PlayerController player;

    private bool isInLowerArea = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
             isInLowerArea = !isInLowerArea;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInLowerArea) 
        {
            audio.volume = 0.02f;
        }
        else 
        {
            audio.volume = 0.1f;
        }
    }
}
