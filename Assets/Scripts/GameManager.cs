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
using UnityEngine.UI;

public enum ButtonPrompt { RED, BLUE, GREEN, YELLOW };

public class GameManager : MonoBehaviour
{
    [Header("Prefabs and Transforms")]
    public GameObject buttonPrefab;
    public RectTransform prompter;
    public GameObject retryButton;
    public Transform gameGrid;
    public RectTransform p1HUD;
    public RectTransform p2HUD;

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

    // GAME VARIABLES
    GameMode currentMode;
    float timer;
    float currentTimer;
    float countdown = 3;
    int score = 0;
    int p2Score = 0;
    [SerializeField]
    int currentPlayer;

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
            debugTimer.text = timer.ToString();

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

                if(currentMode.hasDecay) currentTimer = timer;

                if (mode.isMultiplayer)
                {
                    p2ScoreText.gameObject.SetActive(true);
                    scoreText.transform.Rotate(0, 0, 180);
                }

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

        if (currentMode.isMultiplayer)
        {
            p2Score = 0;
            p2ScoreText.text = p2Score.ToString();
        }

        prompter.gameObject.SetActive(true);

        GridManager.SpawnButtons(buttonPrefab, gameGrid, currentMode.cellSize, currentMode.padding);
        GridManager.EnableButtons();

        SetPrompt();
    }

    float DecayTimer()
    {
        if (score > 0 && score % currentMode.decayInterval == 0)
        {
            if (currentMode.timerBase != currentMode.timerMax)
                currentTimer =  currentMode.timerBase - currentMode.timerDecay;
            return currentTimer;
        }

        return currentTimer;
    }

    void SetPrompt()
    {
        if (!prompter.gameObject.activeSelf) prompter.gameObject.SetActive(true);

        promptColor = (ButtonPrompt)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ButtonPrompt)).Length);
        textPrompt = Enum.GetName(typeof(ButtonPrompt), (ButtonPrompt)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ButtonPrompt)).Length));

        promptText.color = promptColor switch
        {
            ButtonPrompt.RED => Color.red,
            ButtonPrompt.BLUE => Color.blue,
            ButtonPrompt.GREEN => Color.green,
            ButtonPrompt.YELLOW => Color.yellow,
            _ => throw new System.NotImplementedException(),
        };

        if (currentMode.isMultiplayer)
        {
            int nextPlayer = UnityEngine.Random.Range(0, 2);

            if (currentPlayer == 0 && nextPlayer == 1)
            {
                prompter.Rotate(0, 0, 180);
            }
            else if ((currentPlayer == 0 && nextPlayer == 0) || (currentPlayer == 1 && nextPlayer == 0))
            {
                prompter.rotation = Quaternion.identity;
            }

            currentPlayer = nextPlayer;
            SetActivePlayerBackground(currentPlayer);
        }

        promptText.text = textPrompt;
    }

    void StopGame(bool didNotScore = false)
    {
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
            UpdateScore(currentPlayer);

            if (currentMode.hasDecay)
            {
                timer = DecayTimer();
            }
            else timer = currentMode.timerBase;

            GridManager.ShuffleGrid();
            SetPrompt();
        }
        else
        {
            StopGame(true);
        }
    }

    void UpdateScore(int currentPlayer = 0)
    {
        if (currentMode.isMultiplayer)
        {
            if (currentPlayer == 0)
            {
                score++;
                p2ScoreText.text = score.ToString();
            }
            else
            {
                p2Score++;
                scoreText.text = p2Score.ToString();
            }
        }
        else
        {
            score++;
            scoreText.text = score.ToString();
        }
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
            if (ctx.performed)
            {
                StopAllCoroutines();
                SceneManager.LoadScene(0);
            }
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

    void SetActivePlayerBackground(int currentPlayer, bool isStopped = false)
    {
        if (currentPlayer == 0)
        {
            p2HUD.GetComponent<Image>().color = Color.white;
            p1HUD.GetComponent<Image>().color = Color.black;
        }
        else
        {
            p1HUD.GetComponent<Image>().color = Color.white;
            p2HUD.GetComponent<Image>().color = Color.black;
        }

        if (isStopped)
        {
            p1HUD.GetComponent<Image>().color = Color.black;
            p2HUD.GetComponent<Image>().color = Color.black;
        }
    }
}
