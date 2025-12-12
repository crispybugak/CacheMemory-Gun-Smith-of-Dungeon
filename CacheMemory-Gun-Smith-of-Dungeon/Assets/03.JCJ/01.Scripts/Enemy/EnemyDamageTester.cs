using UnityEngine;

public class EnemyDamageTester : MonoBehaviour
{
    [SerializeField] private float radius = 10f;
    [SerializeField] private float interval = 1f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private LayerMask enemyLayer;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            DamageEnemiesInRange();
        }
    }

    private void DamageEnemiesInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<BaseEnemy>(out var enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}