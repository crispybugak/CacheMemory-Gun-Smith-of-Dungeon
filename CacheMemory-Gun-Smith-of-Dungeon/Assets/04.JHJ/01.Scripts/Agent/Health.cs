using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using TMPro;

public class Health : MonoBehaviour, IGetDamage
{

    [Header("Regeneration")]
    [SerializeField] private float lastDamagedTime;

    [field : SerializeField]public HealthDataSO HealthData { get; private set; }
    [field: SerializeField]public float CurrentHealth { get; private set; }

    public float Maxhealth => HealthData.Maxhealth;
    public float HealingInterval => HealthData.HealingInterval;

    public float sumDamage;
    public float sumHealing;

    public Action OnDamagedPlayer;
    public Action OnHealing;

    private void Start()
    {
        CurrentHealth = Maxhealth;
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
            yield return new WaitForSeconds(HealingInterval);
        }
    }

    public void OnDamaged(float damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, Maxhealth);
        OnDamagedPlayer?.Invoke();
        lastDamagedTime = 0;

        StartCoroutine(HealingHpCT());
    }
}
