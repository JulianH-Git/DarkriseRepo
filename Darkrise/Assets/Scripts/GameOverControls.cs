using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverControls : MonoBehaviour
{
    private bool hasPressed = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !hasPressed) 
        {
            hasPressed = true;
        }

        if (hasPressed) 
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }
}
