using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("EnemyBullet")]
    public float damage = 4f;
    public float lifeTime = 3f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"데미지: {damage}");
            Destroy(gameObject);
        }
    }
}