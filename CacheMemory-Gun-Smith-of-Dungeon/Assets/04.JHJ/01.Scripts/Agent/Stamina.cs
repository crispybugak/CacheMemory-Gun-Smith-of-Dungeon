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
        _isMove = _agent.RidCompo.linearVelocity.sqrMagnitude > 0.1f;      // _ 매 프레임 이동 여부 갱신

        bool canRun =
            _runRequested &&                                              // _ 달리기 키 눌렸고
            _isMove &&                                                    // _ 실제로 움직이고 있고
            _currentStamina > 0f;                                         // _ 스태미나 남았을 때만 런

        _isRunning = canRun;                                              // _ 외부에서 확인용 실제 러닝 상태

        if (canRun)                                                       // _ 런 상태
        {
            _agentMovement.MoveSpeed = RunSpeed;                          // _
            UseStamina();                                                 // _
        }
        else                                                              // _ 걷기/정지 + 회복 상태
        {
            _agentMovement.MoveSpeed = DefaultSpeed;                      // _
            RechargeStamina();                                            // _
        }
    }

    private void UseStamina()
    {
        lastUseStaminaTime = 0f;                                          // _
        _currentStamina -= UseStaminaGage * Time.deltaTime;               // _

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
        if (!_isRunning)                                                  // _ 실제로 달리는 중이 아닐 때만 회복
        {
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
        _runRequested = isRunning;                                        // _ 키 입력만 저장
    }
}
