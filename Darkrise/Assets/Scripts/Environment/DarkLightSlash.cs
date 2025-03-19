using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class DarkLightSlash : MonoBehaviour
{
    MaterialPropertyBlock propertyBlock;
    SpriteRenderer sr;
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        sr = this.GetComponent<SpriteRenderer>();
        Color lightAttack = new Color(255.0f, 245.0f, 105.0f);

        switch (player.currentAttackType)
        {
            case (AttackType.Neutral):
                break;
            case (AttackType.Light):
                sr.color = lightAttack;
                break;

            case (AttackType.Dark):
                sr.color = Color.black;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
