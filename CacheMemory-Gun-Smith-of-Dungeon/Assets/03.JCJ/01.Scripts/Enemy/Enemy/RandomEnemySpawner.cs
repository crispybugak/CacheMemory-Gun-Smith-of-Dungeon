using System.Collections.Generic;
using UnityEngine;

public class RandomEnemySpawner : MonoBehaviour
{
    [System.Serializable]
    private class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public EnemyData data;
        public int weight = 1;
    }
    [SerializeField] private List<EnemySpawnData> spawnableEnemies = new();
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero;

    private void Start()
    {
        spawnAreaCenter = transform.position; // 스포너 위치 중심
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (spawnableEnemies == null || spawnableEnemies.Count == 0)
        {
            Debug.LogWarning("Spawnable enemies list is empty.", this);
            return;
        }
        for (int i = 0; i < enemyCount; i++)
        {
            var data = GetRandomEnemy();
            Vector2 pos = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(data.enemyPrefab, pos, Quaternion.identity);
            if (enemy.TryGetComponent<BaseEnemy>(out var baseEnemy))
            {
                baseEnemy.EnemyData = data.data;
            }
        }
    }

    private EnemySpawnData GetRandomEnemy()
    {
        int total = 0;
        foreach (var e in spawnableEnemies) total += Mathf.Max(0, e.weight);
        if (total <= 0)
        {
            Debug.LogWarning("Total weight <= 0, fallback to first enemy.", this);
            return spawnableEnemies[0];
        }
        int rand = Random.Range(0, total);
        int cum = 0;
        foreach (var e in spawnableEnemies)
        {
            cum += Mathf.Max(0, e.weight);
            if (rand < cum) return e;
        }
        return spawnableEnemies[0];
    }

    private Vector2 GetRandomSpawnPosition()
    {
        return spawnAreaCenter + Random.insideUnitCircle.normalized * Random.Range(spawnRadius * 0.7f, spawnRadius);
    }
}