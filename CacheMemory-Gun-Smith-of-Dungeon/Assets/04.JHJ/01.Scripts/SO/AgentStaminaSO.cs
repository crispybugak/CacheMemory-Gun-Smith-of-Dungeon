using System;
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
            useStaminaGage = _useStaminaGage,
            rechargeSpeed = _rechargeSpeed,
            backFollowStaminaBar = _backFollowStaminaBar,
            backBarRechargeSpeed = _backBarRechargeSpeed,
            defaultSpeed = _defaultSpeed,
            runSpeed = _runSpeed
        };
    }

    public void ApplySaveData(SaveData data)
    {
        _useStaminaGage = data.useStaminaGage;
        _rechargeSpeed = data.rechargeSpeed;
        _backFollowStaminaBar = data.backFollowStaminaBar;
        _backBarRechargeSpeed = data.backBarRechargeSpeed;
        _defaultSpeed = data.defaultSpeed;
        _runSpeed = data.runSpeed;
    }
}