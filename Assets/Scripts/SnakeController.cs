using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SnakeController : MonoBehaviour
{
    private FoodManager foodManager;

    [Header("Settings")]
    [SerializeField] private GameObject snakeBodyPrefab;
    [SerializeField] private Color snakeColor = Color.white;

    [Header("Runtime")]
    private Vector2Int currentDirection = Vector2Int.right;
    private Vector2Int bufferedDirection;
    private bool directionBuffered = false;
    private bool hasMoved = true;
    private Vector2Int currentPos;
    private bool isRainbowEffectActive = false;
    private float moveRate = 0.35f;
    private Vector2Int gridSize;

    public List<Transform> bodyParts = new List<Transform>();

    public void Initialize(Vector2Int gridSize, FoodManager foodManager)
    {
        this.gridSize = gridSize;
        this.foodManager = foodManager;
        InitializeSnake();
    }

    private void InitializeSnake()
    {
        bodyParts.Add(this.transform);
        currentPos = new Vector2Int(gridSize.x / 2, gridSize.y / 2);
        transform.position = new Vector3(currentPos.x, currentPos.y, 0);
    }

    private void UpdateMoveRate(string difficulty)
    {
        float[] rates = { 0.65f, 0.35f, 0.25f };
        string[] difficulties = { "Easy", "Normal", "Hard" };
        int index = System.Array.IndexOf(difficulties, difficulty);
        moveRate = rates[index];
    }

    public void StartGame()
    {
        StopAllCoroutines();
        StartCoroutine(Move());
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Playing)
            return;

        Vector2Int newDirection = GetDirectionFromInput();

        if (newDirection != Vector2.zero && newDirection != currentDirection)
        {
            if (hasMoved)
            {
                currentDirection = newDirection;
                hasMoved = false;
            }
            else
            {
                bufferedDirection = newDirection;
                directionBuffered = true;
            }
        }
    }

    private Vector2Int GetDirectionFromInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) return Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) return Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) return Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) return Vector2Int.right;
        return Vector2Int.zero;
    }

    private IEnumerator Move()
    {
        while (GameManager.instance.currentState == GameManager.GameState.Playing)
        {
            yield return new WaitForSeconds(moveRate);
            PerformMovement();
            CheckCollisions();
            HandleFoodConsumption();
            hasMoved = true;
            ApplyBufferedDirection();
        }
    }

    private void PerformMovement()
    {
        Vector2Int previousPos = currentPos;
        currentPos += currentDirection;

        // Update head position
        bodyParts[0].position = new Vector3(currentPos.x, currentPos.y, bodyParts[0].position.z);

        // Update body positions
        for (int i = 1; i < bodyParts.Count; i++)
        {
            Vector2Int tempPos = new Vector2Int((int)bodyParts[i].position.x, (int)bodyParts[i].position.y);
            bodyParts[i].position = new Vector3(previousPos.x, previousPos.y, bodyParts[i].position.z);
            previousPos = tempPos;
        }
    }

    private void CheckCollisions()
    {
        // Check boundary collisions
        if (currentPos.x < 0 || currentPos.x >= gridSize.x || currentPos.y < 0 || currentPos.y >= gridSize.y)
        {
            GameOver();
            return;
        }

        // Check for self collisions
        for (int i = 1; i < bodyParts.Count; i++)
        {
            if ((Vector2)bodyParts[i].position == (Vector2)currentPos)
            {
                GameOver();
                return;
            }
        }
    }

    private void HandleFoodConsumption()
    {
        if (foodManager.IsFoodAtPosition(currentPos))
        {
            Grow();
            foodManager.SpawnFood();
        }
    }

    private void ApplyBufferedDirection()
    {
        if (!directionBuffered) return;
        if (!IsOppositeDirection(bufferedDirection, currentDirection))
        {
            currentDirection = bufferedDirection;
        }
        directionBuffered = false;
    }

    private bool IsOppositeDirection(Vector2Int dir1, Vector2Int dir2) => dir1 == -dir2;

    private void UpdateColor(Color color)
    {
        snakeColor = color;
        foreach (Transform body in bodyParts)
        {
            body.GetComponent<SpriteRenderer>().color = snakeColor;
        }
    }

    public void ToggleRainbowEffect(bool state) => isRainbowEffectActive = state;

    public void ResetSnake()
    {
        //reset direction to right
        currentDirection = new Vector2Int(1, 0);

        //reset snake position to center
        Vector2Int startingPosition = new Vector2Int(gridSize.x / 2, gridSize.y / 2);
        bodyParts[0].position = new Vector3(startingPosition.x, startingPosition.y, 0);
        currentPos = startingPosition;

        //reset snake size
        while (bodyParts.Count > 1)
        {
            Destroy(bodyParts[bodyParts.Count - 1].gameObject);
            bodyParts.RemoveAt(bodyParts.Count - 1);
        }

        StartGame();
    }

    public void Grow()
    {
        Transform newPart = Instantiate(snakeBodyPrefab, bodyParts[bodyParts.Count - 1].position, Quaternion.identity).transform;
        newPart.GetComponent<SpriteRenderer>().color = snakeColor;
        bodyParts.Add(newPart);

        //Update score in GameManager
        GameManager.instance.IncreaseScore(bodyParts.Count);
    }

    private void GameOver()
    {
        //StopAllCoroutines();
        GameManager.instance.GameOver();
    }

    public void HandleSettingsChange(string difficutly, Color color)
    {
        UpdateMoveRate(difficutly);
        UpdateColor(color);
    }

    public void UpdateRainbowColor(Color newColor)
    {
        UpdateColor(newColor);
    }
}
