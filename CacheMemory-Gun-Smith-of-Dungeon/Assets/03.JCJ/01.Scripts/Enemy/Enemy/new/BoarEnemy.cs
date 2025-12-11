using UnityEngine;
using System.Collections;

public class BoarEnemy : BaseEnemy
{
    private int hashIsAttacking;
    private int hashIsSleeping;
    private bool isSleeping = false;

    protected override void Start()
    {
        base.Start();
        hashIsAttacking = Animator.StringToHash("isAttacking");
        hashIsSleeping = Animator.StringToHash("isSleeping");
    }

    protected override void Attack()
    {
        if (isSleeping) return;
        
        animator?.SetBool(hashIsSleeping, false);
        animator?.SetBool(hashIsAttacking, true);
        PerformAttack();
    }

    protected override void PerformAttack()
    {
        // 돌진 공격 로직 (필요시 추가)
    }

    protected override void ApplyAttackDamage()
    {
        base.ApplyAttackDamage();
        animator?.SetBool(hashIsAttacking, false);
        StartCoroutine(SleepAfterAttack());
    }

    private IEnumerator SleepAfterAttack()
    {
        yield return new WaitForSeconds(0.5f);
        animator?.SetBool(hashIsSleeping, true);
        isSleeping = true;
        yield return new WaitForSeconds(GetEnemyData().sleepDuration);
        animator?.SetBool(hashIsSleeping, false);
        isSleeping = false;
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