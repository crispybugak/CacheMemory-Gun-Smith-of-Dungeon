using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerBoss : BaseEnemy
{
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject shadowBoltPrefab;
    private float lastSpawnTime = -999f;
    private readonly List<GameObject> minions = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        minions.RemoveAll(m => m == null);
        if (GetEnemyData() != null &&
            Time.time - lastSpawnTime > GetEnemyData().spawnCooldown &&
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
            if (minion.TryGetComponent<BaseEnemy>(out var minionEnemy))
            {
                minionEnemy.EnemyData = GetEnemyData();
            }
            minions.Add(minion);
        }
    }

    protected override void PerformAttack()
    {
        if (shadowBoltPrefab == null || GetPlayerTransform() == null) return;
        Vector2 shootDir = ((Vector2)GetPlayerTransform().position - (Vector2)transform.position).normalized;
        Vector3 spawnOffset = (Vector3)shootDir * 0.5f;
        GameObject bolt = Instantiate(shadowBoltPrefab, transform.position + spawnOffset, Quaternion.identity);
        if (bolt.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.Launch(shootDir, GetEnemyData().specialAbilityDamage, projectileSpeed);
        }
    }

    protected override void Die()
    {
        base.Die();
        foreach (var minion in minions)
        {
            if (minion != null)
            {
                if (minion.TryGetComponent<BaseEnemy>(out var enemy))
                    enemy.TakeDamage(9999f);
                else
                    Destroy(minion);
            }
        }
        minions.Clear();
    }
}