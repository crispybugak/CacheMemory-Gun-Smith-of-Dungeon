using System.Linq;
using _06.SDW._01.Scripts.Passive;
using _06.SDW._01.Scripts.SO;
using UnityEngine;

namespace _06.SDW._01.Scripts.JSON
{
    public class PlayerSelectionApplier : MonoBehaviour
    {
        [Header("필수 컴포넌트")]
        [SerializeField] private Health health;
        [SerializeField] private Stamina stamina;
        [SerializeField] private CharacterPassiveHolder passiveHolder;
        [SerializeField] private CharacterSkillSet skillSet;
        [SerializeField] private Animator animator;

        [Header("데이터베이스 (이 씬에서 사용할 SO/Animator 풀)")]
        [SerializeField] private PassiveSO[] passiveDatabase;
        [SerializeField] private SkillSO[] skillDatabase;
        [SerializeField] private RuntimeAnimatorController[] animatorDatabase;

        private void Reset()
        {
            if (health == null) health = GetComponent<Health>();
            if (stamina == null) stamina = GetComponent<Stamina>();
            if (passiveHolder == null) passiveHolder = GetComponent<CharacterPassiveHolder>();
            if (skillSet == null) skillSet = GetComponent<CharacterSkillSet>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        private void Awake()
        {
            ApplyFromSave();
        }

        public void ApplyFromSave()
        {
            if (!PlayerSelectionSaveSystem.TryLoad(out var data))
            {
                Debug.LogWarning("[PlayerSelectionApplier] 저장 데이터가 없어서 적용을 건너뜁니다.");
                return;
            }

            // --- 1. Health / Stamina 수치 적용 ---
            if (health != null && health.HealthData != null)
            {
                health.HealthData.ApplySaveData(data.health);

                // Health 내부에서 CurrentHealth를 건드릴 수 없으니,
                // 기존 메서드를 이용해서 Maxhealth 기준으로 다시 풀피로 맞춰줌
                health.AddBonusMaxHealth(0f);
            }

            if (stamina != null && stamina.AgentStaminaData != null)
            {
                stamina.AgentStaminaData.ApplySaveData(data.stamina);

                // 동일하게 기존 메서드로 재계산 + UI 업데이트
                stamina.AddBonusMaxStamina(0f);
            }

            // --- 2. 패시브 적용 ---
            if (passiveHolder != null && passiveDatabase != null && passiveDatabase.Length > 0)
            {
                PassiveSO passive = FindByName(passiveDatabase, data.passiveName);
                if (passive != null)
                {
                    passiveHolder.SetSinglePassive(passive);
                }
            }

            // --- 3. 스킬 적용 ---
            if (skillSet != null && skillDatabase != null && skillDatabase.Length > 0)
            {
                SkillSO skill = FindByName(skillDatabase, data.skillName);
                if (skill != null)
                {
                    skillSet.SetSkill(skill);
                }
            }

            // --- 4. 애니메이터 적용 ---
            if (animator != null && animatorDatabase != null && animatorDatabase.Length > 0)
            {
                RuntimeAnimatorController controller = FindByName(animatorDatabase, data.animatorName);
                if (controller != null)
                {
                    animator.runtimeAnimatorController = controller;
                }
            }
        }

        private static T FindByName<T>(T[] array, string name) where T : Object
        {
            if (string.IsNullOrEmpty(name)) return null;
            return array.FirstOrDefault(x => x != null && x.name == name);
        }
    }
}
