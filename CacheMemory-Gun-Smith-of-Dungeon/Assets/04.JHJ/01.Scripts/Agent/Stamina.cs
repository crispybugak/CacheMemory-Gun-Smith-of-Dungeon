using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    
    // 스태미나 바 사용시 최대치(BackBar)가 줄어들으며 점진적으로 최대치가 다시 복구 됨.
    [Header("Max/Current")]
    [SerializeField] public float _baseMaxStamina { get; private set; } = 100f;
    [SerializeField] public float _currentMaxStamina { get; private set; }                        

    [Header("Rates")]
    [SerializeField] private float _useStaminaGage = 20f;   
    [SerializeField] private float _rechargeSpeed = 10f;     
    [SerializeField] private float _backFollowStaminaBar = 8f; 
    [SerializeField] private float _backBarRechargeSpeed = 4f;   
    [SerializeField] private float lastUseStaminaTime;

    [Header("Move")]
    [SerializeField] private float _defaultSpeed = 5f;
    [SerializeField] private float _runSpeed;

    [Header("Stamina")]
    public float _currentStamina { get; private set; }
    public float _backStamina { get; private set; }                    
    public bool _isRunning { get; private set; }

    private AgentMovement _agentMovement;
    private Agent _agent;
    StaminaUI _staminaUI;

    private void Awake()
    {
        _agentMovement = GetComponent<AgentMovement>();
        _agent = GetComponent<Agent>();
        _staminaUI = GetComponent<StaminaUI>();
    }
     
    private void Start()
    {
        _currentStamina = _baseMaxStamina; 
        _backStamina = _baseMaxStamina;
        _currentMaxStamina = _backStamina;
        _agentMovement.MoveSpeed = _defaultSpeed;
        _staminaUI.UpdateUI();
    }
    private void Update()
    {
        bool value = _agent.RidCompo.linearVelocity.sqrMagnitude > 0.1;

        if (_isRunning && value)
            UseStamina();
        else if(!_isRunning)
            RechargeStamina();
    }


    private void UseStamina()
    {
        if (_agentMovement != null)
            _agentMovement.MoveSpeed = _isRunning ? _agentMovement.MoveSpeed = _runSpeed : _defaultSpeed;
        if (_isRunning)
        {
            lastUseStaminaTime = 0;
            _currentStamina -= _useStaminaGage * Time.deltaTime;
            if (_backStamina > _currentStamina)
                _backStamina = Mathf.Lerp(_backStamina, _currentStamina, _backFollowStaminaBar * Time.deltaTime * 30); // 백바 감소
        }
        _currentMaxStamina = _backStamina; // 최대치
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, _currentMaxStamina);
        _staminaUI.UpdateUI();
    }

    private void RechargeStamina()
    {
        if (!_isRunning)
        {
            lastUseStaminaTime += Time.deltaTime;
            if (1.5 < lastUseStaminaTime)
            {
                _currentStamina += _rechargeSpeed * Time.deltaTime;
                if (_backStamina < _baseMaxStamina)
                    _backStamina += _backBarRechargeSpeed * Time.deltaTime;
            }
        }
        _currentMaxStamina = _backStamina; // 최대치
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, _currentMaxStamina);
        _staminaUI.UpdateUI();
    }
    public void SetRunning(bool isRunning)
    {
        _isRunning = isRunning;
    }
}
