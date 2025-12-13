using UnityEngine;
using System.Collections;

public class NecromancerBoss : BaseEnemy
{
    private int hashIsAttacking;
    private int hashSpawnTrigger;
    private int minionSpawned = 0;

    [SerializeField] private GameObject minionPrefab;

    protected override void Start()
    {
        base.Start();
        isRangedEnemy = true;
        hashIsAttacking = Animator.StringToHash("isAttacking");
        hashSpawnTrigger = Animator.StringToHash("spawnTrigger");
        attackSoundName = "necromencer-charge";
        deathSoundName = "necromancer-dead"; 
    }

    protected override void Attack()
    {
        animator?.SetBool(hashIsAttacking, true);
        PlayAttackSound();  // ← 추가!
        PerformAttack();
    }

    protected override void PerformAttack()
    {
        StartCoroutine(LaunchProjectileSequence());

        if (minionSpawned < GetEnemyData().maxMinions && 
            Time.time - lastSpecialTime >= GetEnemyData().specialAbilityCooldown)
        {
            animator?.SetTrigger(hashSpawnTrigger);
            PlaySummonSound();  // ← 추가!
            lastSpecialTime = Time.time;
            minionSpawned++;
            StartCoroutine(SpawnMinionAfterAnimation(0.5f));
        }
    }

    private IEnumerator LaunchProjectileSequence()
    {
        yield return new WaitForSeconds(2.5f);
    
        // 발사할 때만 사운드!
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("necromencer-charge");
        }
    
        LaunchProjectile();
    }

    protected override void ApplyAttackDamage()
    {
        base.ApplyAttackDamage();
        animator?.SetBool(hashIsAttacking, false);
    }

    private void LaunchProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning($"{name}: projectilePrefab이 없습니다");
            return;
        }

        if (GetPlayerTransform() == null) return;

        Vector2 direction = (GetPlayerTransform().position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + (Vector3)direction * 1.2f;

        Projectile projectile = null;

        if (ProjectilePool.Instance != null)
        {
            projectile = ProjectilePool.Instance
                .Get(projectilePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            projectile = projectileObj.GetComponent<Projectile>();
        }

        if (projectile != null)
        {
            projectile.Launch(direction, GetEnemyData().specialAbilityDamage, projectileSpeed);
        }
        else
        {
            Debug.LogError($"{name}: Projectile 스크립트가 없습니다");
        }
    }

    private void PlaySummonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("necromencer-spawn");
        }
    }

    private IEnumerator SpawnMinionAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (minionPrefab != null)
        {
            Vector2 spawnOffset = Random.insideUnitCircle * 1.5f;
            Instantiate(minionPrefab, transform.position + (Vector3)spawnOffset, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"{name}: minionPrefab이 없습니다");
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
    }

    public void ResetMinionCount() => minionSpawned = 0;
}
