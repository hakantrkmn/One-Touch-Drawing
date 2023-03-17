using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.NextLevelButton += NextLevelButton;
    }

    private void OnDisable()
    {
        EventManager.NextLevelButton -= NextLevelButton;
    }

    private void NextLevelButton(int index)
    {
        SceneManager.LoadScene(index); 
    }
}
