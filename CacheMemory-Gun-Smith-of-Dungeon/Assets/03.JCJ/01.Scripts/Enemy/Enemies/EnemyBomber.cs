using UnityEngine;

public class EnemyBomber : Enemy
{
    [Header("Bomber")]
    public float explosionRange = 2.5f;
    public float explosionDelay = 0.5f;
    private bool isExploding = false;

    protected override void InitializeStateMachine()
    {
        stateMachine = new EnemyStateMachine();
        stateMachine.AddState(EnemyStateType.Patrol, new PatrolState(this));
        stateMachine.AddState(EnemyStateType.Chase, new EnemyBomberDashState(this, explosionRange));
        stateMachine.AddState(EnemyStateType.Attack, new EnemyBomberExplodeState(this));
        stateMachine.AddState(EnemyStateType.Dead, new DeadState(this));
        
        stateMachine.ChangeState(EnemyStateType.Patrol);
    }

    public void TriggerExplosion()
    {
        if (isExploding) return;
        
        isExploding = true;
        Invoke(nameof(Explode), explosionDelay);
    }

    private void Explode()
    {
        Debug.Log("Bomb!");
        
        // 폭발 이펙트, 사운드
        
        isAlive = false;
        Destroy(gameObject);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        
        // 폭탄병은 체력이 0이 되면 자폭
        if (currentHealth <= 0 && !isExploding)
        {
            TriggerExplosion();
        }
    }

    public float GetExplosionRange() => explosionRange;
}