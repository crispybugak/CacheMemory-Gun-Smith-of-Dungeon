using System.Collections;
using UnityEngine;

public class SlimeEnemy : BaseEnemy
{
    [SerializeField] private GameObject acidBlobPrefab;
    
    private float lastAbilityTime = -999f;
    
    protected override void Update()
    {
        if (GetPlayerTransform() == null) return;
        
        float sqrDistToPlayer = ((Vector2)transform.position - 
            (Vector2)GetPlayerTransform().position).sqrMagnitude;
        
        float sqrAbilityRange = GetEnemyData().specialAbilityRange * 
            GetEnemyData().specialAbilityRange;
        
        if (sqrDistToPlayer < sqrAbilityRange && 
            Time.time - lastAbilityTime > GetEnemyData().specialAbilityCooldown)
        {
            CastAbility();
            return;
        }
        
        base.Update();
    }
    
    private void CastAbility()
    {
        lastAbilityTime = Time.time;
        if (GetAnimator() != null)
            GetAnimator().SetBool("isAbility", true);
        
        StartCoroutine(ShootCoroutine());
    }
    
    private IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        
        Vector2 shootDir = ((Vector2)GetPlayerTransform().position - 
            (Vector2)transform.position).normalized;
        
        if (acidBlobPrefab != null)
        {
            GameObject blob = Instantiate(acidBlobPrefab, 
                transform.position + (Vector3)shootDir * 0.5f, 
                Quaternion.identity);
            
            if (blob.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Launch(shootDir, GetEnemyData().specialAbilityDamage);
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        if (GetAnimator() != null)
            GetAnimator().SetBool("isAbility", false);
    }
    
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