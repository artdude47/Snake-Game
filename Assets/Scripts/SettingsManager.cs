using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI difficultyText;
    [SerializeField]
    private SnakeController snakeController;
    [SerializeField]
    private UIManager uiManager;

    private int currentDifficultyIndex = 1; //normal is default
    private string[] difficultyLevels = { "Easy", "Normal", "Hard" };
    public Image colorDisplay;
    private bool isRainbow = false;

    private Color rainbowFlagColor = Color.black;
    public Color CurrentRainbowColor { get; private set; }
    private Tween rainbowTween;
    public List<Color> snakeColors = new List<Color>()
    {
        Color.white,
        Color.red,
        Color.yellow,
        Color.blue
    };

    [System.Serializable]
    public struct ColorUnlock
    {
        public int scoreRequired;
        public Color color;
    }

    public List<ColorUnlock> colorUnlock = new List<ColorUnlock>();

    private int currentColorIndex = 0;
    private Color snakeColor = Color.white;

    private void Start()
    {
        //Set the initial difficulty text
        UpdateDifficultyText();
        UpdateSnakeColor();

        RefreshColors();
    }

    public void IncreaseDifficulty()
    {
        currentDifficultyIndex = Mathf.Clamp(currentDifficultyIndex + 1, 0, difficultyLevels.Length - 1);
        UpdateDifficultyText();
    }

    public void DecreaseDifficulty()
    {
        currentDifficultyIndex = Mathf.Clamp(currentDifficultyIndex - 1, 0, difficultyLevels.Length - 1);
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
            isRainbow = true;
        }
        else
        {
            StopRainbowEffect();
            colorDisplay.color = snakeColor;
            isRainbow = false;
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
        });
        snakeController.StartRainbowEffect();
    }


    private void StopRainbowEffect()
    {
        if(rainbowTween != null)
        {
            rainbowTween.Kill();
        }

        snakeController.StopRainbowEffect();
    }

    private void UpdateDifficultyText()
    {
        difficultyText.text = difficultyLevels[currentDifficultyIndex];
    }

    public void OnConfirm()
    {
        snakeController.UpdateMoveRate(difficultyLevels[currentDifficultyIndex]);
        snakeController.UpdateColor(snakeColor);
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
