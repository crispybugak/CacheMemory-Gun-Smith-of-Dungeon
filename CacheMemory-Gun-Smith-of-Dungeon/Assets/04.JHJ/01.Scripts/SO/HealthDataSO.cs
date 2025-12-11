using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "SO/Health")]
public class HealthDataSO : ScriptableObject
{
    [Header("Health")]
    [field: SerializeField] public float Maxhealth { get; private set; }

    [Header("Self-healing-Setting")]
    [field: SerializeField] public float HealingDelay { get; private set; } = 5f;
    [field: SerializeField] public float HealAmountPercent { get; private set; } = 5f;
    [field: SerializeField] public float HealingInterval { get; private set; } = 1f;

    public HealthSaveData ToSaveData()
    {
        return new HealthSaveData
        {
            maxHealth = Maxhealth,
            healingDelay = HealingDelay,
            healAmountPercent = HealAmountPercent,
            healingInterval = HealingInterval
        };
    }

    public void ApplySaveData(HealthSaveData data)
    {
        if (data == null) return;

        Maxhealth        = data.maxHealth;
        HealingDelay     = data.healingDelay;
        HealAmountPercent = data.healAmountPercent;
        HealingInterval  = data.healingInterval;
    }
}