using UnityEngine;
using System.Collections;

public class BoarEnemy : BaseEnemy
{
    private int hashSleepTransition;

    private bool isSleeping = false;

    protected override void Start()
    {
        base.Start();
        hashSleepTransition = Animator.StringToHash("sleepTransition");
        attackSoundName = "boar-attack-sound";
    }

    protected override void Attack()
    {
        if (isSleeping) return;

        PerformAttack();
    }

    protected override void ApplyAttackDamage()
    {
        base.ApplyAttackDamage();
        StartCoroutine(SleepAfterAttack());
    }

    private IEnumerator SleepAfterAttack()
    {
        yield return new WaitForSeconds(0.5f);

        isSleeping = true;

        if (GetAnimator() != null)
            GetAnimator().SetTrigger(hashSleepTransition);

        yield return new WaitForSeconds(GetEnemyData().sleepDuration);

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