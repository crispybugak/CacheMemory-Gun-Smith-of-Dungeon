using _06.SDW._01.Scripts.SO;
using UnityEngine;

namespace _06.SDW._01.Scripts.Passive
{
    public class CharacterPassiveHolder : MonoBehaviour
    {
        [SerializeField] private PassiveSO[] passives;

        private void Start()
        {
            // 시작할 때 가지고 있는 패시브 전부 적용
            foreach (var passive in passives)
            {
                if (passive == null) continue;
                passive.Apply(gameObject);
            }
        }

        // === 저장된 패시브 하나를 런타임에 세팅 ===
        public void SetSinglePassive(PassiveSO passive)
        {
            if (passive == null)
            {
                passives = System.Array.Empty<PassiveSO>();
            }
            else
            {
                passives = new[] { passive };
            }
        }
    }
}