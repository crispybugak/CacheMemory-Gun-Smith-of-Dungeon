using Unity.VisualScripting;
using UnityEngine;

public class AgentInput : MonoBehaviour
{
    private Agent _agent { get; set; }
    private AgentMovement _agentMovement { get; set; }

    private void Awake()
    {
        _agent = GetComponent<Agent>();
        _agentMovement = GetComponent<AgentMovement>();
        _agentMovement = _agent.MoveCompo;
    }
}
