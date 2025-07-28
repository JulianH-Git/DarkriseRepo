using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverControls : MonoBehaviour
{
    private bool hasPressed = false;
    private Player player;
    private int playerId = 0;
    // Update is called once per frame

    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    }
    void Update()
    {
        if (player.GetAnyButtonDown() && !hasPressed) 
        {
            hasPressed = true;
        }

        if (hasPressed) 
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }
}
