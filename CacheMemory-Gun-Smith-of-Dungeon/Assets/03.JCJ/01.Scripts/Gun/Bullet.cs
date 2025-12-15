using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 10f;
    private Vector2 startPos;
    public float distance = 10f;
    public SpriteRenderer _spr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * speed;
        if (Vector2.Distance(startPos, transform.position) > distance)
            Destroy(gameObject);
    }
    
    public void ResetBullet()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}