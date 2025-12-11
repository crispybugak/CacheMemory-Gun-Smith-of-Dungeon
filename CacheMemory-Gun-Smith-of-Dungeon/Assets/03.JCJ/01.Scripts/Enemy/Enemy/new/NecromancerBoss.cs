using UnityEngine;
using System.Collections;

public class NecromancerBoss : BaseEnemy
{
    private int hashIsAttacking;
    private int hashSpawnTrigger;
    private int minionSpawned = 0;

    protected override void Start()
    {
        base.Start();
        hashIsAttacking = Animator.StringToHash("isAttacking");
        hashSpawnTrigger = Animator.StringToHash("spawnTrigger");
    }

    protected override void Attack()
    {
        animator?.SetBool(hashIsAttacking, true);
        PerformAttack();
    }

    protected override void PerformAttack()
    {
        LaunchProjectile();

        if (minionSpawned < GetEnemyData().maxMinions && 
            Time.time - lastSpecialTime >= GetEnemyData().specialAbilityCooldown)
        {
            animator?.SetTrigger(hashSpawnTrigger);
            lastSpecialTime = Time.time;
            minionSpawned++;
        }
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
            Debug.LogWarning($"{name}: projectilePrefab이 없습니다!");
            return;
        }

        Vector2 direction = (GetPlayerTransform().position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + (Vector3)direction * 1.2f;
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Launch(direction, GetEnemyData().specialAbilityDamage, projectileSpeed);
        }
        else
        {
            Debug.LogError($"{name}: Projectile 스크립트가 없습니다!");
            Destroy(projectileObj);
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (GetAnimator() != null)
            GetAnimator().SetTrigger("isHurt");
    }

    protected override void Die()
    {
        if (GetAnimator() != null)
            GetAnimator().SetTrigger("isDead");
        base.Die();
    }

    public void ResetMinionCount() => minionSpawned = 0;
}
