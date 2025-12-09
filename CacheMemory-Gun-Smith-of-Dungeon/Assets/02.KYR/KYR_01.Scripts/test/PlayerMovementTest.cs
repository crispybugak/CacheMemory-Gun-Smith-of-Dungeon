using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementTest : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D _rb;
    private Vector2 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 키보드 입력 받기 (WASD, 방향키)
        _input.x = Input.GetAxisRaw("Horizontal"); // A,D / ←, →
        _input.y = Input.GetAxisRaw("Vertical");   // W,S / ↑, ↓

        // 대각선 이동 속도 보정
        _input = _input.normalized;
    }

    private void FixedUpdate()
    {
        // 물리 업데이트에서 속도 적용
        _rb.linearVelocity = _input * _moveSpeed;
    }
}