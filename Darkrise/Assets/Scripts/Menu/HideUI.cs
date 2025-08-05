using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideUI : MonoBehaviour
{
    SpriteRenderer[] UIElements;
    Image[] imageElements;
    float alpha = 1f;
    const float FADEALPHA = 0.3f;
    [SerializeField] string[] tags;
    [SerializeField] BoxCollider2D lightUIBox;
    bool fadeOut;
    bool fadeIn;
    void Start()
    {
        UIElements = GetComponentsInChildren<SpriteRenderer>(true);
        imageElements = GetComponentsInChildren<Image>(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeOut && alpha > FADEALPHA)
        {
            alpha -= 0.05f;
            ChangeAlpha(alpha);
        }

        if(fadeIn && alpha < 1f)
        {
            alpha += 0.05f;
            ChangeAlpha(alpha);
        }

        if(PlayerController.Instance.currentAttackType == PlayerController.AttackType.Light)
        {
            if(lightUIBox.isActiveAndEnabled == false)
            {
                lightUIBox.enabled = true;
            }
        }
        else
        {
            if (lightUIBox.isActiveAndEnabled == true)
            {
                lightUIBox.enabled = false;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (collision.CompareTag(tags[i]))
            {
                fadeOut = true;
                fadeIn = false;
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
                fadeOut = false;
                fadeIn = true;
                return;
            }
            
        }
    }

    void ChangeAlpha(float newAlpha)
    {
        foreach (SpriteRenderer element in UIElements)
        {
            element.color = new Color(element.color.r, element.color.g, element.color.b, newAlpha);
        }

        foreach (Image element in imageElements)
        {
            element.color = new Color(element.color.r, element.color.g, element.color.b, newAlpha);
        }
    }
}
