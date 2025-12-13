using UnityEngine;

public class BossRoomBattle : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RoomController roomController; // 보스방 문
    [SerializeField] private Collider2D enterTrigger;       // 플레이어 입장 트리거
    [SerializeField] private GameObject bossPrefab;        

    private bool _unlocked = false; // 모든 몬스터방 클리어해서 보스방 해금됐는지
    private bool _entered  = false; // 플레이어가 이미 보스방에 들어왔는지
    private bool _cleared  = false; // 보스까지 잡았는지

    private BaseEnemy _bossInstance;

    private void Awake()
    {
        if (!roomController) roomController = GetComponent<RoomController>();
        if (!enterTrigger)   enterTrigger   = GetComponent<Collider2D>();

        if (enterTrigger != null)
            enterTrigger.isTrigger = true;
        
        if (roomController != null)
            roomController.CloseAllLinkedDoors();
    }

    private void OnEnable()
    {
        //  DungeonProgress.Instance의 이벤트 구독
        if (DungeonProgress.Instance != null)
        {
            DungeonProgress.Instance.OnAllMonsterRoomsCleared += HandleAllMonsterRoomsCleared;
        }
    }

    private void OnDisable()
    {
        // DungeonProgress.Instance의 이벤트 구독 해제
        if (DungeonProgress.Instance != null)
        {
            DungeonProgress.Instance.OnAllMonsterRoomsCleared -= HandleAllMonsterRoomsCleared;
        }

        if (_bossInstance != null)
            _bossInstance.OnDeath -= HandleBossDeath;
    }
    private void HandleAllMonsterRoomsCleared()
    {
        Debug.Log("[BossRoomBattle] 모든 몬스터 방 클리어됨. 보스방 해금.");
        _unlocked = true;

        if (roomController != null)
            roomController.OpenAllLinkedDoors(); // 문 열기
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_unlocked) return;     
        if (_entered || _cleared) return;
        if (!other.CompareTag("Player")) return;

        _entered = true;
        
        if (roomController != null)
            roomController.CloseAllLinkedDoors();

        SpawnBoss();
    }

    private void SpawnBoss()
    {
        if (!bossPrefab)
        {
            Debug.LogWarning("[BossRoomBattle] bossPrefab 비어있음");
            return;
        }

        Vector3 spawnPos = transform.position;

        var go = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        _bossInstance = go.GetComponent<BaseEnemy>();

        if (_bossInstance == null)
        {
            Debug.LogWarning("[BossRoomBattle] bossPrefab에 BaseEnemy 없음");
            return;
        }

        _bossInstance.OnDeath += HandleBossDeath;
    }
    
    private void HandleBossDeath(BaseEnemy boss)
    {
        if (_cleared) return;
        _cleared = true;

        boss.OnDeath -= HandleBossDeath;

        // 보스방 문 다시 열기 (출구/포탈 등)
        if (roomController != null)
            roomController.OpenAllLinkedDoors();

        Debug.Log("[BossRoomBattle] 보스 처치, 보스방 클리어");
    }
}
