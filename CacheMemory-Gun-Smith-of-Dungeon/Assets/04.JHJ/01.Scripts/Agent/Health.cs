using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private Agent _agent;

    [Header("UI")]
    [SerializeField] private Image _healthBar; 

    [Header("Health Setting")]
    [SerializeField] private float _maxhealth = 100;
    [SerializeField] private float _baseHealth;
    public float _currentHealth { get; private set; }

    public HitEffect _hitEffect;

    private event Action onDamagedPlayer;
    private void Awake()
    {
        _agent = GetComponent<Agent>();
    }
    private void Start()
    {    
        _currentHealth = _maxhealth;
    }
    private void OnEnable()
    {
        onDamagedPlayer += UpdateUI;
    }

    private void OnDisable()
    {
        onDamagedPlayer -= UpdateUI;
    }
    public void OnDamaged(float damage)
    {
        onDamagedPlayer?.Invoke();
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, _maxhealth);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            float damage = _maxhealth * 0.1f;
            Debug.Log(damage);
            OnDamaged(damage);
        }
    }
    private void UpdateUI()
    {
        _hitEffect.Play();
        _healthBar.fillAmount = _currentHealth / _maxhealth;
    }
}
