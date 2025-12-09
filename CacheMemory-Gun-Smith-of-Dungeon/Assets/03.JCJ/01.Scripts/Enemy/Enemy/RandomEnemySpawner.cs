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
    
    [SerializeField] private List<EnemySpawnData> spawnableEnemies = new List<EnemySpawnData>();
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero;
    
    private void Start()
    {
        SpawnEnemies();
    }
    
    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            EnemySpawnData selectedEnemy = GetRandomEnemy();
            Vector2 spawnPos = GetRandomSpawnPosition();
            
            GameObject enemy = Instantiate(selectedEnemy.enemyPrefab, spawnPos, Quaternion.identity);
            
            if (enemy.TryGetComponent<BaseEnemy>(out var baseEnemy))
            {
                var soField = typeof(BaseEnemy).GetField("enemyData", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                soField?.SetValue(baseEnemy, selectedEnemy.data);
            }
        }
    }
    
    private EnemySpawnData GetRandomEnemy()
    {
        int totalWeight = 0;
        foreach (var enemy in spawnableEnemies)
        {
            totalWeight += enemy.weight;
        }
        
        int random = Random.Range(0, totalWeight);
        int current = 0;
        
        foreach (var enemy in spawnableEnemies)
        {
            current += enemy.weight;
            if (random < current)
                return enemy;
        }
        
        return spawnableEnemies[0];
    }
    
    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDist = Random.Range(spawnRadius * 0.7f, spawnRadius);
        return spawnAreaCenter + randomDir * randomDist;
    }
}