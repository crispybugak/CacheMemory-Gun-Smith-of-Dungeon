using System.Collections;
using UnityEngine;

namespace _06.SDW._01.Scripts.SO
{
    [CreateAssetMenu(fileName = "CircleDotFieldSkill", menuName = "SO/Skill/CircleDotField")]
    public class CircleDotFieldSkillSO : SkillSO, ISkill
    {
        [Header("장판 설정")]
        [SerializeField] private float radius = 2f;          // 장판 반경
        [SerializeField] private float duration = 3f;        // 장판 유지 시간
        [SerializeField] private float tickInterval = 0.5f;  // 데미지 주기
        [SerializeField] private LayerMask targetLayer;      // 맞을 대상 레이어

        [Header("이펙트")]
        [SerializeField] private GameObject fieldPrefab;     // 장판 이펙트 프리팹

        public IEnumerator UseSkill(GameObject owner)
        {
            if (owner == null) yield break;

            // 중심 위치 계산 (RigidBody2D 있으면 그 위치, 없으면 Transform)
            Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();
            Vector2 center = rb != null ? rb.position : owner.transform.position;

            // 장판 이펙트 생성
            GameObject fieldInstance = null;
            if (fieldPrefab != null)
            {
                fieldInstance = Instantiate(fieldPrefab, center, Quaternion.identity);

                // 장판 크기 조절 (프리팹이 1단위 짜리 원이라고 가정)
                fieldInstance.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
            }

            float elapsed = 0f;

            // duration 동안 tickInterval마다 데미지
            while (elapsed < duration)
            {
                DoCircleDamage(center, radius);
                yield return new WaitForSeconds(tickInterval);
                elapsed += tickInterval;
            }

            // 장판 이펙트 삭제
            if (fieldInstance != null)
            {
                Destroy(fieldInstance);
            }
        }

        private void DoCircleDamage(Vector2 center, float radius)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, targetLayer);

            foreach (var hit in hits)
            {
                IGetDamage damageTarget = hit.GetComponent<IGetDamage>();
                if (damageTarget != null)
                {
                    damageTarget.OnDamaged(Damage); // SkillSO에 있던 Damage 사용
                }
            }
        }
    }
}
