using System.Collections;
using UnityEngine;

public class BombEnemy : BaseEnemy
{
    private bool isExploding;

    protected override void Start()
    {
        base.Start();
    }

    protected override void PerformAttack()
    {
        if (isExploding) return;
        Explode();
    }

    private void Explode()
    {
        isExploding = true;
        if (GetAnimator() != null)
            GetAnimator().SetBool("isExploding", true);

        StartCoroutine(ExplosionCoroutine());
    }

    private IEnumerator ExplosionCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            GetEnemyData().explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                TryDamagePlayer(GetEnemyData().attackDamage);

                if (hit.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    Vector2 knockbackDir = ((Vector2)hit.transform.position -
                                            (Vector2)transform.position).normalized;
                    rb.linearVelocity += knockbackDir * GetEnemyData().explosionForce;
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (GetEnemyData().maxHealth - Mathf.RoundToInt(damage) <= 0)
        {
            Explode();
        }
    }

    protected override void Die()
    {
        // Bomb은 자폭으로만 제거됨
    }
}