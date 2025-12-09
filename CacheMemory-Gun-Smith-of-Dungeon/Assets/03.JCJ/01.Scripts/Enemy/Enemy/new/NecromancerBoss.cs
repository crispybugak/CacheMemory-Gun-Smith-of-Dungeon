using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerBoss : BaseEnemy
{
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject shadowBoltPrefab;
    
    private float lastSpawnTime = -999f;
    private List<GameObject> minions = new List<GameObject>();
    
    protected override void Update()
    {
        minions.RemoveAll(m => m == null);
        
        if (Time.time - lastSpawnTime > GetEnemyData().spawnCooldown && 
            minions.Count < GetEnemyData().maxMinions)
        {
            SpawnMinion();
            return;
        }
        
        base.Update();
    }
    
    private void SpawnMinion()
    {
        lastSpawnTime = Time.time;
        if (GetAnimator() != null)
            GetAnimator().SetTrigger("spawnTrigger");
        
        StartCoroutine(SpawnCoroutine());
    }
    
    private IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        
        if (skeletonPrefab != null)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 1.5f;
            GameObject minion = Instantiate(skeletonPrefab, spawnPos, Quaternion.identity);
            minions.Add(minion);
        }
    }
    
    protected override void PerformAttack()
    {
        Vector2 shootDir = ((Vector2)GetPlayerTransform().position - 
                            (Vector2)transform.position).normalized;
        
        if (shadowBoltPrefab != null)
        {
            GameObject bolt = Instantiate(shadowBoltPrefab, 
                transform.position + (Vector3)shootDir * 0.5f, 
                Quaternion.identity);
            
            if (bolt.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Launch(shootDir, GetEnemyData().specialAbilityDamage);
            }
        }
    }
}