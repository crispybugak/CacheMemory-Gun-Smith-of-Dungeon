using UnityEngine;

public class Agent : MonoBehaviour
{
    public Rigidbody2D RidCompo { get; set; }
    public AgentInput InputCompo { get; set; }
    public AgentMovement MoveCompo { get; set; }
    [field:SerializeField] public AgentMovementSO MovementSOCompo { get; private set; }
    [field:SerializeField] public HealthDataSO HealthDataSOCompo { get; private set; }

    private Transform agentVisual; 

    private void Awake()
    {
        InputCompo = GetComponent<AgentInput>();
        MoveCompo = GetComponent<AgentMovement>();
        RidCompo = GetComponent<Rigidbody2D>();
        agentVisual = transform.Find("AgentVisual");
    }
    private void Start()
    {
    }
}
