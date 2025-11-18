using UnityEngine;

public class AgentRenderer : MonoBehaviour
{
    [SerializeField] private Agent agent;

    private void Awake()
    {
        if  (agent == null)
            agent = GetComponent<Agent>();
    }

    private void Update()
    {
        Vector2 dir = (Camera.main.ScreenToWorldPoint(agent.MovementSOCompo.mousePos) - transform.position).normalized;
        transform.right = dir; 
    }
}
