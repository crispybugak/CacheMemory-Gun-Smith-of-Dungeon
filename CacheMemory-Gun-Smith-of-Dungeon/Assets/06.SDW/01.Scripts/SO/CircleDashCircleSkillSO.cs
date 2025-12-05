using System.Collections;
using UnityEngine;

namespace _06.SDW._01.Scripts.SO
{
    [CreateAssetMenu(fileName = "CircleDashCircleSkill", menuName = "SO/Skill/CircleDashCircle")]
    public class CircleDashCircleSkillSO : SkillSO, ISkill
    {
        [Header("범위 공격 설정")]
        [SerializeField] private float firstRadius = 1.5f;
        [SerializeField] private float secondRadius = 1.5f;
        [SerializeField] private LayerMask targetLayer;

        [Header("돌진 설정")]
        [SerializeField] private float dashDistance = 3f;
        [SerializeField] private float dashDuration = 0.2f;

        public IEnumerator UseSkill(GameObject owner)
        {
            if (owner == null) yield break;

            Agent agent = owner.GetComponent<Agent>();
            Rigidbody2D rb = agent != null ? agent.RidCompo : owner.GetComponent<Rigidbody2D>();
            if (rb == null) yield break;

            // 여기서만 방향 계산
            Vector2 dir = GetDashDirection(owner);
            if (dir.sqrMagnitude < 0.001f)
                dir = Vector2.right;
            dir = dir.normalized;

            // 1. 첫 번째 원형 공격
            DoCircleDamage(rb.position, firstRadius);

            // 2. 돌진
            yield return Dash(rb, dir);

            // 3. 두 번째 원형 공격
            DoCircleDamage(rb.position, secondRadius);
        }

        private Vector2 GetDashDirection(GameObject owner)
        {
            AgentMovement movement = owner.GetComponent<AgentMovement>();
            if (movement != null)
            {
                Vector2 moveDir = movement.IsMoved();
                if (moveDir.sqrMagnitude > 0.001f)
                    return moveDir;
            }
            
            return Vector2.right;
        }

        private void DoCircleDamage(Vector2 center, float radius)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, targetLayer);

            foreach (var hit in hits)
            {
                IGetDamage damageTarget = hit.GetComponent<IGetDamage>();
                if (damageTarget != null)
                {
                    damageTarget.OnDamaged(Damage);
                }
            }
        }

        private IEnumerator Dash(Rigidbody2D rb, Vector2 dir)
        {
            Vector2 start = rb.position;
            Vector2 end = start + dir * dashDistance;

            float elapsed = 0f;
            while (elapsed < dashDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / dashDuration);

                Vector2 pos = Vector2.Lerp(start, end, t);
                rb.MovePosition(pos);

                yield return null;
            }

            rb.MovePosition(end);
        }
    }
}
