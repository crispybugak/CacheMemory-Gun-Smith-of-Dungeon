using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_", menuName = "Game/Enemy Data", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("기본 스탯")]
    public float moveSpeed = 2f;
    public int maxHealth = 20;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 5;

    [Header("A* 경로 설정")]
    public float pathUpdateInterval = 0.3f;
    public float stoppingDistance = 0.2f;

    [Header("특수 능력")]
    public float specialAbilityCooldown = 2f;
    public float specialAbilityRange = 3f;
    public int specialAbilityDamage = 6;

    // ========== BOAR 특수 ==========
    [Header("Boar 특수")]
    public float sleepDuration = 2.5f;

    // ========== BOMB 특수 ==========
    [Header("Bomb 특수")]
    public float explosionRadius = 2f;
    public float explosionForce = 10f;

    // ========== NECROMANCER 특수 ==========
    [Header("Necromancer 특수")]
    public int maxMinions = 3;
    public float spawnCooldown = 4f;
}