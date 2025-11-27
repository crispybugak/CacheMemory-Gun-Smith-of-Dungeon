using UnityEngine;

[CreateAssetMenu(fileName = "AgentStaminaSO", menuName = "Scriptable Objects/AgentStaminaSO")]
public class AgentStaminaSO : ScriptableObject
{
    [Header("Rates")]
    [field: SerializeField] public float _useStaminaGage { get; private set; }
    [field: SerializeField] public float _rechargeSpeed { get; private set; }
    [field: SerializeField] public float _backFollowStaminaBar { get; private set; }
    [field: SerializeField] public float _backBarRechargeSpeed { get; private set; }

    [Header("Move")]
    [field: SerializeField] public float _defaultSpeed { get; private set; }
    [field: SerializeField] public float _runSpeed { get; private set; }
}
