using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

public enum ButtonPrompt { RED, BLUE, GREEN, YELLOW };

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject buttonPrefab;
    public RectTransform prompter;
    public GameObject retryButton;
    public Transform gameGrid;

    [Header("Text Display")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text p2ScoreText;
    [SerializeField] TMP_Text promptText;
    [SerializeField] TMP_Text timerText;

    [Header("Pause Menu")]
    [SerializeField] RectTransform pauseMenu;
    [SerializeField] TMP_Text hintText;

    [Header("Game Modes")]
    public List<GameMode> gameModes = new();

    [Header("Game Variables")]
    float timer;
    float countdown = 3;
    int score = 0;
    string textPrompt;
    ButtonPrompt promptColor;
    bool gameStarted = false;

    [Header("Debug")]
    [SerializeField]
    TMP_Text debugTimer;
    
    private IEnumerator countdownCoroutine;

    public delegate void OnButtonTap(ButtonPrompt prompt);
    public static OnButtonTap onButtonTap;

    InputAction backAction;

    GameMode currentMode;

    void Awake()
    {
        EnhancedTouchSupport.Enable();

        countdownCoroutine = StartCountdown();
    
        backAction = InputSystem.actions.FindAction("Back");
        backAction.performed += PauseGame;

        onButtonTap += DidScore;
    }

    void Start()
    {
        Initialize();
    }

    void OnDisable()
    {
        backAction.performed -= PauseGame;
    }

    void Update()
    {
        if (timer > 0f && gameStarted)
        {
            timer -= Time.deltaTime;
            //debugTimer.text = timer.ToString();

            if (timer <= 0f)
            {
                print("time's up!");
                StopGame();
            }
        }
    }

    void Initialize()
    {
        foreach (GameMode mode in gameModes)
        {
            if (mode.isActive)
            {
                currentMode = mode;
                timer = currentMode.timerBase;

                hintText.text = currentMode.hintText; 

                GridManager.GenerateGrid(currentMode.columns, currentMode.rows, currentMode.padding);

                StartCoroutine(countdownCoroutine);
                break;
            }
        }
    }

    IEnumerator StartCountdown()
    {
        print("started");
        timerText.gameObject.SetActive(true);
        while (!gameStarted)
        {
            timerText.text = countdown.ToString("F0");
            countdown--;

            if (countdown < 0)
            {
                print("GO!");
                StartGame();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void StartGame()
    {
        StopCoroutine(countdownCoroutine);

        gameStarted = true;
        timerText.gameObject.SetActive(false);
        timerText.text = "";
        countdown = 3;
        timer = currentMode.timerBase;
        score = 0;
        scoreText.text = score.ToString();

        prompter.gameObject.SetActive(true);

        GridManager.SpawnButtons(buttonPrefab, gameGrid, currentMode.cellSize, currentMode.padding);
        GridManager.EnableButtons();

        SetPrompt();
    }

    void SetPrompt()
    {
        if (!prompter.gameObject.activeSelf) prompter.gameObject.SetActive(true);

        promptColor = (ButtonPrompt)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ButtonPrompt)).Length);
        textPrompt = Enum.GetName(typeof(ButtonPrompt), (ButtonPrompt)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ButtonPrompt)).Length));

        promptText.text = textPrompt;

        promptText.color = promptColor switch
        {
            ButtonPrompt.RED => Color.red,
            ButtonPrompt.BLUE => Color.blue,
            ButtonPrompt.GREEN => Color.green,
            ButtonPrompt.YELLOW => Color.yellow,
            _ => throw new System.NotImplementedException(),
        };

        print($"TAP {promptColor} --- NOT {textPrompt}");
    }

    void StopGame(bool didNotScore = false)
    {
        print("stopped");
        GridManager.DisableButtons();
        timerText.gameObject.SetActive(true);

        _ = didNotScore ? timerText.text = $"X" : timerText.text = $"TIME'S UP!";
        gameStarted = false;

        prompter.gameObject.SetActive(false);

        countdown = 3;
        timer = currentMode.timerBase;

        retryButton.SetActive(true);
    }

    void DidScore(ButtonPrompt prompt)
    {
        if (promptColor == prompt)
        {
            UpdateScore();
            timer = currentMode.timerBase;
            GridManager.ShuffleGrid();
            SetPrompt();
        }
        else
        {
            StopGame(true);
        }
    }

    void UpdateScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void PauseGame(InputAction.CallbackContext ctx)
    {
        if (!pauseMenu.gameObject.activeSelf)
        {
            pauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.gameObject.SetActive(false);
            Time.timeScale = 1f;

            //todo: add confirmation logic to end game here
            //SceneManager.LoadScene(0);
        }
    }

    public void ClosePause()
    {
        pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Retry()
    {
        StartCoroutine(countdownCoroutine);
        retryButton.SetActive(false);
    }
}
