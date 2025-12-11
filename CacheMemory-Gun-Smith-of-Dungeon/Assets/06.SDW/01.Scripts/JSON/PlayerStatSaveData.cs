using System;

[Serializable]
public class HealthSaveData
{
    public float maxHealth;
    public float healingDelay;
    public float healAmountPercent;
    public float healingInterval;
}

[Serializable]
public class StaminaSaveData
{
    public float useStaminaGage;
    public float rechargeSpeed;
    public float backFollowStaminaBar;
    public float backBarRechargeSpeed;
    public float defaultSpeed;
    public float runSpeed;
}

[Serializable]
public class PlayerStatSaveData
{
    public HealthSaveData health;
    public StaminaSaveData stamina;
    
    public string passiveName;   // PassiveSO.name
    public string skillName;     // SkillSO.name
    public string animatorName;  // RuntimeAnimatorController.name
}