using UnityEngine;
using Pathfinding;

public abstract class Enemy : MonoBehaviour
{
    public EnemyConfig config;
    public EnemyStateMachine stateMachine;
    public Transform targetTransform;
    public float currentHealth;
    public bool isAlive = true;

    [Header("Vision")]
    public float visionRange = 10f;
    [Range(0f, 360f)]
    public float visionAngle = 90f;
    [Range(-180f, 180f)]
    public float visionRotationOffset = 0f;
    public LayerMask visionLayerMask;
    public LayerMask obstacleLayerMask;

    [Header("Patrol")]
    public PatrolPoint patrolPoint;

    [Header("AI Flags")]
    public bool canPatrol = true;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float lastAttackTime;
    private Vector2 lastFacingDirection = Vector2.right;

    private AIPath aiPath;      
    private AILerp aiLerp;         
    private float originalSpeed;

    private bool hasPlayerInSight = false;
    private float visionCheckDelay = 0.2f;
    private float visionCheckTimer = 0f;

    private float loseSightDelay = 0.5f;
    private float loseSightTimer = 0f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        aiPath = GetComponent<AIPath>();
        aiLerp = GetComponent<AILerp>();

        if (aiPath == null && aiLerp == null)
        {
            Debug.LogError($"[{name}] Neither AIPath nor AILerp found!");
        }
    }

    protected virtual void Start()
    {
        if (config == null)
        {
            Debug.LogError("Config not assigned to " + gameObject.name);
            return;
        }

        currentHealth = config.GetStats().maxHealth;

        // 시작 시 기본 속도 적용
        originalSpeed = config.GetMoveStats().baseSpeed;
        ApplySpeed(originalSpeed);

        InitializeStateMachine();
    }

    protected virtual void Update()
    {
        if (!isAlive) return;

        UpdateFacingDirectionFromTransform();
        CheckVision();

        stateMachine?.Execute();
    }


    private void UpdateFacingDirectionFromTransform()
    {
        float rotationZ = transform.eulerAngles.z;
        float radians = rotationZ * Mathf.Deg2Rad;
        lastFacingDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        if (Mathf.Abs(lastFacingDirection.x) > 0.1f)
            spriteRenderer.flipX = lastFacingDirection.x < 0;
    }

    private void CheckVision()
    {
        visionCheckTimer += Time.deltaTime;
        if (visionCheckTimer < visionCheckDelay) return;
        visionCheckTimer = 0f;

        if (targetTransform != null)
        {
            if (IsInVisionCone(targetTransform))
            {
                hasPlayerInSight = true;
                loseSightTimer = 0f;
            }
            else
            {
                loseSightTimer += visionCheckDelay;
                if (loseSightTimer >= loseSightDelay)
                {
                    hasPlayerInSight = false;
                    targetTransform = null;
                }
            }
            return;
        }
        loseSightTimer = 0f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            visionRange,
            visionLayerMask
        );

        foreach (Collider2D col in colliders)
        {
            Transform potentialTarget = col.transform;

            if (IsInVisionCone(potentialTarget))
            {
                targetTransform = potentialTarget;
                hasPlayerInSight = true;
                return;
            }
        }
    }

    public bool IsInVisionCone(Transform target)
    {
        if (target == null) return false;

        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget > visionRange) return false;

        float playerAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        if (playerAngle < 0) playerAngle += 360f;

        float enemyRotZ = transform.eulerAngles.z;
        float centerAngle = NormalizeAngle(enemyRotZ + visionRotationOffset);

        float half = visionAngle * 0.5f;
        float start = NormalizeAngle(centerAngle - half);
        float end = NormalizeAngle(centerAngle + half);

        bool inRange;
        if (start < end)
            inRange = playerAngle >= start && playerAngle <= end;
        else
            inRange = playerAngle >= start || playerAngle <= end;

        if (!inRange) return false;
        if (!IsLineOfSightClear(target.position)) return false;

        return true;
    }

    private bool IsLineOfSightClear(Vector3 targetPosition)
    {
        Vector2 origin = transform.position;
        Vector2 dir = (targetPosition - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, targetPosition);

        Debug.DrawRay(origin, dir * dist, Color.yellow, 0.1f);

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            dir,
            dist,
            obstacleLayerMask
        );

        if (hit.collider != null)
        {
            Debug.DrawRay(origin, dir * dist, Color.red, 0.1f);
            return false;
        }

        return true;
    }

    private static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }


    private void ApplySpeed(float speed)
    {
        if (aiPath != null)
        {
            aiPath.maxSpeed = speed;
            Debug.Log($"[{name}] AIPath.maxSpeed set to: {speed}");
        }

        if (aiLerp != null)
        {
            aiLerp.speed = speed;
            Debug.Log($"[{name}] AILerp.speed set to: {speed}");
        }
    }

    public void SetMoveSpeed(float speed)
    {
        ApplySpeed(speed);
    }

    public void SetChaseSpeed()
    {
        if (config == null) return;

        if (aiPath != null)
            originalSpeed = aiPath.maxSpeed;
        else if (aiLerp != null)
            originalSpeed = aiLerp.speed;

        ApplySpeed(config.GetMoveStats().chaseSpeed);
    }

    public void RestoreOriginalSpeed()
    {
        ApplySpeed(originalSpeed);
    }

    public void SetCanPatrol(bool value)
    {
        canPatrol = value;
    }

    public void SetAIDestination(Transform t, string reason = "")
    {
        var ai = GetComponent<AIDestinationSetter>();
        if (ai == null) return;

        ai.target = t;
    }


    public void MoveToward(Vector3 targetPos, float speed)
    {
        Vector2 dir = (targetPos - transform.position).normalized;
        rb.linearVelocity = dir * speed;

        if (dir.x != 0)
            spriteRenderer.flipX = dir.x < 0;
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
    }


    public bool IsInDetectionRange(Transform target)
    {
        if (target == null) return false;
        return Vector2.Distance(transform.position, target.position)
               <= config.GetStats().detectionRange;
    }

    public bool IsInAttackRange(Transform target)
    {
        if (target == null) return false;
        return Vector2.Distance(transform.position, target.position)
               <= config.GetCombatStats().attackRange;
    }


    public bool CanAttack()
        => Time.time >= lastAttackTime + config.GetStats().attackCooldown;

    public void SetLastAttackTime()
        => lastAttackTime = Time.time;


    public virtual void TakeDamage(float damage)
    {
        float actualDamage = damage * (1f - config.GetCombatStats().defenseRate);
        currentHealth -= actualDamage;

        if (currentHealth <= 0)
            Die();
    }

    public virtual void Die()
    {
        isAlive = false;
        stateMachine.ChangeState(EnemyStateType.Dead);
        Destroy(gameObject, 1f);
    }

    public Transform GetTarget() => targetTransform;
    public float GetCurrentHealth() => currentHealth;
    public bool IsAlive() => isAlive;
    public PatrolPoint GetPatrolPoint() => patrolPoint;
    public Vector2 GetFacingDirection() => lastFacingDirection;
    public bool HasPlayerInSight() => hasPlayerInSight;

    protected abstract void InitializeStateMachine();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        float enemyRotZ = transform.eulerAngles.z;
        float centerAngle = NormalizeAngle(enemyRotZ + visionRotationOffset);
        float half = visionAngle * 0.5f;

        Gizmos.color = Color.green;
        float startRad = (centerAngle - half) * Mathf.Deg2Rad;
        float endRad = (centerAngle + half) * Mathf.Deg2Rad;
        Vector2 startDir = new Vector2(Mathf.Cos(startRad), Mathf.Sin(startRad));
        Vector2 endDir = new Vector2(Mathf.Cos(endRad), Mathf.Sin(endRad));
        Gizmos.DrawLine(transform.position, (Vector3)transform.position + (Vector3)startDir * visionRange);
        Gizmos.DrawLine(transform.position, (Vector3)transform.position + (Vector3)endDir * visionRange);

        Gizmos.color = Color.blue;
        float centerRad = centerAngle * Mathf.Deg2Rad;
        Vector2 centerDir = new Vector2(Mathf.Cos(centerRad), Mathf.Sin(centerRad));
        Gizmos.DrawLine(transform.position, (Vector3)transform.position + (Vector3)centerDir * visionRange);

        if (Application.isPlaying && targetTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetTransform.position);
        }
    }
}
