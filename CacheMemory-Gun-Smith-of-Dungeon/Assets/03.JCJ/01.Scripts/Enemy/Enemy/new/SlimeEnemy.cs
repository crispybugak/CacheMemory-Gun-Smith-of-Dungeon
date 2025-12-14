using UnityEngine;

public class SlimeEnemy : BaseEnemy
{
    private int hashIsAbility;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        isRangedEnemy = true;
        hashIsAbility = Animator.StringToHash("isAbility");
        attackSoundName = "slime-lunch-sound"; 
    }

    protected override void Update()
    {
        base.Update();
        
        if (!isAttacking && GetPlayerTransform() != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, GetPlayerTransform().position);
             
            if (distToPlayer <= GetEnemyData().attackRange)
            {
                Debug.Log($"{name}: 공격 범위 내, 거리: {distToPlayer}, 공격 범위: {GetEnemyData().attackRange}");
            }
        }
    }

    protected override void Attack()
    {
        isAttacking = true;
        animator?.SetBool(hashIsAbility, true);
        PerformAttack();
    }

    protected override void PerformAttack()
    {
        LaunchAcidProjectile();
    }

    protected override void ApplyAttackDamage()
    {
        animator?.SetBool(hashIsAbility, false);
        isAttacking = false;
    }

    private void LaunchAcidProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"{name}: projectilePrefab이 없습니다 Inspector에서 Projectile Prefab을 할당하세요");
            return;
        }

        if (GetPlayerTransform() == null)
        {
            Debug.LogError($"{name}: 플레이어를 찾을 수 없습니다");
            return;
        }

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
            projectile.Launch(direction, GetEnemyData().attackDamage, projectileSpeed);
        }
        else
        {
            Debug.LogError($"{name}: Projectile 스크립트가 없습니다");
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
}