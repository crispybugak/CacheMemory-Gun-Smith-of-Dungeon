using UnityEngine;

public class EnemyBomberExplodeState : IEnemyState
{
    private Enemy enemy;
    private EnemyBomber bomber;

    public EnemyBomberExplodeState(Enemy enemy)
    {
        this.enemy = enemy;
        this.bomber = enemy as EnemyBomber;
    }

    public void Enter()
    {
        enemy.StopMovement();
        bomber?.TriggerExplosion();
    }

    public void Execute() { }

    public void Exit() { }
}