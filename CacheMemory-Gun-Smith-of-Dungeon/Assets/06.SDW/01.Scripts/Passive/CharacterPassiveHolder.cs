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
                passive.Apply(gameObject);
            }
        }
    }
}