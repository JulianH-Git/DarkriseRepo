using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Area")]

    [SerializeField] private MusicArea area;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Player"))
        {
            AudioManager.instance.SetMusicArea(area);
            AudioManager.instance.musicVolume = 1f;
        }
    }
}
