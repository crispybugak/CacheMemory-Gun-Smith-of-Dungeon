using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected EnemyData enemyData;

    public EnemyData EnemyData
    {
        get => enemyData;
        set { enemyData = value; ValidateEnemyData(); CacheValues(); }
    }

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    protected Transform playerTransform;
    protected Health playerHealth;
    protected Seeker seeker;

    protected int currentHealth;
    protected float lastAttackTime = -999f;
    protected float lastSpecialTime = -999f;
    protected bool isFacingRight = true;
    protected Vector2 facingDirection = Vector2.right;
    protected Vector2 moveDirection = Vector2.zero;
    protected GameObject projectilePrefab;

    protected int hashIsMoving;

    private Path path;
    private int currentWaypoint;
    private bool pathPending;
    private bool isPathfindingActive = false;
    protected bool isChasing = false;

    [Header("패트롤 설정")]
    [SerializeField] private float patrolSpeed = 2.5f;
    [SerializeField] private float patrolDuration = 3f;
    [SerializeField] private float patrolRayDistance = 1.5f;

    private Vector2 patrolDirection = Vector2.right;
    private float lastPatrolChangeTime = 0f;

    [Header("시야 설정")]
    [SerializeField] private bool showFieldOfViewInGame = false;
    [SerializeField] private LayerMask obstacleLayer = ~0;

    [Header("원거리 공격")]
    [SerializeField] private bool isRangedEnemy = false;
    [SerializeField] private GameObject projectilePrefab_inspector;
    public float projectileSpeed { get; private set; } = 12f;

    [Header("플립 떨림 방지")]
    [SerializeField] private float flipDelay = 0.15f;
    private float lastFlipTime = 0f;

    [Header("추적 성능")]
    [SerializeField] private float pathUpdateInterval = 0.15f;
    private float nextPathUpdateTime = 0f;

    [Header("피격 효과")]
    [SerializeField] private float hitFlashDuration = 0.2f;
    [SerializeField] private Color hitFlashColor = new Color(1f, 1f, 1f, 0.6f);

    [Header("거리 유지 설정")]
    [SerializeField] private float rangedKeepDistance = 3.5f;
    [SerializeField] private float rangedBackAwaySpeed = 0.6f;

    [Header("혼잡 회피")]
    [SerializeField] private float pushForce = 2f;
    [SerializeField] private float pushRadius = 1.5f;

    [Header("공격 설정")]
    [SerializeField] protected float attackAnimationDuration = 0.6f;

    private Color originalColor;
    private bool isShowingAttackWarning = false;

    private float sqrDetectionRange;
    private float sqrAttackRange;
    private float sqrMeleeMinDistance;
    private float sqrRangedMinDistance;
    private float sqrRangedKeepDistance;

    protected bool IsSafeToUpdate => enemyData != null && playerTransform != null && enabled;

    protected virtual void Start()
    {
        InitializeComponents();
        FindPlayer();
        ValidateEnemyData();

        if (IsSafeToUpdate)
        {
            CacheValues();
        }

        currentHealth = enemyData.maxHealth;
        lastAttackTime = -enemyData.attackCooldown;
        lastSpecialTime = -999f;

        lastPatrolChangeTime = Time.time;
        patrolDirection = Random.insideUnitCircle.normalized;
        
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    protected virtual void Update()
    {
        if (!IsSafeToUpdate)
        {
            Patrol();
            ApplyMovement();
            UpdateFacingAndFlip();
            PushAwayFromEnemies();
            return;
        }

        float sqrDist = Vector2.SqrMagnitude((Vector2)playerTransform.position - (Vector2)transform.position);

        if (sqrDist < sqrAttackRange)
        {
            HandleAttackRange(sqrDist);
        }
        else if (sqrDist < sqrDetectionRange)
        {
            HandleChaseRange(sqrDist);
        }
        else
        {
            isChasing = false;
            DisablePathfinding();
            Patrol();
        }

        ApplyMovement();
        UpdateFacingAndFlip();
        PushAwayFromEnemies();

        if (isChasing && Time.time > nextPathUpdateTime)
        {
            UpdatePathNow();
            nextPathUpdateTime = Time.time + pathUpdateInterval;
        }
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        projectilePrefab = projectilePrefab_inspector;

        hashIsMoving = Animator.StringToHash("isMoving");

        rb.gravityScale = 0f;
        rb.linearDamping = 4f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    private void FindPlayer()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            playerTransform = playerGO.transform;
            playerHealth = playerGO.GetComponent<Health>();
        }
    }

    private void CacheValues()
    {
        sqrDetectionRange = enemyData.detectionRange * enemyData.detectionRange;
        sqrAttackRange = enemyData.attackRange * enemyData.attackRange;
        
        sqrMeleeMinDistance = Mathf.Pow(enemyData.attackRange * 0.7f, 2);
        sqrRangedMinDistance = Mathf.Pow(rangedKeepDistance * 0.8f, 2);
        sqrRangedKeepDistance = Mathf.Pow(rangedKeepDistance, 2);
    }

    private void ValidateEnemyData()
    {
        if (enemyData == null)
        {
            Debug.LogError($"{name}: EnemyData 없음!", this);
            enabled = false;
        }
    }

    private void HandleAttackRange(float sqrDist)
    {
        isChasing = true;
        
        if (isRangedEnemy)
        {
            if (sqrDist < sqrRangedMinDistance)
            {
                DisablePathfinding();
                BackAwayFromPlayer(rangedBackAwaySpeed);
            }
            else if (sqrDist < sqrRangedKeepDistance)
            {
                DisablePathfinding();
                Idle();
            }
            else
            {
                EnablePathfinding();
                MoveTowardPlayer();
            }
        }
        else
        {
            if (sqrDist < sqrMeleeMinDistance)
            {
                DisablePathfinding();
                BackAwayFromPlayer(enemyData.moveSpeed * 0.5f);
            }
            else
            {
                DisablePathfinding();
                Idle();
            }
        }

        if (Time.time - lastAttackTime >= enemyData.attackCooldown)
        {
            AttackWithWarning();
        }
    }

    private void HandleChaseRange(float sqrDist)
    {
        isChasing = true;
        EnablePathfinding();

        if (isRangedEnemy && sqrDist > sqrRangedKeepDistance)
        {
            MoveTowardPlayer();
        }
        else if (!isRangedEnemy)
        {
            MoveTowardPlayer();
        }
        else
        {
            Idle();
        }
    }

    private void Patrol()
    {
        DisablePathfinding();
        isChasing = false;

        Vector2 rayOrigin = (Vector2)transform.position + patrolDirection * 0.6f;
        if (Physics2D.Raycast(rayOrigin, patrolDirection, patrolRayDistance, obstacleLayer))
        {
            ChangePatrolDirection();
            return;
        }

        if (Time.time - lastPatrolChangeTime >= patrolDuration)
        {
            ChangePatrolDirection();
            lastPatrolChangeTime = Time.time;
            return;
        }

        moveDirection = patrolDirection.normalized;
    }

    private void ChangePatrolDirection()
    {
        var dirs = new Vector2[]
        {
            Vector2.right, Vector2.left, Vector2.up, Vector2.down,
            (Vector2.right + Vector2.up).normalized,
            (Vector2.right + Vector2.down).normalized,
            (Vector2.left + Vector2.up).normalized,
            (Vector2.left + Vector2.down).normalized
        };

        var valid = new List<Vector2>();
        foreach (var dir in dirs)
        {
            if (!Physics2D.Raycast(transform.position + (Vector3)dir * 0.6f, dir, patrolRayDistance, obstacleLayer))
                valid.Add(dir);
        }

        patrolDirection = valid.Count > 0 ? valid[Random.Range(0, valid.Count)] : Random.insideUnitCircle.normalized;
    }

    private void EnablePathfinding()
    {
        if (isPathfindingActive || seeker == null) return;
        isPathfindingActive = true;
        UpdatePathNow();
    }

    private void DisablePathfinding()
    {
        isPathfindingActive = false;
    }

    private void UpdatePathNow()
    {
        if (seeker == null || playerTransform == null || pathPending) return;
        pathPending = true;
        seeker.StartPath(transform.position, playerTransform.position, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
        pathPending = false;
    }

    protected void MoveTowardPlayer()
    {
        if (playerTransform == null) return;

        if (path == null || path.vectorPath.Count == 0 || pathPending)
        {
            moveDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            moveDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            return;
        }

        Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);

        if (dist < enemyData.stoppingDistance)
            currentWaypoint++;

        moveDirection = dir;
    }

    private void ApplyMovement()
    {
        if (rb == null) return;

        bool moving = moveDirection.sqrMagnitude > 0.01f;
        animator?.SetBool(hashIsMoving, moving);

        float speed = isChasing ? enemyData.moveSpeed : patrolSpeed;
        rb.linearVelocity = moving ? moveDirection * speed : Vector2.zero;
    }

    private void UpdateFacingAndFlip()
    {
        Vector2 targetDirection;
        if (moveDirection.sqrMagnitude < 0.01f && playerTransform != null)
        {
            targetDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        }
        else
        {
            targetDirection = moveDirection.normalized;
        }

        if (targetDirection.sqrMagnitude < 0.01f) return;

        facingDirection = targetDirection;

        if (Time.time - lastFlipTime < flipDelay) return;

        bool shouldFaceRight = facingDirection.x > 0.15f;
        bool shouldFaceLeft = facingDirection.x < -0.15f;

        if (shouldFaceRight && !isFacingRight)
        {
            isFacingRight = true;
            spriteRenderer.flipX = false;
            lastFlipTime = Time.time;
        }
        else if (shouldFaceLeft && isFacingRight)
        {
            isFacingRight = false;
            spriteRenderer.flipX = true;
            lastFlipTime = Time.time;
        }
    }

    private void PushAwayFromEnemies()
    {
        if (rb == null) return;

        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            pushRadius,
            LayerMask.GetMask("Enemy"));

        foreach (var enemy in nearbyEnemies)
        {
            if (enemy.gameObject == gameObject) continue;

            BaseEnemy otherEnemy = enemy.GetComponent<BaseEnemy>();
            if (otherEnemy == null) continue;

            Vector2 pushDirection = ((Vector2)transform.position - (Vector2)enemy.transform.position).normalized;
            rb.linearVelocity += pushDirection * pushForce * Time.deltaTime;
        }
    }

    private void BackAwayFromPlayer(float speed = -1f)
    {
        if (speed < 0) speed = enemyData.moveSpeed * 0.5f;
        
        Vector2 awayDirection = ((Vector2)transform.position - (Vector2)playerTransform.position).normalized;
        moveDirection = awayDirection;
        
        if (rb != null)
        {
            rb.linearVelocity = awayDirection * speed;
        }
    }

    protected virtual void Idle() => moveDirection = Vector2.zero;

    private void AttackWithWarning()
    {
        lastAttackTime = Time.time;
        StartCoroutine(AttackWarningSequence());
    }

    private IEnumerator AttackWarningSequence()
    {
        isShowingAttackWarning = true;
        Attack();

        yield return new WaitForSeconds(attackAnimationDuration);

        ApplyAttackDamage();

        isShowingAttackWarning = false;
    }

    protected virtual void Attack()
    {
        // 서브클래스에서 오버라이드
    }

    protected virtual void ApplyAttackDamage()
    {
        if (playerTransform == null) return;

        float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        if (distToPlayer <= enemyData.attackRange * 1.2f)
        {
            if (playerHealth != null)
            {
                playerHealth.OnDamaged(enemyData.attackDamage);
            }
        }
    }

    protected virtual void PerformAttack()
    {
        // 서브클래스에서 구현
    }

    protected bool TryDamagePlayer(int damage)
    {
        if (playerHealth != null)
        {
            playerHealth.OnDamaged(damage);
            return true;
        }
        return false;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= Mathf.RoundToInt(damage);
        StartCoroutine(HitFlash());

        if (currentHealth <= 0) Die();
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;
        Color original = spriteRenderer.color;
        
        spriteRenderer.color = hitFlashColor;
        
        yield return new WaitForSeconds(hitFlashDuration);
        
        spriteRenderer.color = original;
    }

    protected virtual void Die()
    {
        DisablePathfinding();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        enabled = false;
        Destroy(gameObject, 2f);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }

    public bool IsShowingAttackWarning => isShowingAttackWarning;

    protected Transform GetPlayerTransform() => playerTransform;
    protected Animator GetAnimator() => animator;
    protected EnemyData GetEnemyData() => enemyData;
}
