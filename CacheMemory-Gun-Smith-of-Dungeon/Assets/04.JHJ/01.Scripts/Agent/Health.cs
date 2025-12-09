using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Health : MonoBehaviour, IGetDamage
{
    private Agent _agent;

    [Header("UI")]
    [SerializeField] private Image _healthBar;

    [Header("Health Setting")]
    [SerializeField] private float _maxhealth;
    [SerializeField] private float _baseHealth;
    public float _CurrentHealth { get; private set; }

    public HitEffect _hitEffect;

    [Header("Regeneration")]
    [SerializeField] private float lastDamagedTime {get; set;}
    [SerializeField] private float healingInterval { get; set; }

    [field : SerializeField]public HealthDataSO HealthDataSO { get; private set; }

    public  Action onDamagedPlayer;
    private void Awake()
    {
        _agent = GetComponent<Agent>();
    }
    private void Start()
    {

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        _maxhealth = HealthDataSO.Maxhealth;
        _CurrentHealth = _maxhealth;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            float damage = _maxhealth * 0.1f;
            Debug.Log(damage);
            OnDamaged(damage);
            lastDamagedTime = Time.deltaTime;
        }
    }
    private IEnumerator HealingHpCT()
    {
        if(10 > lastDamagedTime)
        {
            yield return new WaitForSeconds(0.5f);
        _CurrentHealth = Mathf.Clamp(0, _CurrentHealth, _maxhealth);
        }

    }

    public void OnDamaged(float damage)
    {
        _CurrentHealth = Mathf.Clamp(_CurrentHealth - damage, 0f, _maxhealth);
        onDamagedPlayer?.Invoke();
    }
}
