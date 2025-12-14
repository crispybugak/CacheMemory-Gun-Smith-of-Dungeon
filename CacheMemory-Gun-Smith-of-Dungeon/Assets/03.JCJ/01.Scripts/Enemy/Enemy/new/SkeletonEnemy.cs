using UnityEngine;

public class SkeletonWarriorEnemy : BaseEnemy
{
    private int hashIsAttacking;

    protected override void Start()
    {
        base.Start();
        hashIsAttacking = Animator.StringToHash("isAttacking");
    }

    protected override void Attack()
    {
        animator?.SetBool(hashIsAttacking, true);
        PerformAttack();
    }

    protected override void PerformAttack()
    {
    }

    protected override void ApplyAttackDamage()
    {
        base.ApplyAttackDamage();
        animator?.SetBool(hashIsAttacking, false);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);  // BaseEnemy에서 PlayHurtAnim() 자동 호출
    }

    protected override void Die()
    {
        base.Die();  // BaseEnemy에서 PlayDeadAnim() 자동 호출
    }
}