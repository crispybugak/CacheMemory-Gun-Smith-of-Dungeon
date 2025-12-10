using System;
using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public abstract class BaseEnemy : MonoBehaviour
{
    public static event Action OnAnyEnemyDied;
    [SerializeField] private EnemyData enemyData;
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private Health playerHealth;
    private Seeker seeker;
    private AIPath aiPath;
    
    private int currentHealth;
    private float lastAttackTime = -999f;
    private float lastPursuitTime = -999f;
    private bool isFacingRight = true;
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 facingDirection = Vector2.right;
    private Path path;
    private int currentWaypoint;
    private bool pathPending;
    
    private float lastFlipTime = 0f;
    [SerializeField] private float flipCooldown = 0.1f;
    
    private bool isPlayerInSight = false;
    [SerializeField] private float fieldOfViewAngle = 120f;
    [SerializeField] private bool showFieldOfView = true;
    [SerializeField] private LayerMask obstacleLayer;
    
    [SerializeField] private bool isRangedEnemy = false;
    
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    
    [SerializeField] private float postAttackCooldown = 0.5f;
    
    private Vector2 patrolDirection = Vector2.right;
    private float lastPatrolChangeTime = 0f;
    [SerializeField] private float patrolDuration = 3f;
    [SerializeField] private float patrolRayDistance = 2f;
    
    [SerializeField] private float rangedEnemyExtraDistance = 0.5f;
    
    private float sqrDetectionRange;
    private float sqrAttackRange;
    private float sqrStoppingDistance;
    private float sqrMinimumDistance;
    private float sqrRangedMinimumDistance;
    
    private bool isAttacking = false;
    private bool isPathfindingActive = false;
    
    protected virtual void OnEnable()
    {
        if (seeker != null)
            seeker.pathCallback += OnPathComplete;
    }
    
    protected virtual void OnDisable()
    {
        if (seeker != null)
            seeker.pathCallback -= OnPathComplete;
    }
    
    protected virtual void Start()
    {
        ValidateEnemyData();
        InitializeComponents();
        CacheValues();
        FindPlayer();
        
        currentHealth = enemyData.maxHealth;
        lastAttackTime = -enemyData.attackCooldown;
        lastPursuitTime = -postAttackCooldown;
        lastPatrolChangeTime = Time.time;
        
        if (seeker != null && playerTransform != null)
        {
            InvokeRepeating(nameof(UpdatePath), 0f, enemyData.pathUpdateInterval);
        }
        
        InitializePlayerDirection();
    }
    
    private void ValidateEnemyData()
    {
        if (enemyData == null)
        {
            Debug.LogError($"{gameObject.name}: EnemyData가 할당되지 않았습니다!");
            enabled = false;
        }
    }
    
    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        
        if (animator == null) Debug.LogWarning($"{gameObject.name}: Animator 없음");
        if (spriteRenderer == null) Debug.LogWarning($"{gameObject.name}: SpriteRenderer 없음");
        if (rb == null) Debug.LogWarning($"{gameObject.name}: Rigidbody2D 없음");
    }
    
    private void CacheValues()
    {
        sqrDetectionRange = enemyData.detectionRange * enemyData.detectionRange;
        sqrAttackRange = enemyData.attackRange * enemyData.attackRange;
        sqrStoppingDistance = enemyData.stoppingDistance * enemyData.stoppingDistance;
        
        sqrMinimumDistance = enemyData.stoppingDistance * enemyData.stoppingDistance;
        
        float rangedMinDistance = enemyData.stoppingDistance + rangedEnemyExtraDistance;
        sqrRangedMinimumDistance = rangedMinDistance * rangedMinDistance;
    }
    
    private void FindPlayer()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            playerTransform = playerGO.transform;
            playerHealth = playerGO.GetComponent<Health>();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Player를 찾을 수 없습니다!");
        }
    }
    
    private void InitializePlayerDirection()
    {
        if (playerTransform == null || spriteRenderer == null) return;

        float playerX = playerTransform.position.x;
        float myX = transform.position.x;

        if (playerX < myX)
        {
            Flip();
        }
    }
    protected virtual void Update()
    {
        if (playerTransform == null)
        {
            HandlePatrol();
            ApplyMovement();
            return;
        }
    
        float sqrDistanceToPlayer = ((Vector2)transform.position - (Vector2)playerTransform.position).sqrMagnitude;
        float targetMinDistance = isRangedEnemy ? sqrRangedMinimumDistance : sqrMinimumDistance;
        
        if (sqrDistanceToPlayer < sqrAttackRange)
        {
            HandleAttackRange(sqrDistanceToPlayer, targetMinDistance);
        }
        else if (IsPlayerInSight() && sqrDistanceToPlayer < sqrDetectionRange)
        {
            HandlePursuitRange(sqrDistanceToPlayer);
        }
        else
        {
            HandlePatrol();
        }
    
        UpdateDynamicFacing();
        ApplyMovement();
    }
    private void HandleAttackRange(float sqrDistanceToPlayer, float targetMinDistance)
    {
        DisablePathfinding();
        
        if (sqrDistanceToPlayer < targetMinDistance)
        {
            BackAwayFromPlayer();
            return;
        }
        
        if (Time.time - lastAttackTime < enemyData.attackCooldown)
        {
            Idle();
            return;
        }
        
        if (IsAnimatorPlaying("Attack"))
        {
            Idle();
            return;
        }
        
        Idle();
        Attack();
    }
    
    private void HandlePursuitRange(float sqrDistanceToPlayer)
    {
        if (isRangedEnemy)
        {
            float safeDistance = sqrAttackRange * 1.2f;  // 공격 범위의 1.2배
            
            if (sqrDistanceToPlayer < sqrAttackRange)
            {
                // 공격 거리 내 - 멈추고 공격
                DisablePathfinding();
                StopMovement();
                Idle();
                if (!IsAnimatorPlaying("Attack") && Time.time - lastAttackTime >= enemyData.attackCooldown)
                {
                    Attack();
                }
            }
            else if (sqrDistanceToPlayer < safeDistance)
            {
                DisablePathfinding();
                StopMovement();
                Idle();
                
                var aiDestSetter = GetComponent<AIDestinationSetter>();
                if (aiDestSetter != null)
                {
                    aiDestSetter.target = null;
                }
            }
            else
            {
                // 안전 거리 밖 - 추격
                EnablePathfinding();
                MoveTowardPlayer();
            }
        }
        else
        {
            // ⭐ 근거리 적: 공격 거리까지 추격
            EnablePathfinding();
            MoveTowardPlayer();
        }
    }
    
    private void HandlePatrol()
    {
        DisablePathfinding();
        Patrol();
    }
    private void EnablePathfinding()
    {
        if (isPathfindingActive) return;
        
        isPathfindingActive = true;
        path = null;
        currentWaypoint = 0;
        
        if (seeker != null && playerTransform != null)
        {
            pathPending = true;
            seeker.StartPath(transform.position, playerTransform.position, OnPathComplete);
        }
    }
    
    private void DisablePathfinding()
    {
        if (!isPathfindingActive) return;
        
        isPathfindingActive = false;
        path = null;
        currentWaypoint = 0;
        pathPending = false;
    }
    private void UpdateDynamicFacing()
    {
        if (playerTransform == null) return;
        
        Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        facingDirection = directionToPlayer;
        
        if (Time.time - lastFlipTime >= flipCooldown)
        {
            if (directionToPlayer.x > 0.1f && isFacingRight == false)
            {
                Flip();
                lastFlipTime = Time.time;
            }
            else if (directionToPlayer.x < -0.1f && isFacingRight == true)
            {
                Flip();
                lastFlipTime = Time.time;
            }
        }
    }
    private void BackAwayFromPlayer()
    {
        Vector2 directionAwayFromPlayer = ((Vector2)transform.position - (Vector2)playerTransform.position).normalized;
        moveDirection = directionAwayFromPlayer;
        
        if (animator != null)
            animator.SetBool("isMoving", true);
    }

    private bool IsAnimatorPlaying(string stateName)
    {
        if (animator == null) return false;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(stateName) && stateInfo.normalizedTime < 1.0f;
    }
    
    private bool IsPlayerInSight()
    {
        if (playerTransform == null) return false;

        Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        Vector2 currentFacingDirection = facingDirection.normalized;
        
        float angleToPlayer = Vector2.Angle(currentFacingDirection, directionToPlayer);
        if (angleToPlayer > fieldOfViewAngle / 2f)
        {
            return false;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);
        if (hit.collider != null)
        {
            return false;
        }
        
        return true;
    }
    
    private void UpdatePath()
    {
        if (pathPending || seeker == null || playerTransform == null)
            return;
        
        if (!isPathfindingActive || !IsPlayerInSight())
            return;
        
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
    
    protected virtual void MoveTowardPlayer()
    {
        if (!isPathfindingActive || path == null)
        {
            moveDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        }
        else
        {
            if (currentWaypoint < path.vectorPath.Count)
            {
                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
                
                if (Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]) < 0.1f)
                {
                    currentWaypoint++;
                }
                
                moveDirection = direction;
                
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f);
                if (hit.collider != null && hit.collider.CompareTag("Enemy") && hit.collider.gameObject != gameObject)
                {
                    moveDirection = new Vector2(direction.y, -direction.x).normalized;
                }
            }
        }
        
        if (animator != null)
            animator.SetBool("isMoving", true);
    }
    
    private void Patrol()
    {
        if (Time.time - lastPatrolChangeTime >= patrolDuration)
        {
            lastPatrolChangeTime = Time.time;
            
            bool wallOnRight = Physics2D.Raycast(transform.position, Vector2.right, patrolRayDistance, obstacleLayer).collider != null;
            bool wallOnLeft = Physics2D.Raycast(transform.position, Vector2.left, patrolRayDistance, obstacleLayer).collider != null;
            bool wallOnUp = Physics2D.Raycast(transform.position, Vector2.up, patrolRayDistance, obstacleLayer).collider != null;
            bool wallOnDown = Physics2D.Raycast(transform.position, Vector2.down, patrolRayDistance, obstacleLayer).collider != null;
            
            List<Vector2> openDirections = new List<Vector2>();
            
            if (!wallOnRight) openDirections.Add(Vector2.right);
            if (!wallOnLeft) openDirections.Add(Vector2.left);
            if (!wallOnUp) openDirections.Add(Vector2.up);
            if (!wallOnDown) openDirections.Add(Vector2.down);
            
            if (openDirections.Count > 0)
            {
                patrolDirection = openDirections[Random.Range(0, openDirections.Count)];
            }
            else
            {
                moveDirection = Vector2.zero;
                if (animator != null)
                    animator.SetBool("isMoving", false);
                return;
            }
        }
        
        bool wallAhead = Physics2D.Raycast(transform.position, patrolDirection, patrolRayDistance, obstacleLayer).collider != null;
        
        if (wallAhead)
        {
            List<Vector2> openDirections = new List<Vector2>();
            
            if (!Physics2D.Raycast(transform.position, Vector2.right, patrolRayDistance, obstacleLayer).collider != null)
                openDirections.Add(Vector2.right);
            if (!Physics2D.Raycast(transform.position, Vector2.left, patrolRayDistance, obstacleLayer).collider != null)
                openDirections.Add(Vector2.left);
            if (!Physics2D.Raycast(transform.position, Vector2.up, patrolRayDistance, obstacleLayer).collider != null)
                openDirections.Add(Vector2.up);
            if (!Physics2D.Raycast(transform.position, Vector2.down, patrolRayDistance, obstacleLayer).collider != null)
                openDirections.Add(Vector2.down);
            
            if (openDirections.Count > 0)
            {
                patrolDirection = openDirections[Random.Range(0, openDirections.Count)];
                moveDirection = patrolDirection;
                if (animator != null)
                    animator.SetBool("isMoving", true);
            }
            else
            {
                moveDirection = Vector2.zero;
                if (animator != null)
                    animator.SetBool("isMoving", false);
            }
        }
        else
        {
            moveDirection = patrolDirection;
            if (animator != null)
                animator.SetBool("isMoving", true);
        }
    }
    
    protected virtual void Idle()
    {
        moveDirection = Vector2.zero;
        if (animator != null)
            animator.SetBool("isMoving", false);
    }
    
    protected virtual void Attack()
    {
        if (Time.time - lastAttackTime < enemyData.attackCooldown)
            return;
        
        lastAttackTime = Time.time;
        lastPursuitTime = Time.time;
        isAttacking = true;
        
        if (animator != null)
            animator.SetBool("isAttacking", true);
        
        PerformAttack();
        StartCoroutine(ResetAttackAnimation());
    }
    
    protected virtual void PerformAttack()
    {
        if (isRangedEnemy)
        {
            if (projectilePrefab == null)
            {
                Debug.LogError($"{gameObject.name}: Projectile Prefab이 할당되지 않았습니다!");
                return;
            }
            
            if (playerTransform == null)
                return;
            
            // 발사체 생성 위치
            Vector2 fireDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            Vector2 spawnPos = (Vector2)transform.position + fireDirection * 0.5f;
            
            // 발사체 생성
            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = fireDirection * projectileSpeed;
            }
            else
            {
                Debug.LogError($"{gameObject.name}: Projectile에 Rigidbody2D가 없습니다!");
            }
            
            // Layer 설정
            if (LayerMask.NameToLayer("Projectile") != -1)
            {
                projectile.layer = LayerMask.NameToLayer("Projectile");
            }
            
            // 발사체에 데미지 정보 전달
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Launch(fireDirection, enemyData.attackDamage);
            }
        }
        else
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, 
                enemyData.attackRange);
            
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    TryDamagePlayer(enemyData.attackDamage);
                }
            }
        }
    }
    
    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
        isAttacking = false;
    }
    
    private void ApplyMovement()
    {
        if (rb == null) return;
        
        if (moveDirection.sqrMagnitude > 0)
        {
            rb.linearVelocity = moveDirection * enemyData.moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        if (spriteRenderer != null)
            spriteRenderer.flipX = !isFacingRight;
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
    
    private void StopMovement()
    {
        if (rb == null) return;
        rb.linearVelocity = Vector2.zero;
        
        if (animator != null)
            animator.SetBool("isMoving", false);
    }
    
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= Mathf.RoundToInt(damage);
        
        if (animator != null)
        {
            animator.SetBool("isHurt", true);
            StartCoroutine(ResetHurtAnimation());
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private IEnumerator ResetHurtAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        if (animator != null)
            animator.SetBool("isHurt", false);
    }
    
    protected virtual void Die()
    {
        if (animator != null)
            animator.SetBool("isDead", true);
        DisablePathfinding();
        OnAnyEnemyDied?.Invoke();
        enabled = false;
        Destroy(gameObject, 1f);
    }
    
    private void OnDrawGizmos()
    {
        if (!showFieldOfView) return;

        Vector2 currentFacingDirection = facingDirection.normalized;
        float halfFOV = fieldOfViewAngle / 2f;
        
        Gizmos.color = Color.blue;
        
        Vector2 leftRay = Quaternion.AngleAxis(halfFOV, Vector3.forward) * currentFacingDirection;
        Vector2 rightRay = Quaternion.AngleAxis(-halfFOV, Vector3.forward) * currentFacingDirection;
        
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftRay * enemyData.detectionRange);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightRay * enemyData.detectionRange);
        
        Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
        for (int i = 0; i < 20; i++)
        {
            float angle1 = -halfFOV + (i / 20f) * fieldOfViewAngle;
            float angle2 = -halfFOV + ((i + 1) / 20f) * fieldOfViewAngle;
            
            Vector2 point1 = Quaternion.AngleAxis(angle1, Vector3.forward) * currentFacingDirection * enemyData.detectionRange;
            Vector2 point2 = Quaternion.AngleAxis(angle2, Vector3.forward) * currentFacingDirection * enemyData.detectionRange;
            
            Gizmos.DrawLine((Vector2)transform.position + point1, (Vector2)transform.position + point2);
        }
        
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + currentFacingDirection * enemyData.detectionRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * patrolRayDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.left * patrolRayDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.up * patrolRayDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.down * patrolRayDistance);
        
        Gizmos.color = Color.yellow;
        DrawCircle(transform.position, enemyData.detectionRange, 20);
        
        Gizmos.color = Color.red;
        DrawCircle(transform.position, enemyData.attackRange, 20);
        
        Gizmos.color = Color.magenta;
        float minDistance = isRangedEnemy 
            ? Mathf.Sqrt(sqrRangedMinimumDistance) 
            : Mathf.Sqrt(sqrMinimumDistance);
        DrawCircle(transform.position, minDistance, 16);
        
        if (isPathfindingActive)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, Vector3.one * 0.3f);
        }
    }
    
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 lastPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
        
    protected Vector2 GetDirection() => moveDirection;
    protected Transform GetPlayerTransform() => playerTransform;
    protected Animator GetAnimator() => animator;
    protected EnemyData GetEnemyData() => enemyData;
}