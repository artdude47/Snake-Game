using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton Instance
    public static GameManager instance;

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public GameState currentState;


    public int score = 0;
    public int baseScorePerFood = 10;

    public float survivalTime = 0f;

    private bool isPaused;

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

    private void Start()
    {
        currentState = GameState.MainMenu;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Playing:
                survivalTime += Time.deltaTime;
                GameEvents.TimeChanged(survivalTime);
                break;
            case GameState.Paused:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused || currentState == GameState.Playing)
            {
                TogglePause();
            }
        }
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

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
        SceneManager.LoadScene("MainMenu");
        currentState = GameState.MainMenu;
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
