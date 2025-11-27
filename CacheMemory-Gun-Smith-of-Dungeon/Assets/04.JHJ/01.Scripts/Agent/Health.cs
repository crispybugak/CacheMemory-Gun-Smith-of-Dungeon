using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour, IGetDamage
{

    [Header("Regeneration")]
    [SerializeField] private float lastDamagedTime;

    [field : SerializeField]public HealthDataSO HealthData { get; private set; }
    [field: SerializeField]public float CurrentHealth { get; private set; }

    public float Maxhealth => HealthData.Maxhealth;
    public float HealingInterval => HealthData.HealingInterval;


    public Action OnDamagedPlayer;
    private bool isCanHealing;
    private void Start()
    {
        CurrentHealth = Maxhealth;
    }

    private void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            lastDamagedTime = 0;
            float damage = CurrentHealth * 0.1f;
            Debug.Log(damage);
            OnDamaged(damage);
        }  
    }
    private IEnumerator HealingHpCT()
    {
        lastDamagedTime += Time.deltaTime;
        while (lastDamagedTime > 10f)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth,0 , Maxhealth);
            yield return new WaitForSeconds(HealingInterval);
            CurrentHealth += Maxhealth * 0.1f;
        }
    }

    public void OnDamaged(float damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, Maxhealth);
        OnDamagedPlayer?.Invoke();
        StartCoroutine(HealingHpCT());
    }
}
