using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Health health;

    [Header("UI")]
    [SerializeField] private Image _healthBar;

    [Header("Health Setting")]
    [SerializeField] private float _maxhealth;
    public float _CurrentHealth { get; private set; }

    public HitEffect _hitEffect;


    private void OnEnable()
    {
        health.OnDamagedPlayer += UpdateUI;
    }
    private void OnDisable()
    {
        health.OnDamagedPlayer -= UpdateUI;
    }
    [field: SerializeField] public HealthDataSO HealthDataSO { get; private set; }
    private void UpdateUI()
    {
        _hitEffect.Play();
        _healthBar.fillAmount = _CurrentHealth / _maxhealth;
    }

}
