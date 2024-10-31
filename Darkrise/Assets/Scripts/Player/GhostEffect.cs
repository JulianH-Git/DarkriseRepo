using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffect : MonoBehaviour
{

    public float delay;
    private float delaySec;
    public GameObject ghostSprite;
    public bool makeGhost = false;

    [SerializeField] float ghostLife;
    // Start is called before the first frame update
    void Start()
    {
        delaySec = delay;
    }

    // Update is called once per frames
    void Update()
    {
        if (makeGhost)
        {
            if (delaySec > 0)
            {
                delaySec -= Time.deltaTime;
            }
            else
            {
                // generate ghost
                GameObject currentGhost = Instantiate(ghostSprite, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                delaySec = delay;
                Destroy(currentGhost,ghostLife);
            }
        }

    }
}
