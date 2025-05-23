using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkRoom : MonoBehaviour
{
    [SerializeField] float timeTillDamage;
    [SerializeField] int damage;
    float damageTimer;

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (PlayerController.Instance.currentAttackType == PlayerController.AttackType.Light && PlayerController.Instance.BubbleUp == false)
            {
                damageTimer += Time.deltaTime;

                if (damageTimer >= timeTillDamage)
                {
                    PlayerController.Instance.TakeDamage(damage);
                    damageTimer = 0;
                }
            }
        }

    }
}
