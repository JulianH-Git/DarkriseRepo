using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideUI : MonoBehaviour
{
    SpriteRenderer[] UIElements;
    Image[] imageElements;
    float alpha = 0.3f;
    [SerializeField] string[] tags;
    void Start()
    {
        UIElements = GetComponentsInChildren<SpriteRenderer>(true);
        imageElements = GetComponentsInChildren<Image>(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if(collision.CompareTag(tags[i]))
            {
                foreach (SpriteRenderer element in UIElements)
                {
                    element.color = new Color(element.color.r, element.color.g, element.color.b, alpha);
                }

                foreach (Image element in imageElements)
                {
                    element.color = new Color(element.color.r, element.color.g, element.color.b, alpha);
                }
                return;
            }
            
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (collision.CompareTag(tags[i]))
            {
                foreach (SpriteRenderer element in UIElements)
                {
                    element.color = new Color(element.color.r, element.color.g, element.color.b, 1.0f);
                }

                foreach (Image element in imageElements)
                {
                    element.color = new Color(element.color.r, element.color.g, element.color.b, 1.0f);
                }
                return;
            }
            
        }
    }
}
