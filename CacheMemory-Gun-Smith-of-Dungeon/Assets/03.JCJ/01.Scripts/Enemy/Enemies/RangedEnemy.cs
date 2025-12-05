using UnityEngine;

public class RangedEnemy : Enemy
{
    public float rangeMin = 7f;
    public float rangeMax = 12f;

    protected override void InitializeStateMachine()
    {
        stateMachine = new EnemyStateMachine();
        stateMachine.AddState(EnemyStateType.Patrol, new PatrolState(this));
        stateMachine.AddState(EnemyStateType.Chase, new RangedChaseState(this, rangeMin, rangeMax));
        stateMachine.AddState(EnemyStateType.Attack, new RangedAttackState(this));
        stateMachine.AddState(EnemyStateType.Dead, new DeadState(this));
    
        stateMachine.ChangeState(EnemyStateType.Patrol);
    }


    public void Shoot()
    {
        if (!CanAttack()) return;
        if (config.GetCombatStats().canAttack == false) return;

        SetLastAttackTime();

        if (IsInAttackRange(targetTransform))
        {
            Vector2 dir = (targetTransform.position - transform.position).normalized;
        
            GameObject bulletObj = Instantiate(
                config.GetBulletPrefab(),
                transform.position,
                Quaternion.identity
            );

            var bullet = bulletObj.GetComponent<EnemyBullet>();
            if (bullet != null)
                bullet.Setup(dir);
        }
    }

}