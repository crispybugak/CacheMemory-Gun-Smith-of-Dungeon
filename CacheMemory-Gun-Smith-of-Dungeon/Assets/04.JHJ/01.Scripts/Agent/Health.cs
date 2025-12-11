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

    public event Action OnDamagedPlayer;
    public event Action OnHealing;

    private void OnEnable()
    {
        // ★ SO 값으로부터 현재 체력 다시 맞춰주는 초기화
        InitFromSO();
    }

    private void Start()
    {
        bonusMaxHealth = 0f;
    }

    private void Update()
    {
        lastDamagedTime += Time.deltaTime;
    }

    // ★ 세이브 로드 후에도 호출할 수 있는 초기화 함수
    //    - HealthDataSO 값(Maxhealth 등)이 바뀐 뒤에 다시 맞춰줄 때 사용
    public void InitFromSO()
    {
        lastDamagedTime = 0f;
        CurrentHealth = Maxhealth;
        OnHealing?.Invoke();   // UI 갱신
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Camera.main.GetComponent<CameraShake>().ShakeCamera(0.2f, 0.1f);
            GameManager.Instance.HitTimeScaleCT();
            float randomDamage = Random.Range(1, 30);
            OnDamaged(randomDamage);
        }
    }

    private IEnumerator HealingHpCT()
    {
        // 데미지 안 맞은 시간 10초 될 때까지 대기
        while (lastDamagedTime < 10) yield return null;

        // 체력이 최대치보다 낮고, 여전히 10초 이상 데미지 안 맞았을 때 회복
        while (CurrentHealth < Maxhealth && lastDamagedTime > 10)
        {
            OnHealing?.Invoke();
            float randomhealing = 10;
            Debug.Log("한번한번한번한번한번한번한번한번한번한번한번한번한번");
            randomhealing = Random.Range(1, 15);
            CurrentHealth = Mathf.Clamp(CurrentHealth += randomhealing, 0, Maxhealth);
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