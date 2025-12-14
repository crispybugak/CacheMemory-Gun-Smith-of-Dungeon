using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float damage = 2f;
    [SerializeField] private float damageRadius = 1f;
    private float timer;
    private bool damageCaused;
    private void Start()
    {
        timer = duration;
        Destroy(gameObject, duration);
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if (!damageCaused && timer < duration * 0.5f)
        {
            CauseDamage();
            damageCaused = true;
        }
    }
    private void CauseDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") && hit.TryGetComponent<Health>(out var health))
            {
                health.OnDamaged(damage);
            }
        }
    }
}