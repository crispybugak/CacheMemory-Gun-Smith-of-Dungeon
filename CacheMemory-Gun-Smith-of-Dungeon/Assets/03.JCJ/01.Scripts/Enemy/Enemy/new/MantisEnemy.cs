using UnityEngine;

public class MantisEnemy : BaseEnemy
{
    [SerializeField] private GameObject acidBlobPrefab;

    protected override void Start()
    {
        base.Start();
    }

    protected override void PerformAttack()
    {
        if (GetPlayerTransform() == null) return;
        Vector2 shootDir = ((Vector2)GetPlayerTransform().position - (Vector2)transform.position).normalized;
        if (acidBlobPrefab != null)
        {
            GameObject blob = Instantiate(acidBlobPrefab,
                transform.position + (Vector3)shootDir * 0.5f,
                Quaternion.identity);
            if (blob.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Launch(shootDir, GetEnemyData().attackDamage, projectileSpeed);
            }
        }
    }
}