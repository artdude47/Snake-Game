using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SnakeController : MonoBehaviour
{
    //Prefab for the snake body
    public GameObject snakeBodyPrefab;

    //define the size of the game grid
    public Vector2Int gridSize;

    //How often the snake moves
    public float moveRate = 0.5f;

    //Reference to the food manager
    public FoodManager foodManager;

    //start game moving to the right
    private Vector2Int currentDirection = new Vector2Int(1, 0);
    public List<Transform> bodyParts = new List<Transform>();
    private Vector2Int currentPos;

    private void Start()
    {
        //initialize the snake with a head
        bodyParts.Add(this.transform);
        currentPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        
    }

    public void StartGame()
    {
        //Start the movement coroutine
        StartCoroutine(Move());
    }

    private void Update()
    {
        if (GameManager.instance.currentState == GameManager.GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.W) && currentDirection.y == 0)
                currentDirection = new Vector2Int(0, 1);
            else if (Input.GetKeyDown(KeyCode.S) && currentDirection.y == 0)
                currentDirection = new Vector2Int(0, -1);
            else if (Input.GetKeyDown(KeyCode.A) && currentDirection.x == 0)
                currentDirection = new Vector2Int(-1, 0);
            else if (Input.GetKeyDown(KeyCode.D) && currentDirection.x == 0)
                currentDirection = new Vector2Int(1, 0);
        }
    }

    private IEnumerator Move()
    {
            while (true)
            {
                yield return new WaitForSeconds(moveRate);

                Vector2Int previousPos = currentPos;
                currentPos += currentDirection;

                //Update head position
                bodyParts[0].position = new Vector3(currentPos.x, currentPos.y, bodyParts[0].position.z);

                //Update body positions
                for (int i = 1; i < bodyParts.Count; i++)
                {
                    Vector2Int tempPos = new Vector2Int((int)bodyParts[i].position.x, (int)bodyParts[i].position.y);
                    bodyParts[i].position = new Vector3(previousPos.x, previousPos.y, bodyParts[i].position.z);
                    previousPos = tempPos;
                }

                //check for body collisions
                CheckCollisions();

                //Check for food consumption
                if (foodManager.IsFoodAtPosition(currentPos))
                {
                    Grow();
                    foodManager.SpawnFood();
                }
            }
    }

    private void CheckCollisions()
    {
        // Check boundary collisions
        if (currentPos.x < 0 || currentPos.x >= gridSize.x || currentPos.y < 0 || currentPos.y >= gridSize.y)
        {
            Debug.Log("Collided with wall");
            GameOver();
            return;
        }

        //Check for self collisions
        for (int i = 1; i < bodyParts.Count; i++)
        {
            if((Vector2)bodyParts[i].position == (Vector2)currentPos)
            {
                Debug.Log("Collided with segment at index: " + i);
                GameOver();
                return;
            }
        }
    }

    public void Grow()
    {
        Transform newPart = Instantiate(snakeBodyPrefab, bodyParts[bodyParts.Count - 1].position, Quaternion.identity).transform;
        bodyParts.Add(newPart);

        //Update score in GameManager
        GameManager.instance.IncreaseScore(bodyParts.Count);
    }

    private void GameOver()
    {
        GameManager.instance.GameOver();
    }
}