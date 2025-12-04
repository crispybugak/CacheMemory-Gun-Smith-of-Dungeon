using UnityEngine;

public class MeleeEnemy : Enemy
{
    protected override void InitializeStateMachine()
    {
        stateMachine = new EnemyStateMachine();
        stateMachine.AddState(EnemyStateType.Patrol, new PatrolState(this));
        stateMachine.AddState(EnemyStateType.Chase, new MeleeChaseState(this));
        stateMachine.AddState(EnemyStateType.Attack, new MeleeAttackState(this));
        stateMachine.AddState(EnemyStateType.Dead, new DeadState(this));
        
        stateMachine.ChangeState(EnemyStateType.Patrol);
    }

    public void ExecuteMeleeAttack()
    {
        if (!CanAttack()) return;
        SetLastAttackTime();

        if (IsInAttackRange(targetTransform))
        {
            var playerHealth = targetTransform.GetComponent<Health>();
            if (playerHealth != null)
            {
                float damage = config.GetStats().attackPower;
                playerHealth.OnDamaged(damage);
            }

            OnAttackExecuted();
        }
    }


    protected virtual void OnAttackExecuted() { }
}