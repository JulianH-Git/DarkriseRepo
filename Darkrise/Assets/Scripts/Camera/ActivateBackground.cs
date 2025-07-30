using LDtkUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActivateBackground : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] GameObject target;
    [SerializeField] GameObject background;

    Image backgroundImage;
    float alpha;

    private void Start()
    {
        backgroundImage = background.GetComponent<Image>();
    }
    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == target)
        {
            if(alpha <= 0.5f)
            {
                alpha += 0.01f;
            }
            else
            {
                alpha = 0.5f;
            }
        }
        else
        {
            if (alpha >= 0.5f)
            {
                alpha -= 0.01f;
            }
            else
            {
                alpha = 0.0f;
            }
        }

        backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, alpha);
    }
}
