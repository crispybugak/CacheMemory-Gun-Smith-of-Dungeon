using Pathfinding;
using UnityEngine;

public class MeleeAttackState : IEnemyState
{
    private Enemy enemy;
    private MeleeEnemy meleeEnemy;
    private AIDestinationSetter aiDestinationSetter;

    public MeleeAttackState(Enemy enemy)
    {
        this.enemy = enemy;
        meleeEnemy = enemy as MeleeEnemy;
        aiDestinationSetter = enemy.GetComponent<AIDestinationSetter>();
    }

    public void Enter()
    {
        if (aiDestinationSetter != null)
        {
            aiDestinationSetter.target = null;
        }
        enemy.StopMovement();
    }

    public void Execute()
    {
        Transform target = enemy.GetTarget();
        
        if (target == null || !enemy.IsInDetectionRange(target))
        {
            enemy.stateMachine.ChangeState(EnemyStateType.Patrol);
            return;
        }

        if (!enemy.IsInAttackRange(target))
        {
            enemy.stateMachine.ChangeState(EnemyStateType.Chase);
            return;
        }

        meleeEnemy?.ExecuteMeleeAttack();
    }

    public void Exit() { }
}