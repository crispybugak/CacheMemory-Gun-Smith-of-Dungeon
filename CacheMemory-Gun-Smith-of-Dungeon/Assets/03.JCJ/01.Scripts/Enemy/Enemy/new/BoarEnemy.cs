using System.Collections;
using UnityEngine;
using Pathfinding;

public class BoarEnemy : BaseEnemy
{
    private bool isSleeping;
    private float sleepTimer;
    
    private AIPath _aiPath;
    
    protected override void Start()
    {
        base.Start();
        _aiPath = GetComponent<AIPath>();
    }
    
    protected override void Update()
    {
        if (isSleeping)
        {
            sleepTimer -= Time.deltaTime;
            if (sleepTimer <= 0)
            {
                WakeUp();
            }
            return;  // 자는 동안 다른 업데이트 안 함
        }
        
        base.Update();
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
                TryDamagePlayer((int)(GetEnemyData().attackDamage * 1.6f));
                
                if (!isSleeping)
                {
                    Sleep();
                }
            }
        }
    }
    
    private void Sleep()
    {
        isSleeping = true;
        sleepTimer = GetEnemyData().sleepDuration;
        
        // 자는 동안 이동 멈추기
        if (_aiPath != null)
        {
            _aiPath.canMove = false;
        }
        
        if (GetAnimator() != null)
        {
            GetAnimator().SetBool("isSleeping", true);
            GetAnimator().SetTrigger("sleepTransition");
        }
    }
    
    private void WakeUp()
    {
        isSleeping = false;
        
        // 깨어나면 이동 다시 시작
        if (_aiPath != null)
        {
            _aiPath.canMove = true;
        }
        
        if (GetAnimator() != null)
            GetAnimator().SetBool("isSleeping", false);
    }
    
    public void TakeDamage(float damage)
    {
        if (isSleeping)
            damage *= 1.5f;
        
        base.TakeDamage(damage);
    }
}