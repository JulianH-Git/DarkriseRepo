using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkRoom : MonoBehaviour
{
    [SerializeField] float timeTillDamage;
    [SerializeField] int damage;
    float damageTimer;
    [SerializeField] bool darkOrLight; // false is dark room, true is light room
    [SerializeField] FuseBox fb;

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !collision.isTrigger)
        {
            switch(darkOrLight)
            {
                case false:
                    if (PlayerController.Instance.currentAttackType == PlayerController.AttackType.Light 
                        && PlayerController.Instance.BubbleUp == false
                        && !PlayerController.Instance.pState.safeFromRoomwide)
                    {
                        damageTimer += Time.deltaTime;

                        if (damageTimer >= timeTillDamage)
                        {
                            PlayerController.Instance.TakeDamage(damage);
                            damageTimer = 0;
                        }
                    }
                    else
                    {
                        damageTimer = 0;
                    }
                        break;
                case true:
                    if (PlayerController.Instance.currentAttackType == PlayerController.AttackType.Dark
                        && !PlayerController.Instance.pState.safeFromRoomwide)
                    {
                        damageTimer += Time.deltaTime;

                        if (damageTimer >= timeTillDamage)
                        {
                            PlayerController.Instance.TakeDamage(damage);
                            damageTimer = 0;
                        }
                    }
                    else if (PlayerController.Instance.currentAttackType == PlayerController.AttackType.Neutral && !PlayerController.Instance.pState.safeFromRoomwide) 
                    {
                        damageTimer += Time.deltaTime;

                        if (damageTimer >= timeTillDamage + 2)
                        {
                            PlayerController.Instance.TakeDamage(damage);
                            damageTimer = 0;
                        }
                    }
                    else
                    {
                        damageTimer = 0;
                    }
                    break;
            }
            
        }

    }

    void Update()
    {
        if(fb != null)
        {
            if(fb.overloaded == true || fb.flashbanged == true)
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
