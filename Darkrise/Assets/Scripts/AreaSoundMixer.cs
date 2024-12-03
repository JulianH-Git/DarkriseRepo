using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSoundMixer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audio;
    [SerializeField]
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            audio.volume = 0.1f;
        }
        else 
        {
            audio.volume = 0.415f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
