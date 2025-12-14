using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
   [SerializeField] private Vector2Int roomSize = new Vector2Int(16,11);
   [SerializeField] private Vector2Int roomSpacing = new Vector2Int(2,2);
   [SerializeField] private GameObject startRoomPrefab;
   [SerializeField] private GameObject bossRoomPrefab;
   [SerializeField] private GameObject[] eventRoomPrefab;
   [SerializeField] private GameObject[] monsterRoomPrefab;
   [SerializeField, Range(0,1)] private float extraEdgeChance = 0.3f;
   [SerializeField, Range(0,1)] private float eventChance = 0.35f;
   [SerializeField] private  int minRooms = 9; 
   [SerializeField] private  int maxRooms = 13; 
   [SerializeField] private int maxEventRooms = 2;
   [SerializeField] private bool randomStart = true;
   [SerializeField] private CorridorGenerator corridorGenerator;
   
   
   int gridw=4,gridh=4;
   private System.Random ran;
   private DungeonGraph graph;
   private Dictionary<Vector2Int, RoomController> placed = new Dictionary<Vector2Int, RoomController>();
   private int eventCount = 0;
   private Vector2Int? firstConnectedRoom = null;
   private Vector2Int farRoom;
   private int currentSeed;
   
   [SerializeField] private bool autoGenerate = false;
   private void Start()
   {
      if (!autoGenerate) return;
      if (PlayerPrefs.HasKey("DungeonSeed"))
      {
         currentSeed = PlayerPrefs.GetInt("DungeonSeed");
         Debug.Log($"저장된 시드 로드: {currentSeed}");
      }
      else
      {
         currentSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
         PlayerPrefs.SetInt("DungeonSeed", currentSeed);
         PlayerPrefs.Save();
         Debug.Log($"새 시드 생성 및 저장: {currentSeed}");
      }
      
      Generate();
   }
   
   public void GeneratePublic()
   {
      Generate(); 
   }
   
   public void Generate(int seed = -1)
   {
      if (seed == -1)
      {
         currentSeed = (int)DateTime.Now.Ticks;
      }
      else
      {
         currentSeed = seed;
      }

      ran = new System.Random(currentSeed);

      foreach (Transform c in transform)
         DestroyImmediate(c.gameObject);

      graph = BuildDungeonGraph();
      eventCount = 0;

      farRoom = FindFarRoom();
      Vector2Int bossDirection = FindEmptyDirection(farRoom);
      Dictionary<Vector2Int, GameObject> roomPrefabs = DetermineRoomTypes();

      placed.Clear();

      foreach (var cell in graph.nodes)
      {
         Vector3 worldPos = GridToWorld(cell);   // 일단 그리드 기준으로 배치
         GameObject prefab = roomPrefabs.ContainsKey(cell) ? roomPrefabs[cell] : monsterRoomPrefab[0];

         var roomobj = Instantiate(prefab, worldPos, Quaternion.identity, transform);

         var roomController = roomobj.GetComponent<RoomController>();
         if (roomController != null)
         {
            placed[cell] = roomController;

            RoomLinks links = CalcLinKs(cell, farRoom, bossDirection);
            roomController.Init(links);
         }
      }
      
      AddExtraEdges();

      Vector2Int bossPos = farRoom + bossDirection;
      Vector3 bossWorldPos = GridToWorld(bossPos);
      graph.nodes.Add(bossPos);
      graph.AddEdge(farRoom, bossPos);
      
      var bossObj = Instantiate(bossRoomPrefab, bossWorldPos, Quaternion.identity, transform);
      var bossController = bossObj.GetComponent<RoomController>();
      if (bossController != null)
      {
         RoomLinks bossLinks = new RoomLinks();
         if (bossDirection == Vector2Int.up) bossLinks.S = true;
         else if (bossDirection == Vector2Int.right) bossLinks.W = true;
         else if (bossDirection == Vector2Int.down) bossLinks.N = true;
         else if (bossDirection == Vector2Int.left) bossLinks.E = true;
         
         bossController.Init(bossLinks);
         placed[bossPos] = bossController;
      }
      
      AlignRoomsByAnchors();

      Debug.Log($"최종 생성된 방 개수: {graph.nodes.Count}개");

      if (corridorGenerator != null)
      {
         corridorGenerator.GenerateCorridors(graph, placed);
      }

   }
   
   Vector2Int FindFarRoom()
   {
      var distance = BFS(graph.startPos);
      Vector2Int farthest = graph.startPos;
      int maxDistance = -1;
      foreach (var kv in distance)
      {
         if (kv.Value > maxDistance)
         {
            maxDistance = kv.Value;
            farthest = kv.Key;
         }
      }
      return farthest;
   }
      
   Dictionary<Vector2Int, int> BFS(Vector2Int start)
   {
      var distances = new Dictionary<Vector2Int, int>();
      var queue = new Queue<Vector2Int>();
      
      queue.Enqueue(start);
      distances[start] = 0;
      
      while (queue.Count > 0)
      {
         var current = queue.Dequeue();
         int currentDist = distances[current];
            
         foreach (var dir in new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left })
         {
            var neighbor = current + dir;
               
            if (!Inside(neighbor)) continue;
            if (!graph.nodes.Contains(neighbor)) continue;
            if (!graph.HasEdge(current, neighbor)) continue;
            if (distances.ContainsKey(neighbor)) continue;
            
            distances[neighbor] = currentDist + 1;
            queue.Enqueue(neighbor);
         }
      }
      
      return distances;
   }

   Vector2Int FindEmptyDirection(Vector2Int cell)
   {
      var candidates = new List<Vector2Int>();
      foreach (var dir in new[]{Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left})
      {
         var neighbor = cell + dir;
         if (!Inside(neighbor) || !graph.nodes.Contains(neighbor))
         {
            candidates.Add(dir);
         }
      }

      return candidates.Count > 0 ? candidates[ran.Next(candidates.Count)] : Vector2Int.up;
   }
   
   Dictionary<Vector2Int, GameObject> DetermineRoomTypes()
   {
      Dictionary<Vector2Int, GameObject> roomPrefabs = new Dictionary<Vector2Int, GameObject>();
      
      List<Vector2Int> remainingNodes = new List<Vector2Int>(graph.nodes);
      remainingNodes.Remove(graph.startPos);
      
      roomPrefabs[graph.startPos] = startRoomPrefab;
      
      if (firstConnectedRoom.HasValue)
      {
         Vector2Int firstRoom = firstConnectedRoom.Value;
         roomPrefabs[firstRoom] = monsterRoomPrefab[ran.Next(0, monsterRoomPrefab.Length)];
         remainingNodes.Remove(firstRoom);
      }
      
      Shuffle(remainingNodes);
         
      List<Vector2Int> nodesToReplace = new List<Vector2Int>();
      foreach (var cell in remainingNodes)
      {
         if (ran.NextDouble() < eventChance && eventCount < maxEventRooms)
         {
            roomPrefabs[cell] = eventRoomPrefab[ran.Next(0, eventRoomPrefab.Length)];
            eventCount++;
            nodesToReplace.Add(cell);
         }
      }
      
      foreach(var node in nodesToReplace)
      {
         remainingNodes.Remove(node);
      }
      
      foreach (var cell in remainingNodes)
      {
         if (!roomPrefabs.ContainsKey(cell))
         {
            roomPrefabs[cell] = monsterRoomPrefab[ran.Next(0, monsterRoomPrefab.Length)];
         }
      }
      
      return roomPrefabs;
   }

   RoomLinks CalcLinKs(Vector2Int cell, Vector2Int farRoom, Vector2Int bossDir)
   {
      bool N = graph.HasEdge(cell, cell + Vector2Int.up);
      bool E = graph.HasEdge(cell, cell + Vector2Int.right);
      bool S = graph.HasEdge(cell, cell + Vector2Int.down);
      bool W = graph.HasEdge(cell, cell + Vector2Int.left);
      
      if (cell == farRoom)
      {
         if (bossDir == Vector2Int.up) N = true;
         else if (bossDir == Vector2Int.right) E = true;
         else if (bossDir == Vector2Int.down) S = true;
         else if (bossDir == Vector2Int.left) W = true;
      }

      return new RoomLinks{N = N, E = E, S = S, W = W};
   }
   
   DungeonGraph BuildDungeonGraph()
   {
      var g = new DungeonGraph(gridw, gridh);
      bool[,] visited = new bool[gridw, gridh];
      firstConnectedRoom = null;
      
      int targetRoomCount = ran.Next(minRooms, maxRooms + 1); 
      Debug.Log($"목표 던전 크기: {targetRoomCount}개");
      
      Vector2Int start = PickRandomBoder();
      g.startPos = start;
      g.nodes.Add(start);
      visited[start.x, start.y] = true;
      
      Vector2Int? firstRoom = null;
      
      foreach (var neighbor in ShuffledNeighbors(start))
      {
         if (!Inside(neighbor)) continue;
         g.nodes.Add(neighbor);                 
         visited[neighbor.x, neighbor.y] = true;
         g.AddEdge(start, neighbor);           
         firstRoom = neighbor;
         firstConnectedRoom = neighbor;
         Debug.Log($"  첫 연결: ({neighbor.x}, {neighbor.y})");
         break;
      }
      
      var stack = new Stack<Vector2Int>();      
      if (firstRoom.HasValue)
         stack.Push(firstRoom.Value);           
      
      while (stack.Count > 0)                    
      {
         if (g.nodes.Count >= targetRoomCount)
         {
            stack.Clear(); 
            break;
         }
         
         var current = stack.Pop();              
         
         foreach (var neighbor in ShuffledNeighbors(current))
         {
            if (!Inside(neighbor)) continue;
            if (visited[neighbor.x, neighbor.y]) continue;  
            
            g.nodes.Add(neighbor);             
            visited[neighbor.x, neighbor.y] = true;
            g.AddEdge(current, neighbor);        
            
            stack.Push(current);                
            stack.Push(neighbor);                
            
            Debug.Log($" 연결: ({current.x},{current.y}) ({neighbor.x},{neighbor.y})");
            break;
         }
      }
      
      if (g.nodes.Count < minRooms)
      {
          return BuildDungeonGraph(); 
      }
      
      return g;
   }
   
   void AddExtraEdges()
   {
      foreach (var cell in graph.nodes)
      {
         if (cell == farRoom) continue;
         
         var neighbors = ShuffledNeighbors(cell);
         foreach (var neighbor in neighbors)
         {
            if (!Inside(neighbor)) continue;
            if (neighbor == farRoom) continue;
            if (graph.nodes.Contains(neighbor) && !graph.HasEdge(cell, neighbor))
            {
               if (ran.NextDouble() < extraEdgeChance)
               {
                  graph.AddEdge(cell, neighbor);
                  
                  if(placed.ContainsKey(cell)) 
                     placed[cell].Init(CalcLinKs(cell, farRoom, Vector2Int.zero));
                  if(placed.ContainsKey(neighbor)) 
                     placed[neighbor].Init(CalcLinKs(neighbor, farRoom, Vector2Int.zero));
               }
            }
         }
      }
   }
   
   void Shuffle<T>(List<T> list)
   {
       for (int i = list.Count - 1; i > 0; i--)
       {
           int j = ran.Next(0, i + 1);
           (list[i], list[j]) = (list[j], list[i]);
       }
   }

   IEnumerable<Vector2Int> ShuffledNeighbors(Vector2Int pos)
   {
      var neighbors = new List<Vector2Int>
      {
         pos + Vector2Int.up,
         pos + Vector2Int.right,
         pos + Vector2Int.down,
         pos + Vector2Int.left
      };
      
      for (int i = neighbors.Count - 1; i > 0; i--)
      {
         int j = ran.Next(0, i + 1);
         (neighbors[i], neighbors[j]) = (neighbors[j], neighbors[i]);
      }
      
      return neighbors;
   }
   
   bool Inside(Vector2Int pos)
   {
      return pos.x >= 0 && pos.y >= 0 && pos.x < gridw && pos.y < gridh;
   }

   Vector2Int PickRandomBoder()
   {
      var candidatea = new List<Vector2Int>();

      for (int y = 0; y < gridh; y++)
      {
         for (int x = 0; x < gridw; x++)
         {
            bool isBorder = (x == 0 || y == 0 || x == gridw - 1 || y == gridh - 1);
            if (isBorder)
               candidatea.Add(new Vector2Int(x, y));
         }
      }
      int randomindex = ran.Next(0, candidatea.Count);
      return candidatea[randomindex];
   }

  void AlignRoomsByAnchors()
{
    if (graph == null || placed.Count == 0) return;

    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

    Vector2Int start = graph.startPos;
    if (!placed.ContainsKey(start)) return; 

    queue.Enqueue(start);
    visited.Add(start);

    while (queue.Count > 0)
    {
        Vector2Int cell = queue.Dequeue();
        if (!placed.TryGetValue(cell, out var room)) continue;

        foreach (Vector2Int neighbor in graph.GetNeighbors(cell)) 
        {
            if (visited.Contains(neighbor)) continue;
            if (!placed.TryGetValue(neighbor, out var neighborRoom)) continue;

            Vector2Int dir = neighbor - cell; 
            Vector3 currentPos = neighborRoom.transform.position;
            
            // 수평 연결 (동/서): Y축 오프셋만 조정
            if (dir.y == 0 && Mathf.Abs(dir.x) == 1) 
            {
                RoomController.DoorDir doorCell = (dir == Vector2Int.right) ? RoomController.DoorDir.E : RoomController.DoorDir.W;
                RoomController.DoorDir doorNeighbor = (dir == Vector2Int.right) ? RoomController.DoorDir.W : RoomController.DoorDir.E;

                Vector3 a = room.GetDoorAnchorWorldPos(doorCell);
                Vector3 b = neighborRoom.GetDoorAnchorWorldPos(doorNeighbor);
                
                float dy = a.y - b.y; 
                currentPos.y += dy;
            }
            // 수직 연결 (남/북): X축 오프셋만 조정
            else if (dir.x == 0 && Mathf.Abs(dir.y) == 1) 
            {
                RoomController.DoorDir doorCell = (dir == Vector2Int.up) ? RoomController.DoorDir.N : RoomController.DoorDir.S;
                RoomController.DoorDir doorNeighbor = (dir == Vector2Int.up) ? RoomController.DoorDir.S : RoomController.DoorDir.N;

                Vector3 a = room.GetDoorAnchorWorldPos(doorCell);
                Vector3 b = neighborRoom.GetDoorAnchorWorldPos(doorNeighbor);

                float dx = a.x - b.x;
                currentPos.x += dx;
            }

            neighborRoom.transform.position = currentPos;

            visited.Add(neighbor);
            queue.Enqueue(neighbor);
        }
    }
}
   Vector3 GridToWorld(Vector2Int gridPos)
   {
      float xPos = gridPos.x * (roomSize.x + roomSpacing.x);
      float yPos = gridPos.y * (roomSize.y + roomSpacing.y);
      return new Vector3(xPos, yPos, 0);
   }

   public struct RoomLinks
   {
      public bool N;
      public bool E;
      public bool S;
      public bool W;
   }
   
   public void ApplyTheme(DungeonThemeSO t)
   {
      startRoomPrefab  = t.startRoomPrefab;
      bossRoomPrefab   = t.bossRoomPrefab;
      eventRoomPrefab  = t.eventRoomPrefabs;
      monsterRoomPrefab = t.monsterRoomPrefabs;

      if (corridorGenerator != null)
      {
         corridorGenerator.SetCorridorPrefabs(
            t.horizontalCorridorPrefab,
            t.verticalCorridorPrefab
         );
      }
   }
   
   [ContextMenu("Clear Saved Seed")]
   public void ClearSavedSeed()
   {
      PlayerPrefs.DeleteKey("DungeonSeed");
      PlayerPrefs.Save();
      Debug.Log("저장된 시드 삭제됨");
   }
}