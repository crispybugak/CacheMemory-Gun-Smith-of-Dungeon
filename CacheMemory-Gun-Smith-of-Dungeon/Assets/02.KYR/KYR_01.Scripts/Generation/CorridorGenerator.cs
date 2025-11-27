using System.Collections.Generic;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    [SerializeField] private GameObject horizontalCorridorPrefab; 
    [SerializeField] private GameObject verticalCorridorPrefab;  

    public void GenerateCorridors(DungeonGraph graph, Vector2Int roomSize, Vector2Int roomSpacing)
    {
        if (graph == null) return;
        

        foreach (var cell in graph.nodes)
        {
            // 동쪽 이웃
            Vector2Int east = cell + Vector2Int.right;
            if (graph.nodes.Contains(east) && graph.HasEdge(cell, east))
            {
                CreateHorizontalCorridor(cell, east, roomSize, roomSpacing);
            }

            // 북쪽 이웃
            Vector2Int north = cell + Vector2Int.up;
            if (graph.nodes.Contains(north) && graph.HasEdge(cell, north))
            {
                CreateVerticalCorridor(cell, north, roomSize, roomSpacing);
            }
        }
    }

    private void CreateHorizontalCorridor(Vector2Int a, Vector2Int b, Vector2Int roomSize, Vector2Int roomSpacing)
    {
        if (horizontalCorridorPrefab == null) return;

        Vector3 posA = GridToWorld(a, roomSize, roomSpacing);
        Vector3 posB = GridToWorld(b, roomSize, roomSpacing);
        Vector3 mid = (posA + posB) * 0.5f;   

        Instantiate(horizontalCorridorPrefab, mid, Quaternion.identity, transform);
    }

    private void CreateVerticalCorridor(Vector2Int a, Vector2Int b, Vector2Int roomSize, Vector2Int roomSpacing)
    {
        if (verticalCorridorPrefab == null) return;

        Vector3 posA = GridToWorld(a, roomSize, roomSpacing);
        Vector3 posB = GridToWorld(b, roomSize, roomSpacing);
        Vector3 mid = (posA + posB) * 0.5f;

        Instantiate(verticalCorridorPrefab, mid, Quaternion.identity, transform);
    }
    
    private Vector3 GridToWorld(Vector2Int gridPos, Vector2Int roomSize, Vector2Int roomSpacing)
    {
        float xPos = gridPos.x * (roomSize.x + roomSpacing.x);
        float yPos = gridPos.y * (roomSize.y + roomSpacing.y);
        return new Vector3(xPos, yPos, 0);
    }
}
