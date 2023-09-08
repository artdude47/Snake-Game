using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    public Vector2Int gridSize = new Vector2Int(20, 20);
    [SerializeField]
    private GameObject boundaryPrefab;

    private void Awake()
    {
        //implement singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        DrawBoundary();
    }

    //Check if a given position is within bounds
    public bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSize.x && position.y >= 0 && position.y < gridSize.y;
    }

    //Draw the boundary walls around the grid for visualization
    public void DrawBoundary()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            Instantiate(boundaryPrefab, new Vector3(i, -1, 0), Quaternion.identity);
            Instantiate(boundaryPrefab, new Vector3(i, gridSize.y, 0), Quaternion.identity);
        }

        for (int i = 0; i < gridSize.y; i++)
        {
            Instantiate(boundaryPrefab, new Vector3(-1, i, 0), Quaternion.identity);
            Instantiate(boundaryPrefab, new Vector3(gridSize.x, i, 0), Quaternion.identity);
        }

        // Drawing corners
        Instantiate(boundaryPrefab, new Vector3(-1, -1, 0), Quaternion.identity);        // Bottom-left
        Instantiate(boundaryPrefab, new Vector3(gridSize.x, -1, 0), Quaternion.identity); // Bottom-right
        Instantiate(boundaryPrefab, new Vector3(-1, gridSize.y, 0), Quaternion.identity); // Top-left
        Instantiate(boundaryPrefab, new Vector3(gridSize.x, gridSize.y, 0), Quaternion.identity); // Top-right
    }
}
