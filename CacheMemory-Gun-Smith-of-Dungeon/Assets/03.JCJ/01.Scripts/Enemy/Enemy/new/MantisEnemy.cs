using UnityEngine;

public class MentisEnemy : BaseEnemy
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
        // 독 공격 로직 (필요시 추가)
    }

    protected override void ApplyAttackDamage()
    {
        base.ApplyAttackDamage();
        animator?.SetBool(hashIsAttacking, false);
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