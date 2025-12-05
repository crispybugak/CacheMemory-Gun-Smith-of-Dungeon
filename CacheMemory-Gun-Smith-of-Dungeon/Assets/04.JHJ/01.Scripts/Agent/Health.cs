using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using TMPro;

public class Health : MonoBehaviour, IGetDamage
{
    [Header("Regeneration")]
    [SerializeField] private float lastDamagedTime;

    [field: SerializeField] public HealthDataSO HealthData { get; private set; }
    [field: SerializeField] public float CurrentHealth { get; private set; }

    // 패시브 보너스
    [Header("패시브 보너스")]
    [field: SerializeField] public float bonusMaxHealth { get; private set; }

    // 기본 + 패시브 합친 실제 최대 체력
    public float Maxhealth => HealthData.Maxhealth + bonusMaxHealth;

    // (아래쪽에 있을 이벤트 / 나머지 필드는 그대로)
    public event Action OnDamagedPlayer;
    public event Action OnHealing;

    private void OnEnable()
    {
        lastDamagedTime = 0f;
        CurrentHealth = Maxhealth;
        OnHealing?.Invoke();
    }

    
    private void Start()
    {
        bonusMaxHealth = 0f;
    }

    private void Update()
    {
        lastDamagedTime += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            float randomDamage = Random.Range(1, 30);
            OnDamaged(randomDamage);
        }  
    }
    private IEnumerator HealingHpCT()
    {
        float randomhealing = Random.Range(1, 30);
        while (lastDamagedTime < 10) yield return null;
        while (CurrentHealth < Maxhealth && lastDamagedTime > 10)
        {
            OnHealing?.Invoke();
            CurrentHealth = Mathf.Clamp(CurrentHealth += randomhealing, 0 , Maxhealth);
            yield return new WaitForSeconds(HealthData.HealingInterval);

        }
    }

    public void OnDamaged(float damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, Maxhealth);
        OnDamagedPlayer?.Invoke();
        lastDamagedTime = 0;

        StartCoroutine(HealingHpCT());
    }
    
    public void AddBonusMaxHealth(float amount)
    {
        bonusMaxHealth += amount;
        CurrentHealth = Maxhealth;
        OnHealing?.Invoke();
    }

}
