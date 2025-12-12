using UnityEngine.Events;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    private Agent _agent;
    private Stamina _stamina;

    public UnityEvent<float> onMove;
    public UnityEvent<Vector2> onRenderer;

    private AgentAnimation _agentAnimation;

    private Vector2 _moveInput;

    private void Awake()
    {
        _agent = GetComponent<Agent>();
        _stamina = GetComponent<Stamina>();
        _agentAnimation = GetComponentInChildren<AgentAnimation>();
    }

    private void OnEnable()
    {
        _agent.MovementSOCompo.OnMovePressed += OnMoveInput;   
        _agent.MovementSOCompo.OnRunPressed += _stamina.SetRunning;
    }

    private void OnDisable()
    {
        _agent.MovementSOCompo.OnMovePressed -= OnMoveInput;
        _agent.MovementSOCompo.OnRunPressed -= _stamina.SetRunning;
    }


    public void OnMoveInput()
    {
        _moveInput = _agent.MovementSOCompo.inputcDir.normalized;
    }

    private void Update()
    {
        onRenderer?.Invoke(_moveInput);

        _agent.RidCompo.linearVelocity = _stamina.AgentStaminaData.MoveSpeed * _moveInput;

        float moveAmount = _moveInput.magnitude;
        onMove?.Invoke(moveAmount);

        _agentAnimation.Animate(_moveInput, moveAmount);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Update();
    }

    public Vector2 IsMoved()
    {
        return _moveInput;
    }
}
