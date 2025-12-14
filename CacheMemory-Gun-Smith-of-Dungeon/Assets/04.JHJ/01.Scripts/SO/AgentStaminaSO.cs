using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentStaminaSO", menuName = "Scriptable Objects/AgentStaminaSO")]
public class AgentStaminaSO : ScriptableObject
{
    [Header("Rates")]
    [field: SerializeField] public float UseStaminaGage { get; private set; }
    [field: SerializeField] public float RechargeSpeed { get; private set; }
    [field: SerializeField] public float BackFollowStaminaBar { get; private set; }
    [field: SerializeField] public float BackBarRechargeSpeed { get; private set; }

    [Header("Move")]
    [field: SerializeField] public float DefaultSpeed { get; private set; }
    [field: SerializeField] public float DefaultRunSpeed { get; private set; }
    [field: SerializeField] public float RunSpeed { get; set; }
    [field: SerializeField] public float MoveSpeed { get;  set; }

    // === JSON 저장용 구조체 ===
    [Serializable]
    public struct SaveData
    {
        public float useStaminaGage;
        public float rechargeSpeed;
        public float backFollowStaminaBar;
        public float backBarRechargeSpeed;
        public float defaultSpeed;
        public float runSpeed;
    }

    public SaveData ToSaveData()
    {
        return new SaveData
        {
            useStaminaGage = UseStaminaGage,
            rechargeSpeed = RechargeSpeed,
            backFollowStaminaBar = BackFollowStaminaBar,
            backBarRechargeSpeed = BackBarRechargeSpeed,
            defaultSpeed = DefaultSpeed,
            runSpeed = RunSpeed
        };
    }

    public void ApplySaveData(SaveData data)
    {
        UseStaminaGage = data.useStaminaGage;
        RechargeSpeed = data.rechargeSpeed;
        BackFollowStaminaBar = data.backFollowStaminaBar;
        BackBarRechargeSpeed = data.backBarRechargeSpeed;
        DefaultSpeed = data.defaultSpeed;
        RunSpeed = data.runSpeed;
    }
}