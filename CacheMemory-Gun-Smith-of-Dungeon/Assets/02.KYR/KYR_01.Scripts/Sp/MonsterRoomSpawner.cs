using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterRoomSpawner : MonoBehaviour
{
    private Action _onClearedCallback;
    
    [Header("참조")]
    [SerializeField] private RoomController room;             
    [SerializeField] private PoissonDiskSpawner pds;          
    [SerializeField] private MonsterListSO monsterList;      

    [Header("스폰 개수 설정")]
    [SerializeField] private int minTotalCount = 3;
    [SerializeField] private int maxTotalCount = 7;

    [Header("구조물(Obstacle) 설정")]
    [SerializeField] private LayerMask obstacleMask;           
    [SerializeField] private float obstacleCheckRadius = 0.3f;
    [SerializeField] private int obstacleResolveIterations = 5;
    [SerializeField] private float obstaclePushStrength = 0.2f;

    [Header("방 밖 체크 설정")]
    [SerializeField] private float outCheckInterval = 30f;

    private readonly List<BaseEnemy> _alive = new();
    private bool _started;
    private bool _cleared;
    private Bounds _roomBounds;

    private void Awake()
    {
        if (!room) room = GetComponentInParent<RoomController>();
        if (!pds)  pds  = GetComponentInChildren<PoissonDiskSpawner>();

        CalcRoomBounds();
    }
    
    public void StartBattle(Action onCleared)
    {
        if (_started || _cleared) return;
        _started = true;
        _onClearedCallback = onCleared;

        SpawnMonsters();
        StartCoroutine(PeriodicOutCheck());
    }

    private void SpawnMonsters()
    {
        _alive.Clear();

        int totalCount = Random.Range(minTotalCount, maxTotalCount + 1);

        var positions = pds != null
            ? pds.GetPositions(totalCount)
            : new List<Vector3> { transform.position };

        for (int i = 0; i < positions.Count; i++)
        {
            var entry = PickEntry(monsterList);
            if (entry == null || !entry.prefab) continue;

            var go = Instantiate(entry.prefab, positions[i], Quaternion.identity);
            var enemy = go.GetComponent<BaseEnemy>();
            if (!enemy) continue;

            // 스폰 직후 구조물 겹침 정리
            ResolveObstacleOverlap(enemy.transform);

            _alive.Add(enemy);
            enemy.OnDeath += HandleEnemyDeath;
        }
    }

    private MonsterListSO.Entry PickEntry(MonsterListSO list)
    {
        if (!list || list.entries == null || list.entries.Count == 0) return null;

        float total = 0f;
        foreach (var e in list.entries)
            total += Mathf.Max(0f, e.weight);

        float r = Random.value * total;
        float acc = 0f;

        foreach (var e in list.entries)
        {
            float w = Mathf.Max(0f, e.weight);
            acc += w;
            if (r <= acc) return e;
        }

        return list.entries[list.entries.Count - 1];
    }

    private void HandleEnemyDeath(BaseEnemy enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath;
        _alive.Remove(enemy);

        if (_alive.Count == 0)
        {
            MarkCleared();
        }
    }

    private void MarkCleared()
    {
        if (_cleared) return;
        _cleared = true;
        
        _onClearedCallback?.Invoke();
        
    }

    private void ResolveObstacleOverlap(Transform t)
    {
        for (int iter = 0; iter < obstacleResolveIterations; iter++)
        {
            var hits = Physics2D.OverlapCircleAll(t.position, obstacleCheckRadius, obstacleMask);
            if (hits.Length == 0) break;

            Vector2 pushDir = Vector2.zero;

            foreach (var h in hits)
            {
                Vector2 dir = (Vector2)t.position - (Vector2)h.bounds.center;
                if (dir.sqrMagnitude < 0.0001f)
                    dir = Random.insideUnitCircle.normalized;

                pushDir += dir.normalized;
            }

            if (pushDir.sqrMagnitude < 0.0001f) break;

            pushDir.Normalize();
            t.position += (Vector3)(pushDir * obstaclePushStrength);
        }
    }

    private void CalcRoomBounds()
    {
        if (pds == null)
        {
            _roomBounds = new Bounds(transform.position, Vector3.one * 10f);
            return;
        }

        var center = pds.transform.position;
        var size   = new Vector3(pds.areaSize.x, pds.areaSize.y, 0);
        _roomBounds = new Bounds(center, size);
    }

    private IEnumerator PeriodicOutCheck()
    {
        while (!_cleared)
        {
            yield return new WaitForSeconds(outCheckInterval);

            foreach (var enemy in _alive)
            {
                if (!enemy) continue;

                var pos = enemy.transform.position;
                if (!_roomBounds.Contains(pos))
                {
                    Vector3 clamped = pos;
                    clamped.x = Mathf.Clamp(clamped.x, _roomBounds.min.x, _roomBounds.max.x);
                    clamped.y = Mathf.Clamp(clamped.y, _roomBounds.min.y, _roomBounds.max.y);

                    enemy.transform.position = clamped;
                }
            }
        }
    }
}
