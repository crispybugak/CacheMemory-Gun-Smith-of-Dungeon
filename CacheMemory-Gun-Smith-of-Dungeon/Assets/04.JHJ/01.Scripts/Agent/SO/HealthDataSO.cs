using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "SO/Health")]
public class HealthDataSO : ScriptableObject
{
    [field: SerializeField] public float Maxhealth { get;private set; } = 100f;

    [Header("Self-healing-Setting")]
    [field: SerializeField] public float HealingDelay { get; private set; } = 5f;
    [field: SerializeField] public float HealAmountPercent { get; private set; } = 5f;
    [field: SerializeField] public float HealInterval { get; private set; } = 1f;
}
