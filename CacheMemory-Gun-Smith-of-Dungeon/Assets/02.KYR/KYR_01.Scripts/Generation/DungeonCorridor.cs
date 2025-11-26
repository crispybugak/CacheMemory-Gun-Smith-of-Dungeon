using UnityEngine;
using System.Collections.Generic;

public class DungeonCorridor : MonoBehaviour
{
    [Header("시각화 설정")]
    [SerializeField] private bool showConnections = true;     // 간선 표시
    [SerializeField] private bool showRoomLabels = true;      // 방 좌표 표시
    [SerializeField] private Color connectionColor = Color.cyan;
    [SerializeField] private Color startRoomColor = Color.green;
    [SerializeField] private Color bossRoomColor = Color.red;
    [SerializeField] private float lineWidth = 0.2f;
    
    private DungeonGen dungeonGen;
    private DungeonGraph graph;
    
    void Start()
    {
        dungeonGen = GetComponent<DungeonGen>();
    }
    
    /// <summary>
    /// Scene 뷰에서 그리기 (에디터 전용)
    /// </summary>
    void OnDrawGizmos()
    {
        if (!showConnections) return;
        if (dungeonGen == null) dungeonGen = GetComponent<DungeonGen>();
        if (dungeonGen == null) return;
        
        // DungeonGen에서 graph 정보 가져오기
        graph = GetGraphFromDungeonGen();
        if (graph == null || graph.nodes.Count == 0) return;
        
        DrawConnections();
        if (showRoomLabels) DrawRoomLabels();
    }
    
    /// <summary>
    /// 간선(연결) 그리기
    /// </summary>
    void DrawConnections()
    {
        Gizmos.color = connectionColor;
        
        foreach (var edge in graph.edges)
        {
            Vector2Int a = edge.Item1;
            Vector2Int b = edge.Item2;
            
            Vector3 posA = GridToWorld(a);
            Vector3 posB = GridToWorld(b);
            
            // 방 중심에서 중심으로 선 그리기
            Gizmos.DrawLine(posA, posB);
            
            // 선 두껍게 (여러 번 그리기)
            for (float offset = -lineWidth; offset <= lineWidth; offset += 0.1f)
            {
                if (posA.x == posB.x) // 수직선
                {
                    Gizmos.DrawLine(posA + Vector3.right * offset, posB + Vector3.right * offset);
                }
                else // 수평선
                {
                    Gizmos.DrawLine(posA + Vector3.up * offset, posB + Vector3.up * offset);
                }
            }
        }
    }
    
    /// <summary>
    /// 방 좌표 라벨 그리기
    /// </summary>
    void DrawRoomLabels()
    {
        foreach (var cell in graph.nodes)
        {
            Vector3 pos = GridToWorld(cell);
            
            // 시작방은 초록, 나머지는 하양
            if (cell == graph.startPos)
                Gizmos.color = startRoomColor;
            else
                Gizmos.color = Color.white;
            
            // 방 중심에 작은 구 그리기
            Gizmos.DrawSphere(pos, 0.3f);
            
#if UNITY_EDITOR
            // 좌표 텍스트 표시
            UnityEditor.Handles.Label(pos + Vector3.up * 0.5f, $"({cell.x},{cell.y})");
#endif
        }
    }
    
    /// <summary>
    /// DungeonGen에서 graph 가져오기 (Reflection 사용)
    /// </summary>
    DungeonGraph GetGraphFromDungeonGen()
    {
        if (dungeonGen == null) return null;
        
        // Reflection으로 private 필드 접근
        var field = typeof(DungeonGen).GetField("graph", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        return field?.GetValue(dungeonGen) as DungeonGraph;
    }
    
    /// <summary>
    /// 그리드 → 월드 좌표 (DungeonGen과 동일한 계산)
    /// </summary>
    Vector3 GridToWorld(Vector2Int gridPos)
    {
        // DungeonGen의 roomSize, roomSpacing 가져오기
        var roomSizeField = typeof(DungeonGen).GetField("roomSize", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        var roomSpacingField = typeof(DungeonGen).GetField("roomSpacing", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        Vector2Int roomSize = (Vector2Int)(roomSizeField?.GetValue(dungeonGen) ?? new Vector2Int(16, 11));
        Vector2Int roomSpacing = (Vector2Int)(roomSpacingField?.GetValue(dungeonGen) ?? new Vector2Int(2, 2));
        
        float xPos = gridPos.x * (roomSize.x + roomSpacing.x);
        float yPos = gridPos.y * (roomSize.y + roomSpacing.y);
        return new Vector3(xPos, yPos, 0);
    }
}
