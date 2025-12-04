using System.Collections.Generic;

public class EnemyStateMachine
{
    private Dictionary<EnemyStateType, IEnemyState> states = new();
    private EnemyStateType currentStateType;
    private IEnemyState currentState;

    public void AddState(EnemyStateType type, IEnemyState state)
    {
        states[type] = state;
    }

    public void ChangeState(EnemyStateType newStateType)
    {
        if (currentStateType == newStateType) return;

        currentState?.Exit();
        currentStateType = newStateType;
        currentState = states[newStateType];
        currentState?.Enter();
    }

    public void Execute() => currentState?.Execute();
    public EnemyStateType GetCurrentState() => currentStateType;
}