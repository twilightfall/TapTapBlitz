using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public enum ButtonPrompt { RED, BLUE, GREEN, YELLOW };

public class GameManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public RectTransform prompter;
    public GameObject retryButton;

    [Header("Text Display")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text promptText;
    [SerializeField] TMP_Text timerText;

    [SerializeField]
    RectTransform pauseMenu;

    [Header("Debug")]
    [SerializeField]
    TMP_Text debugTimer;
    [SerializeField]
    float timer;

    [SerializeField]
    float countdown = 3;

    float gameTimer = 2f;

    int score = 0;
    string textPrompt;
    ButtonPrompt promptColor;
    
    private IEnumerator countdownCoroutine;

    [SerializeField]
    bool gameStarted = false;

    InputAction backAction;

    public static GameManager instance;

    GameMode currentMode;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        EnhancedTouchSupport.Enable();

        countdownCoroutine = StartCountdown();
    }

    void Start()
    {
        StartCoroutine(countdownCoroutine);
        backAction = InputSystem.actions.FindAction("Back");
        backAction.performed += PauseGame;
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

    public void LoadGameModeData(GameMode mode)
    {
        currentMode = mode;

    }

    void Initialize()
    {
        gameTimer = currentMode.timerBase;
        StartCoroutine(countdownCoroutine);
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
        }
    }

    public void Retry()
    {
        StartCoroutine(countdownCoroutine);
        retryButton.SetActive(false);
    }

    private void StartGame()
    {
        StopCoroutine(countdownCoroutine);
        gameStarted = true;
        timerText.gameObject.SetActive(false);
        timerText.text = "";
        countdown = 3;
        timer = gameTimer;
        score = 0;
        scoreText.text = score.ToString();

        prompter.gameObject.SetActive(true);

        GridManager.instance.SpawnButtons();
        GridManager.instance.EnableButtons();

        SetPrompt();
    }

    public bool DidScore(ButtonPrompt prompt)
    {
        if(promptColor == prompt)
        {
            UpdateScore();
            timer = 1.5f;
            return true;
        }
        else
        {
            StopGame(true);
            return false;
        }
    }

    public void UpdateScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void StopGame(bool didNotScore = false)
    {
        print("stopped");
        GridManager.instance.DisableButtons();

        timerText.gameObject.SetActive(true);

        _ = didNotScore ? timerText.text = $"X" : timerText.text = $"TIME'S UP!";
        gameStarted = false;

        countdown = 3;
        timer = gameTimer;

        retryButton.SetActive(true);
    }

    public void SetPrompt()
    {
        promptColor = (ButtonPrompt)Random.Range(0, System.Enum.GetValues(typeof(ButtonPrompt)).Length);
        textPrompt = System.Enum.GetName(typeof(ButtonPrompt), (ButtonPrompt)Random.Range(0, System.Enum.GetValues(typeof(ButtonPrompt)).Length));
        
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

    public void ClosePause()
    {
        pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    //void GenerateButtons()
    //    {
    //        foreach (ButtonPrompt prompt in System.Enum.GetValues(typeof(ButtonPrompt)))
    //        {
    //            GameObject button = Instantiate(buttonPrefab, buttonPanel);
    //            button.GetComponent<Button>().SetColor(prompt);
    //            buttons.Add(button);
    //        }
    //    }

    //void RandomizeButtons()
    //    {
    //        foreach (GameObject button in buttons)
    //        {
    //            int randomIndex = Random.Range(0, buttons.Count);
    //            button.transform.SetSiblingIndex(randomIndex);
    //        }
    //        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonPanel.GetComponent<RectTransform>());
    //    }
}
