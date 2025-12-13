using UnityEngine;

[def]
public class MousePointer : MonoBehaviour
{
    [SerializeField] private AgentMovementSO agentMovement;

    [Header("Recoil")]
    [SerializeField] private float recoilDamping = 18f;   // 흔들림 감쇠
    [SerializeField] private float recoilReturn = 22f;    // 중앙 복귀력
    [SerializeField] private float maxRadius = 220f;      // 중앙에서 벗어날 수 있는 최대 반경

    private RectTransform _rectTransform;
    private Camera _cam;

    private Vector2 _aimOffset;     // 현재 반동으로 밀린 위치
    private Vector2 _recoilVel;     // 반동 속도 (덕코프 핵심)

    public Vector2 position
    {
        get => _rectTransform.anchoredPosition;
        set => _rectTransform.anchoredPosition = value;
    }

    private void Awake()
    {
        _cam = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        UpdateRecoil(Time.deltaTime);
        ClampAim();
        ApplyPosition();
    }

    private void UpdateRecoil(float dt)
    {
        // 1. 속도 감쇠
        _recoilVel = Vector2.Lerp(
            _recoilVel,
            Vector2.zero,
            1f - Mathf.Exp(-recoilDamping * dt)
        );

        // 2. 중앙으로 끌어당기는 힘 (스프링)
        Vector2 toCenter = -_aimOffset;
        _recoilVel += toCenter * (recoilReturn * dt);

        // 3. 위치 갱신
        _aimOffset += _recoilVel * dt;
    }

    private void ClampAim()
    {
        if (_aimOffset.magnitude > maxRadius)
            _aimOffset = _aimOffset.normalized * maxRadius;
    }

    private void ApplyPosition()
    {
        // 기본 마우스 방향 + 반동 오프셋
        position = agentMovement.mouseDir + _aimOffset;
    }

    public void AddRecoil(Vector2 dir, float strength = 35f)
    {
        // 위치가 아니라 "속도"에 힘을 줌
        _recoilVel += dir * strength;
    }
}
