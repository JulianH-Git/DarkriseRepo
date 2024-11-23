using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleControls : MonoBehaviour
{
    private bool hasPressed = false;

    [SerializeField]
    GameObject fade;

    Color fadeColor;

    private void Start()
    {
        fadeColor = fade.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !hasPressed) 
        {
            hasPressed = true;
        }

        if (hasPressed) 
        {
            fadeColor.a += 0.1f;
        }

        if (fadeColor.a >= 1) 
        {
            SceneManager.LoadScene("LDTKTest");
        }
    }
}
