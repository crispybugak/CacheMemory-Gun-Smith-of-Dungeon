using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class AgentMovement : MonoBehaviour
{

    private Agent _agent;
    private Stamina _stamina;

    [field : SerializeField] public float MoveSpeed { get;  set; }
    private void Awake()
    { 
        _agent = GetComponent<Agent>();
        _stamina = GetComponent<Stamina>();
    }


    private void OnEnable()
    {
        _agent.MovementSOCompo.OnMovePressed += OnMove;
        _agent.MovementSOCompo.OnRunPressed += _stamina.SetRunning;
    }
    public void OnMove()
    {
        Vector2 input = _agent.MovementSOCompo.inputcDir.normalized;
        _agent.RidCompo.linearVelocity = MoveSpeed * input;
    }

    private void OnDisable()
    {
        _agent.MovementSOCompo.OnMovePressed -= OnMove;
        _agent.MovementSOCompo.OnRunPressed -= _stamina.SetRunning;
    }


}
