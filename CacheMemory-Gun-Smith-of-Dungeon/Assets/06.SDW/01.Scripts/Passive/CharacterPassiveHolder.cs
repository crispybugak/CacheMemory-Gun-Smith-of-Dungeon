using _06.SDW._01.Scripts.SO;
using UnityEngine;

namespace _06.SDW._01.Scripts.Passive
{
    public class CharacterPassiveHolder : MonoBehaviour
    {
        [SerializeField] private PassiveSO[] passives;

        private bool _initialized;

        private void Start()
        {
            ApplyAll();
            _initialized = true;
        }

        private void ApplyAll()
        {
            if (passives == null) return;

            foreach (var passive in passives)
            {
                passive?.Apply(gameObject);
            }
        }

        public void SetPassives(PassiveSO[] newPassives, bool applyNow = true)
        {
            // 이미 한 번 적용된 상태면 Remove로 원상복구
            if (_initialized && passives != null)
            {
                foreach (var passive in passives)
                {
                    passive?.Remove(gameObject);
                }
            }

            passives = newPassives;

            if (applyNow)
            {
                ApplyAll();
                _initialized = true;
            }
        }

        public void SetSinglePassive(PassiveSO passive, bool applyNow = true)
        {
            SetPassives(passive != null ? new[] { passive } : null, applyNow);
        }
    }
}