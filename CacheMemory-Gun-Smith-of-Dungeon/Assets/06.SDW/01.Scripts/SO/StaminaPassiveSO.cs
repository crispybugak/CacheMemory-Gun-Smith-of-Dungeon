using UnityEngine;

namespace _06.SDW._01.Scripts.SO
{
    [CreateAssetMenu(fileName = "StaminaPassive", menuName = "SO/Passive/StaminaUp")]
    public class StaminaPassiveSO : PassiveSO
    {
        [Header("스태미나 증가량 설정")]
        [SerializeField] private float bonusMaxStamina = 20f; // 최대 스태미나 +20 같은 느낌

        public override void Apply(GameObject target)
        {
            if (target.TryGetComponent(out Stamina stamina))
            {
                stamina.AddBonusMaxStamina(bonusMaxStamina);
            }
        }

        public override void Remove(GameObject target)
        {
            if (target.TryGetComponent(out Stamina stamina))
            {
                stamina.AddBonusMaxStamina(-bonusMaxStamina);
            }
        }
    }
}