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

    [Header("패시브 보너스")]
    [field: SerializeField] public float bonusMaxStamina { get; private set; }

    bool _isMove;
    bool _runRequested;                    // 달리기 키 입력 상태
    public bool _isRunning { get; private set; }  // 실제로 달리는 상태

    private AgentMovement _agentMovement;
    private Agent _agent;
    [SerializeField] private StaminaUI _staminaUI;

    public float MaxStaminaWithPassive => _baseMaxStamina + bonusMaxStamina;

    private void Awake()
    {
        _agentMovement = GetComponent<AgentMovement>();
        _agent = GetComponent<Agent>();
    }

    private void OnEnable()
    {
        // ★ SO / 패시브 기준으로 스태미나 수치 초기화
        InitFromSO();
    }

    private void Start()
    {
        _agentMovement.MoveSpeed = DefaultSpeed;
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

    // ★ 세이브 로드 후에도 호출할 수 있는 초기화 함수
    //    - AgentStaminaSO 값이 바뀐 뒤에 다시 최대/현재 스태미나를 맞춰줄 때 사용
    public void InitFromSO()
    {
        lastUseStaminaTime = 0f;

        // 처음 켜질 때는 항상 꽉 찬 상태
        _currentMaxStamina = MaxStaminaWithPassive;
        _currentStamina = _currentMaxStamina;
        _backStamina = _currentMaxStamina;

        if (_staminaUI != null)
            _staminaUI.UpdateUI();
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

                // 이제는 baseMax가 아니라 패시브 포함 최대치까지 회복
                if (_backStamina < MaxStaminaWithPassive)
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

    public void AddBonusMaxStamina(float amount)
    {
        bonusMaxStamina += amount;

        // 패시브 적용 시점에 새 최대치 기준으로 풀 충전
        _currentMaxStamina = MaxStaminaWithPassive;
        _currentStamina = _currentMaxStamina;
        _backStamina = _currentMaxStamina;

        if (_staminaUI != null)
            _staminaUI.UpdateUI();
    }
}