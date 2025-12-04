using UnityEngine;
using Pathfinding;

public class MeleeChaseState : IEnemyState
{
    private Enemy enemy;

    public MeleeChaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.SetCanPatrol(false);

        enemy.SetChaseSpeed();

        Transform t = enemy.GetTarget();
        if (t != null)
            enemy.SetAIDestination(t, "Chase.Enter");
    }

    public void Execute()
    {
        Transform t = enemy.GetTarget();

        if (t == null || !enemy.HasPlayerInSight())
        {
            enemy.SetCanPatrol(true);
            enemy.stateMachine.ChangeState(EnemyStateType.Patrol);
            return;
        }

        if (enemy.IsInAttackRange(t))
        {
            enemy.stateMachine.ChangeState(EnemyStateType.Attack);
            return;
        }

        enemy.SetAIDestination(t, "Chase.Execute");
    }

    public void Exit()
    {
        enemy.RestoreOriginalSpeed();
    }
}