using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //Main Menu
    public GameObject mainMenuPanel;
    public Button playButton;
    public Button howToPlayButton;
    public Button settingsButton;

    //in game UI
    public GameObject inGamePanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeSurvivedText;
    public TextMeshProUGUI pausedText;
    public SnakeController snakeController;

    //Game over UI
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI finalTimeSurvived;
    public TextMeshProUGUI highestTimeSurvived;
    public TextMeshProUGUI highScoreGet;
    public TextMeshProUGUI highTimeGet;
    public Button retryButton;
    public Button mainMenuButton;

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
    }

    private void Start()
    {
        //Main Menu
        playButton.onClick.AddListener(StartGame);
        howToPlayButton.onClick.AddListener(ShowHowToPlay);
        settingsButton.onClick.AddListener(ShowSettings);

        //Game over UI
        retryButton.onClick.AddListener(RetryGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void StartGame()
    {
        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        inGamePanel.SetActive(true);
        highScoreGet.gameObject.SetActive(false);
        highTimeGet.gameObject.SetActive(false);

        GameManager.instance.StartGame();
        snakeController.StartGame();
    }

    private void ShowHowToPlay()
    {

    }

    private void ShowSettings()
    {

    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score.ToString()}";
    }

    public void UpdateTimeSurvived(float time)
    {
        timeSurvivedText.text = $"Time: {Mathf.RoundToInt(time).ToString()}";
    }

    private void ShowPauseStatus(bool isPaused)
    {
        pausedText.gameObject.SetActive(isPaused);
    }

    public void ShowGameOver(int finalScore, float finalTime)
    {
        inGamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        finalScoreText.text = $"Score: {finalScore.ToString()}";
        finalTimeSurvived.text = $"Time Survived: {Mathf.RoundToInt(finalTime).ToString()}";

        highScoreText.text = $"High Score: {PlayerPrefs.GetInt("HighScore")}";
        highestTimeSurvived.text = $"Highest Time: {PlayerPrefs.GetFloat("LongestSurvivalTime")}";

        //Show text if high score is acheived
        if (PlayerPrefs.GetInt("HighScore") == finalScore)
            highScoreGet.gameObject.SetActive(true);
        if (PlayerPrefs.GetFloat("LongestSurvivalTime") == finalTime)
            highTimeGet.gameObject.SetActive(true);
    }

    private void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.instance.StartGame();
    }

    private void ReturnToMainMenu()
    {
        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        GameManager.instance.ReturnToMainMenu();
    }
}
