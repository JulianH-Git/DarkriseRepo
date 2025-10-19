using UnityEngine;

public class DarkRoom : MonoBehaviour
{
    [SerializeField] float timeTillDamage;
    [SerializeField] int damage;
    [SerializeField] float damageTimer;
    [SerializeField] bool darkOrLight; // false is dark room, true is light room
    [SerializeField] FuseBox fb;
    PlayerController controller;

    void Start()
    {
        controller = PlayerController.Instance;
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            switch (darkOrLight)
            {
                case false:
                    if (controller.currentAttackType == PlayerController.AttackType.Light
                        && controller.BubbleUp == false
                        && !controller.pState.safeFromRoomwide
                        && !controller.pState.hiding)
                    {
                        damageTimer += Time.deltaTime;

                        if (damageTimer >= (timeTillDamage))
                        {
                            controller.TakeDamage(damage);
                            damageTimer = 0;
                        }
                    }
                    else
                    {
                        damageTimer = 0;
                    }
                    break;
                case true:
                    if (controller.currentAttackType == PlayerController.AttackType.Dark
                        && !controller.pState.safeFromRoomwide
                        && !controller.pState.hiding)
                    {
                        damageTimer += Time.deltaTime;

                        if (damageTimer >= (timeTillDamage))
                        {
                            controller.TakeDamage(damage);
                            damageTimer = 0;
                        }

                    }
                    else if (controller.currentAttackType == PlayerController.AttackType.Neutral && !controller.pState.safeFromRoomwide && !controller.pState.hiding)
                    {
                        damageTimer += Time.deltaTime;

                        if (damageTimer >= timeTillDamage + 3)
                        {
                            controller.TakeDamage(damage);
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

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            damageTimer = 0;
        }
    }

    void Update()
    {
        if (fb != null)
        {
            if (fb.overloaded == true || fb.flashbanged == true)
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
