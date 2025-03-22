using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    [SerializeField] protected GameObject player;
    protected PlayerController controller;

    [SerializeField] protected List<GameObject> affectedSprites;

    private bool hasBeenUsed = false;

    [SerializeField] protected SpriteRenderer indicateColor;

    [SerializeField] protected GameObject indicator;

    // Start is called before the first frame update
    void Start()
    {
        controller = player.GetComponent<PlayerController>();
        indicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && !hasBeenUsed)
        {
            indicateColor.color = Color.green;
            indicator.SetActive(true);

            if (controller.Interact())
            {
                indicator.SetActive(false);
                foreach (GameObject sprite in affectedSprites)
                {
                    sprite.SetActive(false);
                    hasBeenUsed = true;
                    indicateColor.color = Color.white;
                }
                this.GetComponent<SpriteRenderer>().flipY = true;
            }
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            indicator.SetActive(false);
            indicateColor.color = Color.white;
        }
    }
}
