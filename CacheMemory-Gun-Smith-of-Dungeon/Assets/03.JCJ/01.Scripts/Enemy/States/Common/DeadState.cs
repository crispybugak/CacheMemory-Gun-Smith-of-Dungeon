public class DeadState : IEnemyState
{
    private Enemy enemy;

    public DeadState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.StopMovement();
        // 사망 애니메이션, 이펙트 등
    }

    public void Execute() { }

    public void Exit() { }
}