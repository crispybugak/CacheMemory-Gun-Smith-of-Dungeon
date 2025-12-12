using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    private Rigidbody2D rb;
    private float damage;
    private bool isLaunched;

    private float spawnTime;

    private ProjectilePool pool;
    private GameObject originalPrefab;

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

    private void OnEnable()
    {
        // 풀에서 꺼낼 때 초기화
        spawnTime = Time.time;
        isLaunched = false;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    public void SetPool(ProjectilePool p, GameObject prefab)
    {
        pool = p;
        originalPrefab = prefab;
    }

    public void Launch(Vector2 direction, float dmg, float speed)
    {
        direction = direction.normalized;
        damage = dmg;
        isLaunched = true;

        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}: Rigidbody2D 없음");
            return;
        }

        rb.linearVelocity = direction * speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        if (Time.time - spawnTime >= lifetime)
        {
            Despawn();
        }
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
            Despawn();
        }
        else if (collision.CompareTag("Wall"))
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        isLaunched = false;

        if (pool != null && originalPrefab != null)
        {
            pool.Return(this, originalPrefab);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
