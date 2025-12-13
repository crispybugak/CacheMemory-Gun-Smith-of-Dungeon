using System.Collections;
using UnityEngine;
using _06.SDW._01.Scripts.Skill;

namespace _06.SDW._01.Scripts.SO
{
    [CreateAssetMenu(fileName = "CircleDashCircleSkill", menuName = "SO/Skill/CircleDashCircle")]
    public class CircleDashCircleSkillSO : SkillSO, ISkill
    {
        [Header("이펙트 프리팹 (필수)")]
        [SerializeField] private GameObject attackEffectPrefab; // 애니메이션이 들어있는 프리팹

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
            
            if (rb == null || attackEffectPrefab == null)
            {
                Debug.LogWarning("Rigidbody 또는 이펙트 프리팹이 없습니다.");
                yield break;
            }

            // 방향 계산
            Vector2 dir = GetDashDirection(owner);
            if (dir.sqrMagnitude < 0.001f) dir = Vector2.right;
            dir = dir.normalized;

            // 1. 이펙트 생성 (플레이어 위치)
            yield return ProcessAttack(rb.position, firstRadius);

            yield return Dash(rb, dir);

            // 돌진 끝난 위치에서 생성
            yield return ProcessAttack(rb.position, secondRadius);
        }

        // 공격 이펙트 생성 ~ 데미지 처리까지의 로직을 함수로 분리
        private IEnumerator ProcessAttack(Vector2 spawnPosition, float radius)
        {
            // 1. 이펙트(애니메이션) 생성
            GameObject effectInstance = Instantiate(attackEffectPrefab, spawnPosition, Quaternion.identity);
            SkillEffectReceiver receiver = effectInstance.GetComponent<SkillEffectReceiver>();

            if (receiver != null)
            {
                bool isHit = false;

                // 2. 이펙트가 보내는 신호를 구독 (람다식 사용)
                receiver.OnHitSignal += () => isHit = true;

                // 3. 신호가 올 때까지 대기
                // (만약 이펙트가 실수로 파괴되면 무한루프 돌 수 있으므로 null 체크 추가)
                yield return new WaitUntil(() => isHit || effectInstance == null);

                // 4. 데미지 적용
                if (isHit) // 신호를 받고 넘어온 경우에만 데미지
                {
                    DoCircleDamage(spawnPosition, radius);
                }
            }
            else
            {
                // 리시버가 안 붙어있으면 그냥 즉시 데미지 주고 넘어감 (안전장치)
                DoCircleDamage(spawnPosition, radius);

                yield return new WaitForSeconds(0.5f); 
            }
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
                rb.MovePosition(Vector2.Lerp(start, end, t));
                yield return null;
            }
            rb.MovePosition(end);
        }
    }
}