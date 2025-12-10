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

    // 컴포넌트
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private Health playerHealth;
    private Seeker seeker;
    private LineRenderer lineRenderer;
    private AIPath _conflictingAIPath;

    // 상태
    private int currentHealth;
    private float lastAttackTime = -999f;
    private bool isFacingRight = true;
    private Vector2 facingDirection = Vector2.right;
    private Vector2 moveDirection = Vector2.zero;

    // A* 경로탐색
    private Path path;
    private int currentWaypoint;
    private bool pathPending;
    private bool isPathfindingActive = false;
    private bool isChasing = false;

    // 패트롤 (이제 절대 멈추지 않음!)
    [Header("패트롤 설정")]
    [SerializeField] private float patrolSpeed = 2.5f;
    [SerializeField] private float patrolDuration = 3f;        // 몇 초마다 방향 바꿀지
    [SerializeField] private float patrolRayDistance = 1.5f;    // 벽 감지 거리

    private Vector2 patrolDirection = Vector2.right;
    private float lastPatrolChangeTime = 0f;

    // 시야
    [Header("시야 설정")]
    [SerializeField] private float fieldOfViewAngle = 120f;
    [SerializeField] private bool showFieldOfViewGizmos = true;
    [SerializeField] private bool showFieldOfViewInGame = true;
    [SerializeField] private LayerMask obstacleLayer = ~0;

    // 원거리 공격
    [Header("원거리 공격")]
    [SerializeField] private bool isRangedEnemy = false;
    [SerializeField] private GameObject projectilePrefab;
    public float projectileSpeed { get; private set; } = 12f;

    // Flip
    [SerializeField] private float flipCooldown = 0.2f;
    private float lastFlipTime = 0f;

    // 캐싱된 값
    private float sqrDetectionRange;
    private float sqrAttackRange;
    private float sqrMeleeMinDistance;
    private float sqrRangedMinDistance;
    private float sqrSafeAttackDistance;

    private readonly Collider2D[] overlapResults = new Collider2D[10];

    protected bool IsSafeToUpdate => enemyData != null && playerTransform != null && enabled;

    protected virtual void Start()
    {
        InitializeComponents();

        _conflictingAIPath = GetComponent<AIPath>();
        if (_conflictingAIPath != null)
        {
            _conflictingAIPath.canMove = false;
            _conflictingAIPath.enabled = false;
        }

        FindPlayer();

        // Player 레이어 시야에서 제외 (이거 없으면 추적 안 됨!)
        int playerLayerIndex = LayerMask.NameToLayer("Player");
        if (playerLayerIndex != -1)
        {
            obstacleLayer &= ~(1 << playerLayerIndex);
        }

        ValidateEnemyData();

        if (IsSafeToUpdate)
        {
            CacheValues();
            SetupFOVVisualizer();
            InvokeRepeating(nameof(UpdatePath), 0f, enemyData.pathUpdateInterval);
        }

        currentHealth = enemyData.maxHealth;
        lastAttackTime = -enemyData.attackCooldown;

        // 패트롤 초기화
        lastPatrolChangeTime = Time.time;
        patrolDirection = Random.insideUnitCircle.normalized;
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

        float sqrDistToPlayer = Vector2.SqrMagnitude((Vector2)playerTransform.position - (Vector2)transform.position);

        if (sqrDistToPlayer < sqrAttackRange)
        {
            HandleAttackRange(sqrDistToPlayer);
        }
        else if (IsPlayerInSight() && sqrDistToPlayer < sqrDetectionRange)
        {
            HandleChaseRange();
        }
        else
        {
            Patrol();
        }

        ApplyMovement();
        UpdateFacingAndFlip();
        UpdateFOVVisualizer();
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();

        rb.gravityScale = 0f;
        rb.linearDamping = 8f;
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

        sqrMeleeMinDistance = Mathf.Pow(Mathf.Max(0.3f, enemyData.attackRange - 0.4f), 2);
        sqrRangedMinDistance = Mathf.Pow(Mathf.Max(1f, enemyData.attackRange * 0.7f), 2);
        sqrSafeAttackDistance = Mathf.Pow(enemyData.attackRange * 1.2f, 2);
    }

    private void ValidateEnemyData()
    {
        if (enemyData == null)
        {
            Debug.LogError($"{name}: EnemyData 없음!", this);
            enabled = false;
        }
    }

    private bool IsPlayerInSight()
    {
        if (playerTransform == null) return false;

        Vector2 toPlayer = playerTransform.position - transform.position;
        float dist = toPlayer.magnitude;

        if (dist > enemyData.detectionRange) return false;

        toPlayer.Normalize();
        if (Vector2.Angle(facingDirection, toPlayer) > fieldOfViewAngle / 2f) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, dist, obstacleLayer);
        return hit.collider == null;
    }

    private void HandleAttackRange(float sqrDist)
    {
        DisablePathfinding();
        Idle();

        float minDist = isRangedEnemy ? sqrRangedMinDistance : sqrMeleeMinDistance;
        if (sqrDist < minDist)
        {
            BackAwayFromPlayer();
            return;
        }

        if (Time.time - lastAttackTime < enemyData.attackCooldown || IsAnimatorPlaying("Attack"))
            return;

        Attack();
    }

    private void HandleChaseRange()
    {
        isChasing = true;
        EnablePathfinding();
        MoveTowardPlayer();
    }

    // 진짜 핵심: 이 Patrol()이 문제였음 → 이제 완벽하게 고침
    private void Patrol()
    {
        DisablePathfinding();
        isChasing = false;

        // 1. 벽 감지 → 방향만 바꾸고 타이머는 그대로
        Vector2 rayOrigin = (Vector2)transform.position + patrolDirection * 0.6f;
        if (Physics2D.Raycast(rayOrigin, patrolDirection, patrolRayDistance, obstacleLayer))
        {
            ChangePatrolDirection();
            return;
        }

        // 2. 일정 시간마다 자연스럽게 방향 전환
        if (Time.time - lastPatrolChangeTime >= patrolDuration)
        {
            ChangePatrolDirection();
            lastPatrolChangeTime = Time.time;  // 여기서만 갱신
            return;
        }

        // 3. 계속 걷기 (절대 멈추지 않음)
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
        seeker.StartPath(transform.position, playerTransform.position, OnPathComplete);
    }

    private void DisablePathfinding()
    {
        isPathfindingActive = false;
    }

    private void UpdatePath()
    {
        if (!isPathfindingActive || pathPending || seeker == null) return;
        if (!IsPlayerInSight()) return;

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
        if (path == null || path.vectorPath.Count == 0)
        {
            moveDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            moveDirection = Vector2.zero;
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
        animator?.SetBool("isMoving", moving);

        float speed = isChasing ? enemyData.moveSpeed : patrolSpeed;
        rb.linearVelocity = moving ? moveDirection * speed : Vector2.zero;
    }

    private void UpdateFacingAndFlip()
    {
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            facingDirection = moveDirection;
            if (Time.time - lastFlipTime > flipCooldown)
            {
                bool faceRight = moveDirection.x > 0;
                if (isFacingRight != faceRight)
                {
                    isFacingRight = faceRight;
                    spriteRenderer.flipX = !faceRight;
                    lastFlipTime = Time.time;
                }
            }
        }
    }

    private void BackAwayFromPlayer()
    {
        moveDirection = ((Vector2)transform.position - (Vector2)playerTransform.position).normalized;
    }

    protected virtual void Idle() => moveDirection = Vector2.zero;

    private bool IsAnimatorPlaying(string name)
    {
        return animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName(name) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
    }

    protected virtual void Attack()
    {
        if (Time.time - lastAttackTime < enemyData.attackCooldown) return;
        lastAttackTime = Time.time;
        animator?.SetTrigger("Attack");
        PerformAttack();
    }

    protected virtual void PerformAttack()
    {
        if (isRangedEnemy && projectilePrefab != null)
        {
            Vector2 dir = (playerTransform.position - transform.position).normalized;
            var proj = Instantiate(projectilePrefab, transform.position + (Vector3)dir * 0.5f, Quaternion.identity);
            proj.GetComponent<Projectile>()?.Launch(dir, enemyData.attackDamage, projectileSpeed);
        }
        else
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, enemyData.attackRange, LayerMask.GetMask("Player"));
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player") && playerHealth != null)
                {
                    playerHealth.OnDamaged(enemyData.attackDamage);
                }
            }
        }
    }

    // 이거 없으면 Boar, Bomb, Skeleton, Slime 다 에러남
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
        animator?.SetTrigger("Hurt");
        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        animator?.SetTrigger("Die");
        DisablePathfinding();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        enabled = false;
        Destroy(gameObject, 2f);
    }

    private void SetupFOVVisualizer()
    {
        if (!showFieldOfViewInGame) return;
        var obj = new GameObject("FOV");
        obj.transform.SetParent(transform, false);
        lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 21;
        lineRenderer.startWidth = lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0f, 1f, 1f, 0.4f);
        lineRenderer.endColor = new Color(0f, 1f, 1f, 0f);
        lineRenderer.loop = true;
    }

    private void UpdateFOVVisualizer()
    {
        if (!lineRenderer) return;
        float half = fieldOfViewAngle / 2f;
        for (int i = 0; i < 21; i++)
        {
            float angle = Mathf.Lerp(-half, half, i / 20f) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (enemyData?.detectionRange ?? 5f);
            lineRenderer.SetPosition(i, transform.position + transform.TransformDirection(dir));
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);
    }

    protected Transform GetPlayerTransform() => playerTransform;
    protected Animator GetAnimator() => animator;
    protected EnemyData GetEnemyData() => enemyData;
}