using UnityEngine;

public class EnemyBomberDashState : IEnemyState
{
    private Enemy enemy;
    private float explosionRange;

    public EnemyBomberDashState(Enemy enemy, float explosionRange)
    {
        this.enemy = enemy;
        this.explosionRange = explosionRange;
    }

    public void Enter()
    {
        enemy.SetCanPatrol(false);
        enemy.SetChaseSpeed();
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

        // 폭탄병은 폭발 범위 내에 들어가면 Attack으로
        if (Vector2.Distance(enemy.transform.position, t.position) <= explosionRange)
        {
            enemy.stateMachine.ChangeState(EnemyStateType.Attack);
            return;
        }

        enemy.SetAIDestination(t, "BomberDash.Execute");
    }

    public void Exit()
    {
        enemy.RestoreOriginalSpeed();
    }
}