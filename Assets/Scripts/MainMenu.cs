using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Game Modes")]
    public List<GameMode> gameModes = new();

    [Header("UI Elements")]
    [SerializeField] TMP_Text levelInfoText;
    [SerializeField] GameObject startButton;

    GameMode selectedMode;

    private void OnEnable()
    {
        foreach (GameMode mode in gameModes)
        {
            mode.ResetMode();
        }
    }

    public void ShowLevelInfo(int level)
    {
        levelInfoText.text = gameModes[level].modeDescription;
        selectedMode = gameModes[level];
        startButton.SetActive(true);
    }

    public void StartGame()
    {
        selectedMode.isActive = true;
        SceneManager.LoadScene(1);
    }
}
