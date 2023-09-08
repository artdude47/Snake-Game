using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TextMeshProUGUI difficultyText;
    [SerializeField]
    private Image colorDisplay;
    [SerializeField]
    private UIManager uiManager;

    [Header("Configuration")]
    public List<string> difficultyLevels = new List<string>() { "Easy", "Normal", "Hard" };
    public List<Color> snakeColors;
    private Color rainbowFlagColor = Color.black;

    private int currentDifficultyIndex = 1; //normal is default
    public Color CurrentRainbowColor { get; private set; }
    private Tween rainbowTween;

    [System.Serializable]
    public struct ColorUnlock
    {
        public int scoreRequired;
        public Color color;
    }
    public List<ColorUnlock> colorUnlock = new List<ColorUnlock>();

    private int currentColorIndex = 0;
    private Color snakeColor = Color.white;

    public delegate void SettingsChangeDelegate(string difficulty, Color snakeColor);
    public event SettingsChangeDelegate OnSettingsChanged;
    public delegate void RainbowColorChangeDelegate(Color currentRainbowColor);
    public event RainbowColorChangeDelegate OnRainbowColorChanged;

    private void Start()
    {
        //Set the initial difficulty text
        UpdateDifficultyText();
        UpdateSnakeColor();

        RefreshColors();
    }

    public void IncreaseDifficulty()
    {
        currentDifficultyIndex = Mathf.Clamp(currentDifficultyIndex + 1, 0, difficultyLevels.Count - 1);
        UpdateDifficultyText();
    }

    public void DecreaseDifficulty()
    {
        currentDifficultyIndex = Mathf.Clamp(currentDifficultyIndex - 1, 0, difficultyLevels.Count - 1);
        UpdateDifficultyText();
    }

    public void PreviousColor()
    {
        currentColorIndex = ((currentColorIndex - 1) + snakeColors.Count) % snakeColors.Count;
        UpdateSnakeColor();
    }

    public void NextColor()
    {
        currentColorIndex = (currentColorIndex + 1) % snakeColors.Count;
        UpdateSnakeColor();
    }

    private void UpdateSnakeColor()
    {
        snakeColor = snakeColors[currentColorIndex];

        if (snakeColor == rainbowFlagColor)
        {
            StartRainbowEffect();
        }
        else
        {
            StopRainbowEffect();
            colorDisplay.color = snakeColor;
        }

    }

    private void StartRainbowEffect()
    {
        StopRainbowEffect();

        rainbowTween = DOTween.Sequence()
            .Append(colorDisplay.DOColor(Color.red, 0.9f))
            .Append(colorDisplay.DOColor(Color.yellow, 0.9f))
            .Append(colorDisplay.DOColor(Color.blue, 0.9f))
            .Append(colorDisplay.DOColor(Color.white, 0.9f))
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear)
            .OnKill(() => colorDisplay.color = snakeColor);

        rainbowTween.OnUpdate(() =>
        {
            CurrentRainbowColor = colorDisplay.color;
            OnRainbowColorChanged?.Invoke(CurrentRainbowColor);
        });
    }

    private void StopRainbowEffect()
    {
        if(rainbowTween != null)
        {
            rainbowTween.Kill();
        }
    }

    private void UpdateDifficultyText()
    {
        difficultyText.text = difficultyLevels[currentDifficultyIndex];
    }

    public void OnConfirm()
    {
        string chosenDifficulty = difficultyLevels[currentDifficultyIndex];
        OnSettingsChanged?.Invoke(chosenDifficulty, snakeColor);
        uiManager.CloseSettings();
    }

    public void RefreshColors()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        foreach (ColorUnlock unlock in colorUnlock)
        {
            if (highScore >= unlock.scoreRequired)
            {
                if (unlock.color == Color.black)
                    snakeColors.Add(rainbowFlagColor);
                else
                    snakeColors.Add(unlock.color);
            }
        }
    }
}
