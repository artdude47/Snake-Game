using UnityEngine;
using TMPro;

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

    private void Start()
    {
        //Set the initial difficulty text
        UpdateDifficultyText();
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

    private void UpdateDifficultyText()
    {
        difficultyText.text = difficultyLevels[currentDifficultyIndex];
    }

    public void OnConfirm()
    {
        snakeController.UpdateMoveRate(difficultyLevels[currentDifficultyIndex]);
        uiManager.CloseSettings();
    }
}
