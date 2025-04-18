using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatueInteraction : InteractTrigger
{
    // Start is called before the first frame update

    [SerializeField] GameObject hintText;
    TextMeshPro hintTextRef;

    float fadeTimer = 3f;
    float fadeAwayPerSecond;
    float alphaValue;
    protected override void Start()
    {
        base.Start();
        hintTextRef = hintText.GetComponent<TextMeshPro>(); 
        fadeAwayPerSecond = 1 / fadeTimer;
        alphaValue = hintTextRef.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeTimer >= 0)
        {
            alphaValue -= fadeAwayPerSecond * Time.deltaTime;
            hintTextRef.color = new Color(hintTextRef.color.r, hintTextRef.color.g, hintTextRef.color.b, alphaValue);
            fadeTimer -= Time.deltaTime;
        }

        if (fadeTimer <= 0)
        {
            hintText.SetActive(false);
        }
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            indicator.SetActive(true);
            indicateColor.color = Color.green;

            if (controller.Interact())
            {
                controller.StatueRecharge();
                hintTextRef.color = new Color(hintTextRef.color.r, hintTextRef.color.g, hintTextRef.color.b, 1);
                fadeTimer = 3f;
                alphaValue = 1f;
                hintText.SetActive(true);
            }
        }
    }
}
