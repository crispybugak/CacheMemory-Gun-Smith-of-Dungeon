using UnityEngine;
using System.Collections;

public class BombEnemy : BaseEnemy
{
    private int hashIsExploding;
    private bool isExploding = false;
    [SerializeField] private float explosionWarningTime = 0.1f; // ✅ 0.3초 → 0.1초로 단축

    protected override void Start()
    {
        base.Start();
        hashIsExploding = Animator.StringToHash("isExploding");
    }

    protected override void Attack()
    {
        if (isExploding) return;
        
        animator?.SetBool(hashIsExploding, true);
        StartCoroutine(ExplodeSequence());
    }

    protected override void PerformAttack()
    {
        // Bomber는 Attack에서 처리
    }

    private IEnumerator ExplodeSequence()
    {
        isExploding = true;
        moveDirection = Vector2.zero;
        
        ApplyExplosionDamage();
        if (GetAnimator() != null)
            GetAnimator().SetTrigger("isDead");
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    private void ApplyExplosionDamage()
    {
        if (GetPlayerTransform() == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            GetEnemyData().explosionRadius,
            LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                TryDamagePlayer(GetEnemyData().specialAbilityDamage);
                break;
            }
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