using System;
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

    // === JSON 저장용 구조체 ===
    [Serializable]
    public struct SaveData
    {
        public float maxhealth;
        public float healingDelay;
        public float healAmountPercent;
        public float healingInterval;
    }

    public SaveData ToSaveData()
    {
        return new SaveData
        {
            maxhealth = Maxhealth,
            healingDelay = HealingDelay,
            healAmountPercent = HealAmountPercent,
            healingInterval = HealingInterval
        };
    }

    public void ApplySaveData(SaveData data)
    {
        Maxhealth = data.maxhealth;
        HealingDelay = data.healingDelay;
        HealAmountPercent = data.healAmountPercent;
        HealingInterval = data.healingInterval;
    }
}