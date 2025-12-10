using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public HealthDataSO HealthDataSO { get; private set; }

    [Header("UI")]
    [SerializeField] private Image _healthBar;

    public HitEffect _hitEffect;

    private void OnEnable()
    {
        Health.OnDamagedPlayer += MinusUpdateUI;
        Health.OnHealing += PlusUpdateUI;
    }
    private void OnDisable()
    {
        Health.OnDamagedPlayer -= MinusUpdateUI;
        Health.OnHealing -= PlusUpdateUI;
    }
    private void MinusUpdateUI()
    {
        if(_hitEffect !=null)
        _hitEffect.Play();
        _healthBar.fillAmount = Health.CurrentHealth / Health.Maxhealth;
    }
    private void PlusUpdateUI()
    {
        _healthBar.fillAmount = Health.CurrentHealth / Health.Maxhealth;
    }
}
