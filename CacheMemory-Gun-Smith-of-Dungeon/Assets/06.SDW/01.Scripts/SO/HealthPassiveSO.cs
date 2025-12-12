using UnityEngine;

namespace _06.SDW._01.Scripts.SO
{
    [CreateAssetMenu(fileName = "HealthPassive", menuName = "SO/Passive/HealthUp")]
    public class HealthPassiveSO : PassiveSO
    {
        [Header("체력 증가량 설정")]
        [SerializeField] private float bonusMaxHealth = 20f;

        public override void Apply(GameObject target)
        {
            if (target.TryGetComponent(out Health health))
            {
                health.AddBonusMaxHealth(bonusMaxHealth);
            }
        }

        public override void Remove(GameObject target)
        {
            if (target.TryGetComponent(out Health health))
            {
                health.AddBonusMaxHealth(-bonusMaxHealth);
            }
        }

    }
}