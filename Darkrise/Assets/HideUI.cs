using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideUI : MonoBehaviour
{
    SpriteRenderer[] UIElements;
    Image[] imageElements;
    float alpha = 0.3f;
    void Start()
    {
        UIElements = GetComponentsInChildren<SpriteRenderer>();
        imageElements = GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(SpriteRenderer element in UIElements)
        {
            element.color = new Color(element.color.r, element.color.g, element.color.b, alpha);
        }

        foreach (Image element in imageElements)
        {
            element.color = new Color(element.color.r, element.color.g, element.color.b, alpha);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (SpriteRenderer element in UIElements)
        {
            element.color = new Color(element.color.r, element.color.g, element.color.b, 1.0f);
        }

        foreach (Image element in imageElements)
        {
            element.color = new Color(element.color.r, element.color.g, element.color.b, 1.0f);
        }
    }

}
