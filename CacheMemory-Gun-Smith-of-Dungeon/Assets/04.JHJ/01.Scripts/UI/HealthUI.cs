using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public HealthDataSO HealthDataSO { get; private set; }

    [Header("UI")]
    [SerializeField] private Image _healthBar;

    public float _CurrentHealth { get; private set; }

    public HitEffect _hitEffect;

    private void OnEnable()
    {
        Health.OnDamagedPlayer += UpdateUI;
    }
    private void OnDisable()
    {
        Health.OnDamagedPlayer -= UpdateUI;
    }
    private void UpdateUI()
    {
        _hitEffect.Play();
        _healthBar.fillAmount = Health.CurrentHealth / Health.Maxhealth;
    }

}
