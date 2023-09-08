using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class MainMenuUI
{
    [SerializeField]
    private GameObject mainMenuPanel;
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button howToPlayButton;
    [SerializeField]
    private Button settingsButton;

    public void Initialize(Action onPlay, Action onHowToPlay, Action onSettings)
    {
        playButton.onClick.AddListener(() => onPlay());
        howToPlayButton.onClick.AddListener(() => onHowToPlay());
        settingsButton.onClick.AddListener(() => onSettings());
    }

    public void Show()
    {
        mainMenuPanel.SetActive(true);
    }

    public void Hide()
    {
        mainMenuPanel.SetActive(false);
    }
}

[System.Serializable]
public class HowToPlayUI
{
    [SerializeField]
    private GameObject howToPlayPanel;
    [SerializeField]
    private Button backButton;

    public void Initialize(Action onBackButton)
    {
        backButton.onClick.AddListener(() => onBackButton());
    }

    public void Show()
    {
        howToPlayPanel.SetActive(true);
    }

    public void Hide()
    {
        howToPlayPanel.SetActive(false);
    }
}

[System.Serializable]
public class SettingsUI
{
    [SerializeField]
    private GameObject settingsPanel;
    [SerializeField]
    private SettingsManager settingsManager;

    public void Show()
    {
        settingsPanel.SetActive(true);
    }

    public void Hide()
    {
        settingsPanel.SetActive(false);
    }
}

[System.Serializable]
public class InGameUI
{
    [SerializeField]
    private GameObject inGamePanel;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI timeSurvivedText;
    [SerializeField]
    private TextMeshProUGUI pausedText;

    public void Show()
    {
        inGamePanel.SetActive(true);
    }

    public void Hide()
    {
        inGamePanel.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateTimeSurvived(float time)
    {
        timeSurvivedText.text = $"Time: {Mathf.RoundToInt(time)}";
    }

    public void ShowPauseStatus(bool isPaused)
    {
        pausedText.gameObject.SetActive(isPaused);
    }
}

[System.Serializable]
public class GameOverUI
{
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private TextMeshProUGUI finalScoreText;
    [SerializeField]
    private TextMeshProUGUI highScoreText;
    [SerializeField]
    private TextMeshProUGUI finalTimeSurvived;
    [SerializeField]
    private TextMeshProUGUI highestTimeSurvived;
    [SerializeField]
    private TextMeshProUGUI highScoreGet;
    [SerializeField]
    private TextMeshProUGUI highTimeGet;
    [SerializeField]
    private Button retryButton;
    [SerializeField]
    private Button mainMenuButton;

    public void Initialize(Action onRetryButton, Action onMainMenuButton)
    {
        retryButton.onClick.AddListener(() => onRetryButton());
        mainMenuButton.onClick.AddListener(() => onMainMenuButton());
    }

    public void Show()
    {
        gameOverPanel.SetActive(true);
    }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
    }

    public void UpdateText(int score, float time)
    {
        finalScoreText.text = $"You scored {score}";
        finalTimeSurvived.text = $"You survived {Mathf.RoundToInt(time)} seconds";
        var highScore = PlayerPrefs.GetInt("HighScore");
        var highTime = PlayerPrefs.GetFloat("LongestSurvivalTime");

        highScoreText.text = $"High score is: {highScore}";
        highestTimeSurvived.text = $"Longest survived is: {Mathf.RoundToInt(highTime)} seconds";

        CheckHighScoreGet(score, highScore);
        CheckHighTimeGet(time, highTime);
    }

    private void CheckHighScoreGet(int score, int highScore)
    {
        if (score == highScore)
            highScoreGet.gameObject.SetActive(true);
        else
            highScoreGet.gameObject.SetActive(false);
    }

    private void CheckHighTimeGet(float time, float highTime)
    {
        if (time == highTime)
            highTimeGet.gameObject.SetActive(true);
        else
            highTimeGet.gameObject.SetActive(false);
    }
}

public enum UIContext
{
    MainMenu,
    InGame,
    HowToPlay,
    Settings,
    GameOver
}

public class UIManager : MonoBehaviour
{
    public MainMenuUI mainMenuUI;
    public HowToPlayUI howToPlayUI;
    public SettingsUI settingsUI;
    public InGameUI inGameUI;
    public GameOverUI gameOverUI;

    public event Action OnStartGameRequest;
    
    private void OnEnable()
    {
        GameEvents.OnScoreChange += UpdateScore;
        GameEvents.OnTimeChange += UpdateTimeSurvived;
        GameEvents.OnGamePause += ShowPauseStatus;
        GameEvents.OnGameOver += ShowGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnScoreChange -= UpdateScore;
        GameEvents.OnTimeChange -= UpdateTimeSurvived;
        GameEvents.OnGamePause -= ShowPauseStatus;
        GameEvents.OnGameOver -= ShowGameOver;
    }
    
    private void Start()
    {
        mainMenuUI.Initialize(StartGame, ShowHowToPlay, ShowSettings);
        howToPlayUI.Initialize(BackToMenu);
        gameOverUI.Initialize(RetryGame, ReturnToMainMenu);
        SetUIContext(UIContext.MainMenu);
    }

    public void SetUIContext(UIContext context)
    {
        DeactivateAllPanels();

        switch(context)
        {
            case UIContext.MainMenu:
                mainMenuUI.Show();
                break;

            case UIContext.InGame:
                inGameUI.Show();
                break;

            case UIContext.HowToPlay:
                howToPlayUI.Show();
                break;

            case UIContext.Settings:
                settingsUI.Show();
                break;

            case UIContext.GameOver:
                gameOverUI.Show();
                break;
            default:
                Debug.LogError($"UI Context {context} not handled!");
                break;
        }
    }

    private void DeactivateAllPanels()
    {
        mainMenuUI.Hide();
        inGameUI.Hide();
        howToPlayUI.Hide();
        settingsUI.Hide();
        gameOverUI.Hide();
    }

    private void StartGame()
    {
        SetUIContext(UIContext.InGame);
        OnStartGameRequest?.Invoke();
    }

    private void ShowHowToPlay()
    {
        SetUIContext(UIContext.HowToPlay);
    }

    private void ShowSettings()
    {
        SetUIContext(UIContext.Settings);
    }

    public void CloseSettings()
    {
        SetUIContext(UIContext.MainMenu);
    }

    private void BackToMenu()
    {
        SetUIContext(UIContext.MainMenu);
    }

    public void UpdateScore(int score)
    {
        inGameUI.UpdateScore(score);
    }

    public void UpdateTimeSurvived(float time)
    {
        inGameUI.UpdateTimeSurvived(time);
    }

    private void ShowPauseStatus(bool isPaused)
    {
        inGameUI.ShowPauseStatus(isPaused);
    }

    public void ShowGameOver(int finalScore, float finalTime)
    {
        SetUIContext(UIContext.GameOver);
        gameOverUI.UpdateText(finalScore, finalTime);
    }

    private void RetryGame()
    {
        SetUIContext(UIContext.InGame);
        GameManager.instance.RetryGame();
    }

    private void ReturnToMainMenu()
    {
        SetUIContext(UIContext.MainMenu);
        GameManager.instance.ReturnToMainMenu();
    }
}
