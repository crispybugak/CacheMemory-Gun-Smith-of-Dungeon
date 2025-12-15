using _06.SDW._01.Scripts.Save;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [field: SerializeField] public AgentStaminaSO AgentStaminaData { get; private set; }

    [Header("SO")]
    public float DefaultSpeed => AgentStaminaData.DefaultSpeed;
    public float RunSpeed => AgentStaminaData.RunSpeed;
    public float RechargeSpeed => AgentStaminaData.RechargeSpeed;                 // 현재바 회복 속도(초당)
    public float BackBarRechargeSpeed => AgentStaminaData.BackBarRechargeSpeed;   // 백바 회복 속도(초당)
    public float UseStaminaGage => AgentStaminaData.UseStaminaGage;               // 현재바 소모 속도(초당)

    // 이 값은 "Lerp t"에 쓰지 말고, "백바가 현재바를 따라 내려오는 속도(초당)"로 쓰자
    public float BackFollowStaminaBar => AgentStaminaData.BackFollowStaminaBar;

    [Header("Max/Current")]
    [field: SerializeField] public float _baseMaxStamina { get; private set; }
    [field: SerializeField] public float lastUseStaminaTime { get; private set; }
    [field: SerializeField] public float _currentMaxStamina { get; private set; }
    [field: SerializeField] public float _currentStamina { get; private set; }
    [field: SerializeField] public float _backStamina { get; private set; }

    [Header("패시브 보너스")]
    [field: SerializeField] public float bonusMaxStamina { get; private set; }

    private bool _isMove;
    private bool _runRequested;
    public bool _isRunning { get; private set; }
    private bool _isSpawn;

    private AgentMovement _agentMovement;
    private Agent _agent;
    public StaminaUI _staminaUI;

    public float MaxStaminaWithPassive => _baseMaxStamina + bonusMaxStamina;

    private void OnEnable()
    {
        if (CharacterSpawner.Instance != null)
            CharacterSpawner.Instance.OnCharacterSpawned += ResetStamina;
    }

    private void Awake()
    {
        _agentMovement = GetComponent<AgentMovement>();
        _agent = GetComponent<Agent>();
    }

    private void Start()
    {
        AgentStaminaData.MoveSpeed = DefaultSpeed;
        _isMove = _agent.RidCompo.linearVelocity.sqrMagnitude > 0.1f;
    }

    private void Update()
    {
        _isMove = _agent.RidCompo.linearVelocity.sqrMagnitude > 0.1f;

        bool canRun = _runRequested && _isMove && _currentStamina > 0f;
        _isRunning = canRun;

        if (canRun)
        {
            AgentStaminaData.MoveSpeed = RunSpeed;
            UseStamina();
        }
        else if (_isSpawn)
        {
            AgentStaminaData.MoveSpeed = DefaultSpeed;
            RechargeStamina();
        }
    }

    private void OnDisable()
    {
        if (CharacterSpawner.Instance != null)
            CharacterSpawner.Instance.OnCharacterSpawned -= ResetStamina;
    }

    private void ResetStamina()
    {
        InitFromSO();
        _isSpawn = true;
    }

    public void InitFromSO()
    {
        lastUseStaminaTime = 0f;

        _currentMaxStamina = MaxStaminaWithPassive;
        _currentStamina = _currentMaxStamina;
        _backStamina = _currentMaxStamina;

        if (_staminaUI != null)
            _staminaUI.UpdateUI();
    }

    private void UseStamina()
    {
        lastUseStaminaTime = 0f;

        // 1) 현재바는 즉시(빠르게) 감소
        _currentStamina -= UseStaminaGage * Time.deltaTime;

        // 2) 백바는 "현재바를 따라 내려오되" 초당 속도로 느리게 따라오게
        //    BackFollowStaminaBar를 "초당 따라오는 양"으로 해석
        if (_backStamina > _currentStamina)
        {
            _backStamina = Mathf.MoveTowards(
                _backStamina,
                _currentStamina,
                BackFollowStaminaBar * Time.deltaTime
            );
        }

        // 3) 현재바는 백바를 절대 넘지 못함(=백바가 현재 최대치 역할)
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, _backStamina);
        _currentMaxStamina = _backStamina;

        if (_staminaUI != null)
            _staminaUI.UpdateUI();
    }

    private void RechargeStamina()
    {
        lastUseStaminaTime = Mathf.Clamp(lastUseStaminaTime, 0f, 10f);
        lastUseStaminaTime += Time.deltaTime;

        if (lastUseStaminaTime >= 1.5f)
        {
            // 1) 백바(캡) 먼저 천천히 회복
            if (_backStamina < MaxStaminaWithPassive)
            {
                _backStamina += BackBarRechargeSpeed * Time.deltaTime;
                _backStamina = Mathf.Min(_backStamina, MaxStaminaWithPassive);
            }

            // 2) 현재바는 백바까지만 더 빠르게 회복
            if (_currentStamina < _backStamina)
            {
                _currentStamina += RechargeSpeed * Time.deltaTime;
                _currentStamina = Mathf.Min(_currentStamina, _backStamina); // ★ 이 줄이 핵심
            }
        }

        _currentMaxStamina = _backStamina; // 캡 반영

        if (_staminaUI != null)
            _staminaUI.UpdateUI();
    }

    public void SetRunning(bool isRunning)
    {
        _runRequested = isRunning;
    }

    public void AddBonusMaxStamina(float amount)
    {
        bonusMaxStamina += amount;

        _currentMaxStamina = MaxStaminaWithPassive;
        _currentStamina = _currentMaxStamina;
        _backStamina = _currentMaxStamina;

        if (_staminaUI != null)
            _staminaUI.UpdateUI();
    }
}
