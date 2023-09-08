using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton Pattern
    public static GameManager instance;
    private void Awake()
    {
        //implement singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    #region Enums
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
    public GameState currentState;
    #endregion

    #region Game Components
    [SerializeField]
    private SnakeController snakeController;
    [SerializeField]
    private FoodManager foodManager;
    [SerializeField]
    private SettingsManager settingsManager;
    [SerializeField]
    private UIManager uiManager;
    #endregion

    #region Game Stats
    public int score = 0;
    public int baseScorePerFood = 10;
    public float survivalTime = 0f;
    #endregion 

    private bool isPaused;

    private void Start()
    {
        currentState = GameState.MainMenu;
        settingsManager.OnSettingsChanged += snakeController.HandleSettingsChange;
        settingsManager.OnRainbowColorChanged += snakeController.UpdateRainbowColor;
        uiManager.OnStartGameRequest += HandleGameStartRequest;
        InitializeGame();
    }

    private void InitializeGame()
    {
        foodManager.Initialize(GridManager.instance.gridSize, snakeController);
        snakeController.Initialize(GridManager.instance.gridSize, foodManager);
    }

    private void OnDestroy()
    {
        settingsManager.OnSettingsChanged -= snakeController.HandleSettingsChange;
        settingsManager.OnRainbowColorChanged -= snakeController.UpdateRainbowColor;
        uiManager.OnStartGameRequest -= HandleGameStartRequest;
    }

    private void Update()
    {
        HandleGameStateUpdates();
        HandleEscapeKey();
    }

    private void HandleGameStateUpdates()
    {
        if(currentState == GameState.Playing)
        {
            survivalTime += Time.deltaTime;
            GameEvents.TimeChanged(survivalTime);
        }
    }

    private void HandleEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (currentState == GameState.Paused || currentState == GameState.Playing))
            TogglePause();
    }

    private void HandleGameStartRequest()
    {
        StartGame();
        snakeController.StartGame();
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        currentState = isPaused ? GameState.Paused : GameState.Playing;
        GameEvents.GamePaused(isPaused);
    }

    public void GameOver()
    {
        currentState = GameState.GameOver;

        //Save high score and longest survival if they are surpassed
        SaveHighScore();
        SaveLongestSurvivalTime();
        GameEvents.GameIsOver(score, survivalTime);
    }

    public void IncreaseScore(int snakeLength)
    {
        //Score increases as snake gets longer
        int scoreToAdd = baseScorePerFood * snakeLength;
        score += scoreToAdd;

        GameEvents.ScoreChanged(score);
    }

    public void ReturnToMainMenu()
    {
        ResetStats();
        currentState = GameState.MainMenu;
    }

    public void RetryGame()
    {
        currentState = GameState.Playing;
        ResetStats();
    }

    private void SaveHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }

    private void SaveLongestSurvivalTime()
    {
        float currentLongestTime = PlayerPrefs.GetFloat("LongestSurvivalTime", 0f);
        if (survivalTime > currentLongestTime)
        {
            PlayerPrefs.SetFloat("LongestSurvivalTime", survivalTime);
            PlayerPrefs.Save();
        }
    }

    private void ResetStats()
    {
        score = 0;
        survivalTime = 0;
        snakeController.ResetSnake();
    }
}

public static class GameEvents
{
    public delegate void UpdateScoreEvent(int score);
    public delegate void UpdateTimeEvent(float time);
    public delegate void GamePauseEvent(bool isPaused);
    public delegate void GameOver(int score, float time);

    public static event UpdateScoreEvent OnScoreChange;
    public static event UpdateTimeEvent OnTimeChange;
    public static event GamePauseEvent OnGamePause;
    public static event GameOver OnGameOver;

    public static void ScoreChanged(int score)
    {
        OnScoreChange?.Invoke(score);
    }

    public static void TimeChanged(float time)
    {
        OnTimeChange?.Invoke(time);
    }

    public static void GamePaused(bool paused)
    {
        OnGamePause?.Invoke(paused);
    }

    public static void GameIsOver(int score, float time)
    {
        OnGameOver?.Invoke(score, time);
    }
}
