using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite lowHealth;
    PlayerController player;
    void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
