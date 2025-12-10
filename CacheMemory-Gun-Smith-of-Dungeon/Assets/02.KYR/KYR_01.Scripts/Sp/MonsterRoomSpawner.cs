using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MonsterRoomSpawner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RoomController room;              // 문 여닫는 스크립트
    [SerializeField] private PoissonDiskSpawner pds;           // 위치 뽑는 PDS
    [SerializeField] private MonsterListSO monsterList;        // 어떤 몹 나올지 SO

    [Header("스폰 개수 설정")]
    [SerializeField] private int minTotalCount = 3;            // 한 방 최소 몹 수
    [SerializeField] private int maxTotalCount = 7;            // 한 방 최대 몹 수

    [Header("구조물(Obstacle) 설정")]
    [SerializeField] private LayerMask obstacleMask;           // 기둥/상자/지형 레이어
    [SerializeField] private float obstacleCheckRadius = 0.3f; // 구조물 겹침 검사 반경
    [SerializeField] private int obstacleResolveIterations = 5;// 최대 밀어내기 반복 횟수
    [SerializeField] private float obstaclePushStrength = 0.2f;// 한 번 밀어낼 때 거리

    [Header("방 밖 체크 설정")]
    [SerializeField] private float outCheckInterval = 30f;     // 방 밖 검사 주기(초)

    private readonly List<BaseEnemy> _alive = new();
    private bool _entered;
    private bool _cleared;
    private Bounds _roomBounds;

    private void Awake()
    {
        if (!room) room = GetComponentInParent<RoomController>();
        if (!pds)  pds  = GetComponentInChildren<PoissonDiskSpawner>();

        // 트리거로 강제
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        CalcRoomBounds();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_entered || _cleared) return;
        if (!other.CompareTag("Player")) return;

        _entered = true;

        // 방 입장  문 닫고 전투 시작
        room.CloseAllLinkedDoors();       // RoomController에 이 메서드 하나 추가해두면 편함
        SpawnMonsters();
        StartCoroutine(PeriodicOutCheck());
    }


    private void SpawnMonsters()
    {
        _alive.Clear();

        // 1. 이 방에서 몇 마리 뽑을지 결정 (나중에 난이도/플레이어 옵션으로 바꿔도 됨)
        int totalCount = Random.Range(minTotalCount, maxTotalCount + 1);

        // 2. PDS로 위치 뽑기
        var positions = pds != null
            ? pds.GetPositions(totalCount)
            : new List<Vector3> { transform.position }; // 혹시 pds 없으면 안전장치

        // MonsterListSO에서 몬스터 타입 뽑아서 배치
        for (int i = 0; i < positions.Count; i++)
        {
            var entry = PickEntry(monsterList);
            if (entry == null || !entry.prefab) continue;

            var go = Instantiate(entry.prefab, positions[i], Quaternion.identity);
            var enemy = go.GetComponent<BaseEnemy>();
            if (!enemy) continue;

            // 구조물과 겹치면 조금씩 밖으로 밀어내기 (한 번만)
            ResolveObstacleOverlap(enemy.transform);

            _alive.Add(enemy);
            //enemy.OnDied += HandleEnemyDead;
        }
    }

    // MonsterListSO에서 weight 기반으로 하나 뽑기
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
    
    private void HandleEnemyDead(BaseEnemy enemy)
    {
       // enemy.OnDied -= HandleEnemyDead;
        _alive.Remove(enemy);

        if (_alive.Count == 0)
        {
            _cleared = true;
            room.OpenAllLinkedDoors();
        }
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
                // 구조물 중심  몬스터 방향
                Vector2 dir = (Vector2)t.position - (Vector2)h.bounds.center;
                if (dir.sqrMagnitude < 0.0001f)
                {
                    dir = Random.insideUnitCircle.normalized;
                }

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
            // pds 없으면 대충 임시 Bounds
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

                // 방 영역 밖으로 나간 경우만 처리
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
