using UnityEngine.Events;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    private Agent _agent;
    private Stamina _stamina;

    public UnityEvent<float> onMove;
    public UnityEvent<Vector2> onRenderer;

    [field: SerializeField] public float MoveSpeed { get; set; }

    private Vector2 _moveInput;

    private void Awake()
    {
        _agent = GetComponent<Agent>();
        _stamina = GetComponent<Stamina>();
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
        _agent.RidCompo.linearVelocity = MoveSpeed * _moveInput;
        onMove?.Invoke(_moveInput.magnitude);
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
