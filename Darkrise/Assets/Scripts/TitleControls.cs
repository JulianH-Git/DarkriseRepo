using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleControls : MonoBehaviour
{
    private bool hasPressed = false;

    [SerializeField]
    SpriteRenderer fade;

    float alpha = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !hasPressed) 
        {
            hasPressed = true;
        }

        if (hasPressed) 
        {
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alpha);
            alpha += 0.05f;
        }

        if (alpha >= 2) 
        {
            SceneManager.LoadScene("LDTKTest");
        }
    }
}
