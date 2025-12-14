using UnityEngine;
using System.Collections.Generic;

public class DungeonGraph
{
    public readonly int W, H;
    public Vector2Int startPos;  
    public HashSet<Vector2Int> nodes = new HashSet<Vector2Int>();
    public HashSet<(Vector2Int, Vector2Int)> edges = new HashSet<(Vector2Int, Vector2Int)>();
   
    public DungeonGraph(int w, int h)
    {
        W = w;
        H = h;
    }
   
    public void AddEdge(Vector2Int a, Vector2Int b)
    {
        var edge = (a.x < b.x || (a.x == b.x && a.y < b.y)) ? (a, b) : (b, a);
        edges.Add(edge);
    }
   
    public bool HasEdge(Vector2Int a, Vector2Int b)
    {
        var edge = (a.x < b.x || (a.x == b.x && a.y < b.y)) ? (a, b) : (b, a);
        return edges.Contains(edge);
    }
    public IEnumerable<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        Vector2Int[] directions = { 
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left 
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = cell + dir;
        
            if (nodes.Contains(neighbor) && HasEdge(cell, neighbor))
            {
                yield return neighbor;
            }
        }
    }
}
