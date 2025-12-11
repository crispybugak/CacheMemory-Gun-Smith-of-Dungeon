using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    private Rigidbody2D rb;
    private float damage;
    private bool isLaunched;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearDamping = 0f;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    public void Launch(Vector2 direction, float dmg, float speed)
    {
        direction = direction.normalized;
        damage = dmg;
        isLaunched = true;
        if (rb == null) return;
        rb.linearVelocity = direction * speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLaunched) return;
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<Health>(out var health))
            {
                health.OnDamaged(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}