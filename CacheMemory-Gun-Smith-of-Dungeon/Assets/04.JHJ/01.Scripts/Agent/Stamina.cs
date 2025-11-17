using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    
    // 스태미나 바 사용시 최대치(BackBar)가 줄어들으며 점진적으로 최대치가 다시 복구 됨.
    [Header("Max/Current")]
    [SerializeField] private float _baseMaxStamina = 100f;   
    private float _currentMaxStamina;                        

    [Header("Rates")]
    [SerializeField] private float _useStaminaGage = 20f;   
    [SerializeField] private float _rechargeSpeed = 10f;     
    [SerializeField] private float _backFollowStaminaBar = 8f; 
    [SerializeField] private float _backBarRechargeSpeed = 4f;   
    [SerializeField] private float _rechargeDelay = 1.5f;

    [Header("UI")]
    [SerializeField] private Image _staminaBar;      
    [SerializeField] private Image _backStaminaBar;   

    [Header("Move")]
    [SerializeField] private float _defaultSpeed = 5f;
    [SerializeField] private float _runSpeed;

    private float _currentStamina; 
    private float _backStamina;                     
    private bool _isRunning;

    private AgentMovement _agentMovement;

    private void Awake()
    {
        _agentMovement = GetComponent<AgentMovement>();
    }

    private void Start()
    {
        _currentStamina = _baseMaxStamina; 
        _backStamina = _baseMaxStamina;
        _currentMaxStamina = _backStamina;
        UpdateUI();
    }
    private void Update()
    {
        // 이동 속도 적용(옵션)
        if (_agentMovement != null)
            _agentMovement.MoveSpeed = _isRunning ? _runSpeed : _defaultSpeed;
        if (_isRunning)
        {
            _currentStamina -= _useStaminaGage * Time.deltaTime;
            if (_backStamina > _currentStamina)
                _backStamina = Mathf.Lerp(_backStamina, _currentStamina, _backFollowStaminaBar * Time.deltaTime * 30); // 백바 감소
        }
        else
        {   
            if(_isRunning == false)
            {
                if (Time.time > 1 +_rechargeDelay)
                {
                    _currentStamina += _rechargeSpeed * Time.deltaTime;
                    if (_backStamina < _baseMaxStamina)
                        _backStamina += _backBarRechargeSpeed * Time.deltaTime;
                }
            }
        }
        _currentMaxStamina = _backStamina; // 최대치
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, _currentMaxStamina);
        UpdateUI();
    }

    private void UpdateUI()
    {
        float mainNorm = _currentStamina / _baseMaxStamina;
        float backNorm = _backStamina / _baseMaxStamina;

        if (_backStaminaBar != null) _backStaminaBar.fillAmount = backNorm; 
        if (_staminaBar != null) _staminaBar.fillAmount = mainNorm;   
    }

    public void SetRunning(bool isRunning)
    {
        _isRunning = isRunning;
    }
}
