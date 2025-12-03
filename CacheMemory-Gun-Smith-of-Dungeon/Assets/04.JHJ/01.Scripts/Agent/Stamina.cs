using UnityEngine;

public class Stamina : MonoBehaviour
{
    [field: SerializeField] public AgentStaminaSO AgentStaminaData { get; private set; }
    [Header("SO")]
    public float DefaultSpeed => AgentStaminaData._defaultSpeed;
    public float RunSpeed => AgentStaminaData._runSpeed;
    public float RechargeSpeed => AgentStaminaData._rechargeSpeed;
    public float BackBarRechargeSpeed => AgentStaminaData._backBarRechargeSpeed;
    public float UseStaminaGage => AgentStaminaData._useStaminaGage;
    public float BackFollowStaminaBar => AgentStaminaData._backFollowStaminaBar;

    [Header("Max/Current")]
    [field: SerializeField] public float _baseMaxStamina { get; private set; }
    [field: SerializeField] public float lastUseStaminaTime { get; private set; }
    [field: SerializeField] public float _currentMaxStamina { get; private set; }
    [field: SerializeField] public float _currentStamina { get; private set; }
    [field: SerializeField] public float _backStamina { get; private set; }

    bool _isMove;
    bool _runRequested;                    // _ 달리기 키 입력 상태
    public bool _isRunning { get; private set; }  // 실제로 달리는 상태

    private AgentMovement _agentMovement;
    private Agent _agent;
    [SerializeField] private StaminaUI _staminaUI;

    private void Awake()
    {
        _agentMovement = GetComponent<AgentMovement>();
        _agent = GetComponent<Agent>();
    }

    private void Start()
    {
        _currentStamina = _baseMaxStamina;
        _backStamina = _baseMaxStamina;
        _currentMaxStamina = _backStamina;
        _agentMovement.MoveSpeed = DefaultSpeed;
        _staminaUI.UpdateUI();
        _isMove = _agent.RidCompo.linearVelocity.sqrMagnitude > 0.1f;

    }

    private void Update()
    {
        _isMove = _agent.RidCompo.linearVelocity.sqrMagnitude > 0.1f;      

        bool canRun =
            _runRequested &&                                              
            _isMove &&                                                    
            _currentStamina > 0f;                                        

        _isRunning = canRun;                                             

        if (canRun)                                                      
        {
            _agentMovement.MoveSpeed = RunSpeed;                          
            UseStamina();                                                 
        }
        else                                                              
        {
            _agentMovement.MoveSpeed = DefaultSpeed;                     
            RechargeStamina();                                            
        }
    }
    private void UseStamina()
    {
        lastUseStaminaTime = 0f;                                         
        _currentStamina -= UseStaminaGage * Time.deltaTime;               

        if (_backStamina > _currentStamina)
        {
            _backStamina = Mathf.Lerp(
                _backStamina,
                _currentStamina,
                BackFollowStaminaBar * Time.deltaTime * 30f
            );
        }

        _currentMaxStamina = _backStamina;
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, _currentMaxStamina);
        _staminaUI.UpdateUI();
    }

    private void RechargeStamina()
    {
        if (!_isRunning)
        {
            lastUseStaminaTime = Mathf.Clamp(lastUseStaminaTime, 0, 10);
            lastUseStaminaTime += Time.deltaTime;

            if (1.5f < lastUseStaminaTime)
            {
                _currentStamina += RechargeSpeed * Time.deltaTime;

                if (_backStamina < _baseMaxStamina)
                    _backStamina += BackBarRechargeSpeed * Time.deltaTime;
            }
        }

        _currentMaxStamina = _backStamina;
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, _currentMaxStamina);
        _staminaUI.UpdateUI();
    }

    public void SetRunning(bool isRunning)
    {
        _runRequested = isRunning;                                       
    }
}
