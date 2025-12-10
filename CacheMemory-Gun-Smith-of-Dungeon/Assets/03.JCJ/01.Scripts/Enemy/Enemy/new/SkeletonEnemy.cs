using UnityEngine;

public class SkeletonEnemy : BaseEnemy
{
    protected override void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 
            GetEnemyData().attackRange);
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                TryDamagePlayer(GetEnemyData().attackDamage);
            }
        }
    }
}