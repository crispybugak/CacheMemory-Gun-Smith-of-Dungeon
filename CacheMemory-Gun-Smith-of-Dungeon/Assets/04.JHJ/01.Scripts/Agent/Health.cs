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
            Camera.main.GetComponent<CameraShake>().ShakeCamera(0.2f,0.1f);
            GameManager.Instance.HitTimeScaleCT();
            float randomDamage = Random.Range(1, 30);
            OnDamaged(randomDamage);
        }  
    }
    private IEnumerator HealingHpCT()
    {
        while (lastDamagedTime < 10) yield return null;
        while (CurrentHealth < Maxhealth && lastDamagedTime > 10)
        {
            OnHealing?.Invoke();
            float randomhealing = 10;
            Debug.Log("한번한번한번한번한번한번한번한번한번한번한번한번한번");
            randomhealing = Random.Range(1, 15);
            CurrentHealth = Mathf.Clamp(CurrentHealth += randomhealing, 0 , Maxhealth);
            if (CurrentHealth >= Maxhealth)
                OnHealing?.Invoke();
            yield return new WaitForSeconds(HealthData.HealingInterval);
            //여기서 HP 100이 되어버리면 위쪽 While문에서 막히면서 OnHealing?.Invoke();가 안 먹혀서 마지막 UI가 갱신이 안 됨
            //가장 아래로 내리면 해결은 되지만, 힐이 3번 정도 동시에 되면서 HP가 비정상적으로 많이 회복 됨
        }
    }

    public void OnDamaged(float damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, Maxhealth);
        OnDamagedPlayer?.Invoke();
        lastDamagedTime = 0;

        StopAllCoroutines();
        StartCoroutine(HealingHpCT());

    }
    
    public void AddBonusMaxHealth(float amount)
    {
        bonusMaxHealth += amount;
        CurrentHealth = Maxhealth;
        OnHealing?.Invoke();

    }
}
