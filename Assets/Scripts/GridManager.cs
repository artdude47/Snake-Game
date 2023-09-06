using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    public Vector2Int gridSize = new Vector2Int(20, 20);
    public GameObject boundaryPrefab;

    public bool showGridInPlayMode = false;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
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
        DrawBoundary();
    }

    //Check if a given position is within bounds
    public bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSize.x && position.y >= 0 && position.y < gridSize.y;
    }

    //Draw the boundary walls around the grid for visualization
    private void DrawBoundary()
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
    }

    private void OnDrawGizmos()
    {
        DrawGrid();
    }

    private void OnDrawGizmosSelected()
    {
        if (showGridInPlayMode)
        {
            DrawGrid();
        }
    }

    private void DrawGrid()
    {
        Gizmos.color = Color.gray;

        for(int x = 0; x <= gridSize.x; x++)
        {
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, gridSize.y, 0));
        }

        for(int y = 0; y <= gridSize.y; y++)
        {
            Gizmos.DrawLine(new Vector3(0, y, 0), new Vector3(gridSize.x, y, 0));
        }
    }
}
