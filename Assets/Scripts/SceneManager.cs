using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [Header("Game Modes")]
    public List<GameMode> gameModes = new();

    [Header("UI Elements")]
    [SerializeField] TMP_Text levelInfoText;
    [SerializeField] GameObject startButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            GameManager.instance.SendMessage("StartGame");
        }
    }

    public void ShowLevelInfo(int level)
    {
        levelInfoText.text = gameModes[level].modeDescription;
        startButton.SetActive(true);
    }

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
