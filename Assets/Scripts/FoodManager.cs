using UnityEngine;

public class FoodManager : MonoBehaviour
{
    [SerializeField]
    private GameObject foodPrefab;

    private Vector2Int gridSize;
    private SnakeController snakeController;
    private Vector2Int currentFoodPosition;
    private GameObject currentFood;

    public void Initialize(Vector2Int gridSize, SnakeController snakeController)
    {
        this.gridSize = gridSize;
        this.snakeController = snakeController;
        SpawnFood();
    }

    public void SpawnFood()
    {
        if (foodPrefab)
        {
            Vector2Int randomPosition;

            //keep trying until we get a position not occupied by the snake
            do
            {
                int x = Random.Range(0, gridSize.x);
                int y = Random.Range(0, gridSize.y);
                randomPosition = new Vector2Int(x, y);
            } while (IsPositionOccupiedBySnake(randomPosition));

            currentFoodPosition = randomPosition;

            if (currentFood != null)
            {
                Destroy(currentFood);
            }
            currentFood = Instantiate(foodPrefab, new Vector3(randomPosition.x, randomPosition.y, 0), Quaternion.identity);          
        }
    }

    private bool IsPositionOccupiedBySnake(Vector2Int position)
    {
        foreach(Transform bodyPart in snakeController.bodyParts)
        {
            if (Vector2Int.RoundToInt(bodyPart.position) == position)
                return true;
        }
        return false;
    }

    public bool IsFoodAtPosition(Vector2Int position)
    {
        return position == currentFoodPosition;
    }
}
