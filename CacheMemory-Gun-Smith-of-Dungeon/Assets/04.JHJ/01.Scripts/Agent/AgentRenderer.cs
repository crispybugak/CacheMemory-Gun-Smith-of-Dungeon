using UnityEngine;

public class AgentRenderer : MonoBehaviour
{
    private Agent _agent;

    private void Awake()
    {
        _agent = GetComponent<Agent>();
    }

    private void Update()
    {
        Vector2 dir = (_agent.MovementSOCompo.mouseDir - (Vector2)transform.position).normalized;
        transform.right = dir; 
    }
}
