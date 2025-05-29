using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBlock : MonoBehaviour
{
    BoxCollider2D bx;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        bx = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(bx, collision, false);
            sr.sortingOrder = 0;
        }
    }
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if(PlayerController.Instance.currentAttackType == PlayerController.AttackType.Dark)
            {
                Physics2D.IgnoreCollision(bx, collision, true);
                sr.sortingOrder = 2;
            }
            else
            {
                Physics2D.IgnoreCollision(bx, collision, false);
                sr.sortingOrder = 0;
            }
        }
    }
}
