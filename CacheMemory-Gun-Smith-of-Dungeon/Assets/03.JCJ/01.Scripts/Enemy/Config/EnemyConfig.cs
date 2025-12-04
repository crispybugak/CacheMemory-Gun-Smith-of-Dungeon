using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public float maxHealth = 100f;
    public float attackCooldown = 1f;
    public float detectionRange = 15f;

    public float attackPower = 10f;
    public float moveSpeed = 2f;
}

[System.Serializable]
public class EnemyCombatStats
{
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float defenseRate = 0.1f;

    public bool canAttack = true;
}

[System.Serializable]
public class EnemyMoveStats
{
    public float baseSpeed = 2f;
    public float chaseSpeed = 3.5f;
}

[System.Serializable]
public class RangedEnemyStats
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
}

[CreateAssetMenu(menuName = "Enemy/Config")]
public class EnemyConfig : ScriptableObject
{
    public EnemyStats stats;
    public EnemyCombatStats combatStats;
    public EnemyMoveStats moveStats;

    public RangedEnemyStats rangedStats;

    public EnemyStats GetStats() => stats;
    public EnemyCombatStats GetCombatStats() => combatStats;
    public EnemyMoveStats GetMoveStats() => moveStats;

    public GameObject GetBulletPrefab() => rangedStats?.bulletPrefab;
    public float GetBulletSpeed() => rangedStats?.bulletSpeed ?? 5f;
}