using UnityEngine;

[RequireComponent(typeof(BaseEnemy))]
public class EnemyDropper : MonoBehaviour
{
    [Header("드랍 설정")]
    [SerializeField] private GameObject dropPrefab;  // 드랍할 프리팹 하나
    [SerializeField] private int minCount = 0;       // 최소 개수
    [SerializeField] private int maxCount = 3;       // 최대 개수 (포함)
    
    [Header("드랍 위치 설정")]
    [SerializeField] private Vector2 offset = Vector2.zero; // 적 기준 기본 오프셋
    [SerializeField] private float spreadRadius = 0.5f;     // 주변으로 흩뿌리기 반경

    private BaseEnemy enemy;

    private void Awake()
    {
        enemy = GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.OnDeath += HandleEnemyDeath;
        }
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnDeath -= HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(BaseEnemy dead)
    {
        if (dropPrefab == null) return;

        int min = Mathf.Max(0, minCount);
        int max = Mathf.Max(min, maxCount);

        int count = Random.Range(min, max + 1);

        Vector2 basePos = dead.transform.position;

        for (int i = 0; i < count; i++)
        {
            Vector2 randomOffset = offset + Random.insideUnitCircle * spreadRadius;
            Vector3 spawnPos = basePos + (Vector2)randomOffset;

            Instantiate(dropPrefab, spawnPos, Quaternion.identity);
        }
    }
    
}