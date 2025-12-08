using Pathfinding;
using UnityEngine;

public class RangedChaseState : IEnemyState
{
    private Enemy enemy;
    private float rangeMin;
    private float rangeMax;
    private AIDestinationSetter aiDestinationSetter;

    private Transform retreatPoint;

    public RangedChaseState(Enemy enemy, float min, float max)
    {
        this.enemy = enemy;
        rangeMin = min;
        rangeMax = max;
        aiDestinationSetter = enemy.GetComponent<AIDestinationSetter>();

        var go = new GameObject("[RetreatPoint]");
        go.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
        retreatPoint = go.transform;
    }

    public void Enter() { }

    public void Execute()
    {
        Transform target = enemy.GetTarget();
        
        if (target == null || !enemy.IsInDetectionRange(target))
        {
            enemy.stateMachine.ChangeState(EnemyStateType.Patrol);
            return;
        }

        float distToTarget = Vector2.Distance(enemy.transform.position, target.position);

        if (distToTarget > rangeMax)
        {
            if (aiDestinationSetter != null)
                aiDestinationSetter.target = target;
        }
        else if (distToTarget < rangeMin)
        {
            if (aiDestinationSetter != null)
            {
                Vector2 awayDir = (enemy.transform.position - target.position).normalized;

                float retreatDistance = Mathf.Clamp(rangeMin - distToTarget + 0.5f, 0.5f, 3f);
                Vector3 retreatPos = enemy.transform.position + (Vector3)(awayDir * retreatDistance);

                retreatPoint.position = retreatPos;
                aiDestinationSetter.target = retreatPoint;
            }
        }
        else
        {
            if (aiDestinationSetter != null)
                aiDestinationSetter.target = null;

            enemy.StopMovement();
            enemy.stateMachine.ChangeState(EnemyStateType.Attack);
        }
    }

    public void Exit()
    {
        if (aiDestinationSetter != null && aiDestinationSetter.target == retreatPoint)
            aiDestinationSetter.target = null;
        
        enemy.StopMovement();
    }
}
