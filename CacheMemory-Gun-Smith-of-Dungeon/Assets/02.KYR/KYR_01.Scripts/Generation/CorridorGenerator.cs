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

        var obj = Instantiate(horizontalCorridorPrefab, mid, Quaternion.identity, transform);
       
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.size = new Vector2(roomSpacing.x, sr.size.y);
        }
    }

    private void CreateVerticalCorridor(Vector2Int a, Vector2Int b, Vector2Int roomSize, Vector2Int roomSpacing)
    {
        if (verticalCorridorPrefab == null) return;

        Vector3 posA = GridToWorld(a, roomSize, roomSpacing);
        Vector3 posB = GridToWorld(b, roomSize, roomSpacing);
        Vector3 mid = (posA + posB) * 0.5f;
        var obj = Instantiate(verticalCorridorPrefab, mid, Quaternion.identity, transform);
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.size = new Vector2(sr.size.x, roomSpacing.y);
        }
    }
    public void GenerateBossCorridor(Vector2Int farRoom, Vector2Int bossPos,
        Vector2Int roomSize, Vector2Int roomSpacing)
    {
        Vector2Int delta = bossPos - farRoom;
        
        if (Mathf.Abs(delta.x) == 1 && delta.y == 0)
        {
            CreateHorizontalCorridor(farRoom, bossPos, roomSize, roomSpacing);
        }
        else if (Mathf.Abs(delta.y) == 1 && delta.x == 0)
        {
            CreateVerticalCorridor(farRoom, bossPos, roomSize, roomSpacing);
        }
    }
    
    private Vector3 GridToWorld(Vector2Int gridPos, Vector2Int roomSize, Vector2Int roomSpacing)
    {
        float xPos = gridPos.x * (roomSize.x + roomSpacing.x);
        float yPos = gridPos.y * (roomSize.y + roomSpacing.y);
        return new Vector3(xPos, yPos, 0);
    }
}
