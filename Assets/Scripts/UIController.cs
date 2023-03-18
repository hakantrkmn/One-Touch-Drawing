using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject interactionPanel;
    public GameObject gamePanel;


    private void OnEnable()
    {
        EventManager.LevelWin += LevelWin;
    }

    private void OnDisable()
    {
        EventManager.LevelWin -= LevelWin;
    }

    private void LevelWin()
    {
        winPanel.SetActive(true);
        interactionPanel.SetActive(false);
        gamePanel.SetActive(false);
    }
}
