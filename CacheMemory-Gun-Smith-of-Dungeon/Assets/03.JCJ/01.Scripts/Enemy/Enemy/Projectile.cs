using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 5f;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    private float damage;
    private bool isLaunched;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }
    
    public void Launch(Vector2 launchDir, float dmg)
    {
        direction = launchDir.normalized;
        damage = dmg;
        isLaunched = true;
        
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        
        // 회전 설정
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLaunched) return;
        
        // 플레이어 피해
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<Health>(out var health))
            {
                health.OnDamaged(damage);
            }
            Destroy(gameObject);
            return;
        }
        
        // 벽 충돌
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }
    }
}