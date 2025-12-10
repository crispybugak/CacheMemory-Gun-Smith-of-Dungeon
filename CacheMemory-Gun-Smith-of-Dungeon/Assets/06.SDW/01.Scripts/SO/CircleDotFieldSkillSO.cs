using System.Collections;
using _06.SDW._01.Scripts.Skill;
using UnityEngine;

namespace _06.SDW._01.Scripts.SO
{
    [CreateAssetMenu(fileName = "CircleDotFieldSkill", menuName = "SO/Skill/CircleDotField")]
    public class CircleDotFieldSkillSO : SkillSO, ISkill
    {
        [SerializeField] private GameObject fieldPrefab;

        public IEnumerator UseSkill(GameObject owner)
        {
            if (owner == null || fieldPrefab == null)
                yield break;
            
            if (FindObjectOfType<CircleDotFieldEffect>() != null)
            {
                yield break;
            }

            // 플레이어 위치 기준으로 생성
            Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();
            Vector2 center = rb != null ? rb.position : owner.transform.position;

            GameObject fieldInstance = Instantiate(fieldPrefab, center, Quaternion.identity);

            // 생성된 이펙트에게 데미지 값 넘겨주기
            CircleDotFieldEffect effect = fieldInstance.GetComponent<CircleDotFieldEffect>();
            if (effect != null)
            {
                effect.Setup(Damage); // SkillSO에 있는 Damage 사용
            }

            // 이 스킬 코루틴은 "생성"까지만 하고 바로 끝내도 됨
            yield break;
        }
    }
}