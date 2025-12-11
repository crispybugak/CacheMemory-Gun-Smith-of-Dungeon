using UnityEngine;

public class SlimeEnemy : BaseEnemy
{
    private int hashIsAbility;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        hashIsAbility = Animator.StringToHash("isAbility");
        
        Debug.Log($"{name}: SlimeEnemy 원거리 공격 활성화!");
    }

    // ✅ 수정: BaseEnemy에서 호출되지 않으면 직접 공격 시도
    protected override void Update()
    {
        base.Update();
        
        // ✅ 추가 디버그: 공격 시도 상태 확인
        if (!isAttacking && GetPlayerTransform() != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, GetPlayerTransform().position);
            
            if (distToPlayer <= GetEnemyData().attackRange)
            {
                Debug.Log($"{name}: 공격 범위 내! 거리: {distToPlayer}, 공격 범위: {GetEnemyData().attackRange}");
            }
        }
    }

    protected override void Attack()
    {
        Debug.Log($"{name}: Attack() 호출됨");
        isAttacking = true;
        animator?.SetBool(hashIsAbility, true);
        PerformAttack();
    }

    // ✅ 원거리 공격: 발사체 발사
    protected override void PerformAttack()
    {
        Debug.Log($"{name}: PerformAttack() 호출됨");
        LaunchAcidProjectile();
    }

    protected override void ApplyAttackDamage()
    {
        Debug.Log($"{name}: ApplyAttackDamage() 호출됨");
        // 슬라임은 발사체로 데미지를 주므로 여기서는 스킵
        animator?.SetBool(hashIsAbility, false);
        isAttacking = false;
    }

    // ✅ 산성 발사체 발사
    private void LaunchAcidProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"{name}: projectilePrefab이 없습니다! Inspector에서 Projectile Prefab을 할당하세요!");
            return;
        }

        if (GetPlayerTransform() == null)
        {
            Debug.LogError($"{name}: 플레이어를 찾을 수 없습니다!");
            return;
        }

        Vector2 direction = (GetPlayerTransform().position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + (Vector3)direction * 1.2f;
        
        Debug.Log($"{name}: 발사체 발사! 방향: {direction}");
        
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            Debug.Log($"{name}: Projectile 발사 성공! 데미지: {GetEnemyData().attackDamage}");
            projectile.Launch(direction, GetEnemyData().attackDamage, projectileSpeed);
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
}
