using System.Collections.Generic;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    [SerializeField] private GameObject horizontalCorridorPrefab;
    [SerializeField] private GameObject verticalCorridorPrefab;
    public void SetCorridorPrefabs(GameObject horizontal, GameObject vertical)
    {
        horizontalCorridorPrefab = horizontal;
        verticalCorridorPrefab = vertical;
    }
    public void GenerateCorridors(
        DungeonGraph graph,
        Dictionary<Vector2Int, RoomController> placed)
    {
        if (graph == null) return;

        foreach (var cell in graph.nodes)
        {
            Vector2Int east = cell + Vector2Int.right;
            if (graph.nodes.Contains(east) && graph.HasEdge(cell, east))
            {
                CreateHorizontalCorridor(cell, east, placed);
            }

            Vector2Int north = cell + Vector2Int.up;
            if (graph.nodes.Contains(north) && graph.HasEdge(cell, north))
            {
                CreateVerticalCorridor(cell, north, placed);
            }
        }
    }

    private void CreateHorizontalCorridor(
        Vector2Int leftCell, Vector2Int rightCell,
        Dictionary<Vector2Int, RoomController> placed)
    {
        if (!placed.TryGetValue(leftCell, out var leftRoom))  return;
        if (!placed.TryGetValue(rightCell, out var rightRoom)) return;

        Vector3 leftDoor  = leftRoom.GetDoorWorldPos(RoomController.DoorDir.E);
        Vector3 rightDoor = rightRoom.GetDoorWorldPos(RoomController.DoorDir.W);

        var obj = Instantiate(horizontalCorridorPrefab, transform);
        var stretch = obj.GetComponent<CorridorStretch>();
        if (stretch != null)
        {
            stretch.SetBetween(leftDoor, rightDoor);
        }
    }

    private void CreateVerticalCorridor(
        Vector2Int bottomCell, Vector2Int topCell,
        Dictionary<Vector2Int, RoomController> placed)
    {
        if (!placed.TryGetValue(bottomCell, out var bottomRoom)) return;
        if (!placed.TryGetValue(topCell, out var topRoom))      return;

        Vector3 bottomDoor = bottomRoom.GetDoorWorldPos(RoomController.DoorDir.N);
        Vector3 topDoor    = topRoom.GetDoorWorldPos(RoomController.DoorDir.S);

        var obj = Instantiate(verticalCorridorPrefab, transform);
        var stretch = obj.GetComponent<CorridorStretch>();
        if (stretch != null)
        {
            stretch.SetBetween(bottomDoor, topDoor);
        }
    }

 // public void GenerateBossCorridor(
 //    Vector2Int farRoom, Vector2Int bossPos,
 //     Dictionary<Vector2Int, RoomController> placed)
 //  {
 //      Vector2Int delta = bossPos - farRoom;
 //
 //      if (Mathf.Abs(delta.x) == 1 && delta.y == 0)
 //      {
 //          CreateHorizontalCorridor(farRoom, bossPos, placed);
 //      }
 //      else if (Mathf.Abs(delta.y) == 1 && delta.x == 0)
 //       {
 //          CreateVerticalCorridor(farRoom, bossPos, placed);
 //       }
 //  }

    
    private Vector3 GridToWorld(Vector2Int gridPos, Vector2Int roomSize, Vector2Int roomSpacing)
    {
        float xPos = gridPos.x * (roomSize.x + roomSpacing.x);
        float yPos = gridPos.y * (roomSize.y + roomSpacing.y);
        return new Vector3(xPos, yPos, 0);
    }
}
