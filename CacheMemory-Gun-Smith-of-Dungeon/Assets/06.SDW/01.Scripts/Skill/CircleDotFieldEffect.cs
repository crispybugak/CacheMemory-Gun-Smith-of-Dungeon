using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _06.SDW._01.Scripts.Skill
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CircleDotFieldEffect : MonoBehaviour
    {
        [Header("장판 설정")]
        [SerializeField] private float duration = 3f;
        [SerializeField] private float tickInterval = 0.5f;
        [SerializeField] private LayerMask targetLayer;
        
        [Header("비주얼")]
        [SerializeField] private float fadeOutTime = 0.5f;
        [SerializeField] private Ease fadeEase = Ease.OutQuad;
        [SerializeField] private SpriteRenderer[] renderers;

        private CircleCollider2D _collider;
        private readonly List<Collider2D> _overlapResults = new (64);

        private float _damage;

        public void Setup(float damage)
        {
            _damage = damage;
        }

        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();

            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<SpriteRenderer>();
            }
        }

        private void OnEnable()
        {
            StartCoroutine(RunRoutine());
        }

        private IEnumerator RunRoutine()
        {
            float elapsed = 0f;

            // duration 동안 tickInterval마다 딜
            while (elapsed < duration)
            {
                DoDamageTick();

                yield return new WaitForSeconds(tickInterval);
                elapsed += tickInterval;
            }

            StartFadeAndDestroy();
        }

        private void DoDamageTick()
        {
            if (_collider == null) return;

            ContactFilter2D filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = targetLayer,
                useTriggers = true
            };

            _overlapResults.Clear();
            int count = _collider.Overlap(filter, _overlapResults);

            for (int i = 0; i < count; i++)
            {
                Debug.Log($"Damage {i}"); 
                Collider2D col = _overlapResults[i];
                if (col == null) continue;

                IGetDamage damageTarget = col.GetComponent<IGetDamage>();
                if (damageTarget != null)
                {
                    damageTarget.OnDamaged(_damage);
                }
            }
        }

        private void StartFadeAndDestroy()
        {
            if (renderers == null || renderers.Length == 0)
            {
                Destroy(gameObject);
                return;
            }
            
            Sequence seq = DOTween.Sequence();

            foreach (var sr in renderers)
            {
                if (sr == null) continue;
                seq.Join(sr.DOFade(0f, fadeOutTime).SetEase(fadeEase));
            }

            seq.OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        private void OnDestroy()
        {
            DOTween.Kill(gameObject);
        }
    }
}
