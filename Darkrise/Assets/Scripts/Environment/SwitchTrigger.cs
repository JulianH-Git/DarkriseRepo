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

    // Start is called before the first frame update
    void Start()
    {
        controller = player.GetComponent<PlayerController>();
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

            if (controller.Interact())
            {
                foreach (GameObject sprite in affectedSprites)
                {
                    sprite.SetActive(false);
                    hasBeenUsed = true;
                    indicateColor.color = Color.white;
                }
                this.gameObject.transform.rotation = new Quaternion(-1, 0, 0, 0);
            }
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            indicateColor.color = Color.white;
        }
    }
}
