using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseEnemy : MonoBehaviour
{
    public event Action<BaseEnemy> OnDeath;
    [SerializeField] protected EnemyData enemyData;

    public EnemyData EnemyData
    {
        get => enemyData;
        set { enemyData = value; ValidateEnemyData(); CacheValues(); }
    }

    public int CurrentHealth => currentHealth;
    public int MaxHealth => enemyData != null ? enemyData.maxHealth : 1;

    public event Action<int, int> OnHealthChanged;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    protected Transform playerTransform;
    protected Health playerHealth;
    protected Collider2D playerCollider;
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
    [SerializeField] protected bool isRangedEnemy = false;
    [SerializeField] private GameObject projectilePrefab_inspector;
    public float projectileSpeed { get; private set; } = 12f;

    [Header("플립 떨림 방지")]
    [SerializeField] private float flipDelay = 0.1f;
    private float lastFlipTime = 0f;
    private float flipThreshold = 0.05f;

    [Header("추적 성능")]
    [SerializeField] private float pathUpdateInterval = 0.15f;
    private float nextPathUpdateTime = 0f;

    [Header("피격 효과")]
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private Color hitFlashColor = Color.red;
    
    private Color originalSpriteColor;

    [Header("사망 연출")]
    [SerializeField] private float deathFadeDuration = 0.6f;

    [Header("거리 유지 설정")]
    [SerializeField] private float rangedKeepDistance = 3.5f;
    [SerializeField] private float rangedBackAwaySpeed = 0.6f;

    [Header("길막 방지 (Traffic Jam Fix)")]
    [SerializeField] private float separationRadius = 1.5f;
    [SerializeField] private float separationWeight = 4.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("공격 설정")]
    [SerializeField] protected float attackAnimationDuration = 0.6f;

    private Color originalColor;
    private bool isShowingAttackWarning = false;

    [Header("시선 확인")]
    [SerializeField] private bool useLineOfSightCheck = true;

    [Header("애니메이션 Bool 옵션")]
    protected string hurtBoolName = "isHurt";
    protected string deadBoolName = "isDead"; 
    [SerializeField] private float hurtBoolDuration = 0.15f;

    private bool hasHurtBool;
    private bool hasDeadBool;

    private float sqrDetectionRange;
    private float sqrAttackRange;
    private float sqrMeleeMinDistance;
    private float sqrRangedMinDistance;
    private float sqrRangedKeepDistance;
    private bool isDead = false;
    protected bool IsSafeToUpdate => enemyData != null && playerTransform != null && enabled;

    protected void RaiseHealthChanged()
    {
        if (enemyData == null) return;
        OnHealthChanged?.Invoke(currentHealth, enemyData.maxHealth);
    }

    protected virtual void Start()
    {
        InitializeComponents();
        FindPlayer();
        ValidateEnemyData();
        InitAnimatorParams();

        if (IsSafeToUpdate)
        {
            CacheValues();
        }

        currentHealth = enemyData.maxHealth;
        lastAttackTime = -enemyData.attackCooldown;
        lastSpecialTime = Time.time;

        lastPatrolChangeTime = Time.time;
        patrolDirection = Random.insideUnitCircle.normalized;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            originalSpriteColor = spriteRenderer.color;
        }

        RaiseHealthChanged();
    }

    protected virtual void Update()
    {
        if (!IsSafeToUpdate)
        {
            Patrol();
            ApplyMovement();
            UpdateFacingAndFlip();
            return;
        }

        Vector2 toPlayer = (Vector2)playerTransform.position - (Vector2)transform.position;
        float dist = toPlayer.magnitude;

        if (dist > enemyData.detectionRange)
        {
            isChasing = false;
            DisablePathfinding();
            Patrol();
        }
        else
        {
            isChasing = true;

            if (isRangedEnemy)
            {
                UpdateRangedMovement(dist);
            }
            else
            {
                UpdateMeleeMovement(dist);
            }

            TryAttack(dist);
        }

        ApplyMovement();
        UpdateFacingAndFlip();

        if (isChasing && Time.time > nextPathUpdateTime)
        {
            UpdatePathNow();
            nextPathUpdateTime = Time.time + pathUpdateInterval;
        }
    }
    
    private void UpdateMeleeMovement(float dist)
    {
        float approachDistance = enemyData.attackRange * 0.8f;

        if (dist > approachDistance)
        {
            EnablePathfinding();
            MoveTowardPlayer();
        }
        else
        {
            DisablePathfinding();
            moveDirection = Vector2.zero;

            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }

    private void UpdateRangedMovement(float dist)
    {
        float minDist = rangedKeepDistance * 0.8f;
        float maxDist = rangedKeepDistance * 1.2f;

        if (dist > maxDist)
        {
            EnablePathfinding();
            MoveTowardPlayer();
        }
        else if (dist < minDist)
        {
            DisablePathfinding();
            BackAwayFromPlayer(rangedBackAwaySpeed);
        }
        else
        {
            DisablePathfinding();
            moveDirection = Vector2.zero;

            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }

    private void TryAttack(float distCenter)
    {
        if (enemyData == null) return;

        float actualDist = distCenter;

        if (playerCollider != null)
        {
            Vector2 closestPoint = playerCollider.ClosestPoint(transform.position);
            actualDist = Vector2.Distance(transform.position, closestPoint);
        }

        float effectiveRange = enemyData.attackRange;

        if (actualDist > effectiveRange)
            return;

        if (Time.time - lastAttackTime < enemyData.attackCooldown)
            return;

        if (isRangedEnemy && useLineOfSightCheck && !HasLineOfSightToPlayer())
            return;

        AttackWithWarning();
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        projectilePrefab = projectilePrefab_inspector;

        hashIsMoving = Animator.StringToHash("isMoving");

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearDamping = 4f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }
    }

    private void InitAnimatorParams()
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(hurtBoolName))
            hasHurtBool = HasAnimatorBool(hurtBoolName);

        if (!string.IsNullOrEmpty(deadBoolName))
            hasDeadBool = HasAnimatorBool(deadBoolName);
    }

    private bool HasAnimatorBool(string name)
    {
        if (animator == null) return false;
    
        foreach (var p in animator.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Bool && p.name == name)
                return true;
        }
        return false;
    }

    private void FindPlayer()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            playerTransform = playerGO.transform;
            playerHealth = playerGO.GetComponent<Health>();
            playerCollider = playerGO.GetComponent<Collider2D>();
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
            Debug.LogError($"{name}: EnemyData", this);
            enabled = false;
        }
    }

    private void HandleAttackRange(float sqrDist)
    {
        float dist = Mathf.Sqrt(sqrDist);

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

            if (Time.time - lastAttackTime >= enemyData.attackCooldown)
            {
                if (!useLineOfSightCheck || HasLineOfSightToPlayer())
                {
                    AttackWithWarning();
                }
            }

            return;
        }

        if (dist > enemyData.attackRange)
        {
            EnablePathfinding();
            MoveTowardPlayer();
            return;
        }

        DisablePathfinding();
        moveDirection = Vector2.zero;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (Time.time - lastAttackTime >= enemyData.attackCooldown)
        {
            AttackWithWarning();
        }
    }

    private void TryAttackIfReady(float sqrDistToPlayer)
    {
        if (enemyData == null || playerTransform == null)
            return;

        if (sqrDistToPlayer > sqrAttackRange)
            return;

        if (Time.time - lastAttackTime < enemyData.attackCooldown)
            return;

        if (isRangedEnemy && useLineOfSightCheck && !HasLineOfSightToPlayer())
            return;

        AttackWithWarning();
    }

    private void HandleChaseRange(float sqrDist)
    {
        isChasing = true;
        EnablePathfinding();
        MoveTowardPlayer();
    }

    private bool HasLineOfSightToPlayer()
    {
        if (playerTransform == null) return false;

        Vector2 dirToPlayer = (Vector2)playerTransform.position - (Vector2)transform.position;
        float distToPlayer = dirToPlayer.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            dirToPlayer.normalized,
            distToPlayer,
            obstacleLayer);

        return !hit.collider || hit.collider.CompareTag("Player");
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

        Vector2 offset2D = Random.insideUnitCircle * 0.5f;
        Vector3 offset = new Vector3(offset2D.x, offset2D.y, 0f);

        Vector3 targetPosition = playerTransform.position + offset;

        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
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

        Vector2 finalDirection = moveDirection;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Vector2 separation = CalculateSeparationVector();
            finalDirection += separation * separationWeight;
        }

        finalDirection.Normalize();

        float speed = isChasing ? enemyData.moveSpeed : patrolSpeed;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = finalDirection * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        bool isActuallyMoving = rb.linearVelocity.sqrMagnitude > 0.1f;
        animator?.SetBool(hashIsMoving, isActuallyMoving);
    }

    private Vector2 CalculateSeparationVector()
    {
        Vector2 separation = Vector2.zero;

        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, separationRadius, enemyLayer);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject) continue;

            Vector2 pushDir = (Vector2)transform.position - (Vector2)neighbor.transform.position;
            float dist = pushDir.magnitude;

            if (dist > 0.01f)
            {
                separation += pushDir.normalized / dist;
            }
        }

        return separation;
    }

    private void UpdateFacingAndFlip()
    {
        Vector2 targetDirection = Vector2.zero;

        if (playerTransform != null && (isChasing || Vector2.SqrMagnitude((Vector2)playerTransform.position - (Vector2)transform.position) < sqrDetectionRange))
        {
            targetDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        }
        else if (moveDirection.sqrMagnitude > 0.01f)
        {
            targetDirection = moveDirection.normalized;
        }

        if (targetDirection.sqrMagnitude < 0.01f) return;

        facingDirection = targetDirection;

        if (Time.time - lastFlipTime < flipDelay) return;
        if (isShowingAttackWarning) return;

        bool shouldFaceRight = facingDirection.x > flipThreshold;
        bool shouldFaceLeft = facingDirection.x < -flipThreshold;

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

    private void BackAwayFromPlayer(float speed = -1f)
    {
        if (speed < 0) speed = enemyData.moveSpeed * 0.5f;

        Vector2 awayDirection = (Vector2)transform.position - (Vector2)playerTransform.position;
        awayDirection.Normalize();
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
    }

    protected virtual void ApplyAttackDamage()
    {
        if (playerHealth == null || playerCollider == null) return;

        Vector2 closestPoint = playerCollider.ClosestPoint(transform.position);
        float dist = Vector2.Distance(transform.position, closestPoint);
        float effectiveRange = enemyData.attackRange;

        if (dist <= effectiveRange)
        {
            playerHealth.OnDamaged(enemyData.attackDamage);
            GameManager.Instance.HitTimeScale();
        }
    }

    protected virtual void PerformAttack()
    {
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

    protected void PlayHurtAnim()
    {
        if (animator != null && hasHurtBool)
        {
            animator.SetBool(hurtBoolName, true);
            StartCoroutine(ResetHurtBool());
        }
    }

    private IEnumerator ResetHurtBool()
    {
        yield return new WaitForSeconds(hurtBoolDuration);
        if (animator != null && hasHurtBool)
            animator.SetBool(hurtBoolName, false);
    }

    protected void PlayDeadAnim()
    {
        if (animator != null && hasDeadBool)
        {
            animator.SetBool(deadBoolName, true);
        }
    }
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= Mathf.RoundToInt(damage);

        RaiseHealthChanged();
        PlayHurtAnim();
        StartCoroutine(HitFlash());

        if (currentHealth <= 0) Die();
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);

        if (spriteRenderer != null)
            spriteRenderer.color = originalSpriteColor;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        isChasing = false;
        moveDirection = Vector2.zero;
        DisablePathfinding();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (animator != null)
        {
            animator.SetBool(hashIsMoving, false);
        }

        OnDeath?.Invoke(this);

        PlayDeadAnim();
        enabled = false;

        StartCoroutine(DeathFadeAndDestroy());
    }

    private IEnumerator DeathFadeAndDestroy()
    {
        if (spriteRenderer != null)
        {
            Color start = spriteRenderer.color;
            float t = 0f;

            while (t < deathFadeDuration)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(1f, 0f, t / deathFadeDuration);
                spriteRenderer.color = new Color(start.r, start.g, start.b, a);
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }

    public bool IsShowingAttackWarning => isShowingAttackWarning;

    protected Transform GetPlayerTransform() => playerTransform;
    protected Animator GetAnimator() => animator;
    protected EnemyData GetEnemyData() => enemyData;
}
