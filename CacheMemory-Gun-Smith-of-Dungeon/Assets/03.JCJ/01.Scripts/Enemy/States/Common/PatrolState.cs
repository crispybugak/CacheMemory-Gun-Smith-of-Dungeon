using UnityEngine;
using Pathfinding;

public class PatrolState : IEnemyState
{
    private Enemy enemy;
    private AIPath aiPath;

    private Transform pointA;
    private Transform pointB;
    private Transform currentTarget;

    private bool goingToA = true;
    private float arriveDistance = 0.3f;

    public PatrolState(Enemy enemy)
    {
        this.enemy = enemy;
        aiPath = enemy.GetComponent<AIPath>();
    }

    public void Enter()
    {
        if (!enemy.canPatrol) return;

        enemy.RestoreOriginalSpeed();

        var patrol = enemy.GetPatrolPoint();
        if (patrol == null || !patrol.IsValid()) return;

        pointA = patrol.pointA;
        pointB = patrol.pointB;

        currentTarget = goingToA ? pointA : pointB;
        enemy.SetAIDestination(currentTarget, "Patrol.Enter");

        if (aiPath != null)
            aiPath.endReachedDistance = 0.3f;
    }

    public void Execute()
    {
        if (!enemy.canPatrol) return;

        Transform t = enemy.GetTarget();
        if (t != null && enemy.HasPlayerInSight())
        {
            enemy.stateMachine.ChangeState(EnemyStateType.Chase);
            return;
        }

        if (currentTarget == null || aiPath == null) return;

        float dist = Vector2.Distance(enemy.transform.position, currentTarget.position);

        if (aiPath.reachedEndOfPath && dist <= arriveDistance)
        {
            goingToA = !goingToA;
            currentTarget = goingToA ? pointA : pointB;
            enemy.SetAIDestination(currentTarget, "Patrol.NextPoint");
        }
    }

    public void Exit() { }
}