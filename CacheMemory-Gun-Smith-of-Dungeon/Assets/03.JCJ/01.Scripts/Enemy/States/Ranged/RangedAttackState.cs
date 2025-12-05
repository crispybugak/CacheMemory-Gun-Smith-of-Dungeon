using UnityEngine;

public class RangedAttackState : IEnemyState
{
    private Enemy enemy;
    private RangedEnemy rangedEnemy;

    public RangedAttackState(Enemy enemy)
    {
        this.enemy = enemy;
        rangedEnemy = enemy as RangedEnemy;
    }

    public void Enter()
    {
        
    }
    public void Execute()
    {
        Transform target = enemy.GetTarget();
        
        if (target == null || !enemy.IsInDetectionRange(target))
        {
            enemy.stateMachine.ChangeState(EnemyStateType.Idle);
            return;
        }

        // 범위 유지
        float distToTarget = Vector2.Distance(enemy.transform.position, target.position);
        if (distToTarget > 12f) // rangeMax
        {
            enemy.stateMachine.ChangeState(EnemyStateType.Chase);
            return;
        }

        rangedEnemy?.Shoot();
    }

    public void Exit() { }
}