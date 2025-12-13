using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private BaseEnemy enemy;   // 안 넣으면 부모에서 자동 찾음
    [SerializeField] private Image fillImage;   // fillAmount 바꿀 이미지
    [SerializeField] private bool hideWhenFull = true;

    private void Awake()
    {
        if (enemy == null)
            enemy = GetComponentInParent<BaseEnemy>();

        if (enemy != null)
        {
            enemy.OnHealthChanged += HandleHealthChanged;
            enemy.OnDeath += HandleEnemyDeath;

            // 초기값 한 번 세팅
            HandleHealthChanged(enemy.CurrentHealth, enemy.MaxHealth);
        }
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnHealthChanged -= HandleHealthChanged;
            enemy.OnDeath -= HandleEnemyDeath;
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (fillImage == null || max <= 0) return;

        float t = Mathf.Clamp01((float)current / max);
        fillImage.fillAmount = t;

        if (hideWhenFull && fillImage.transform.parent != null)
            fillImage.transform.parent.gameObject.SetActive(t < 1f);
    }

    private void HandleEnemyDeath(BaseEnemy _)
    {
        // 죽으면 헬스바 끄고 싶으면
        if (fillImage != null && fillImage.transform.parent != null)
            fillImage.transform.parent.gameObject.SetActive(false);
    }
}