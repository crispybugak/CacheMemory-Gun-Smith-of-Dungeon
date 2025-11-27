using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    private Transform gunTransform;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform gunPos;
    [SerializeField] private int poolCount = 25;
    private GameObject[] bullets;
    [SerializeField] private float fireRate = 0.2f;
    private float lastFireTime = 0f;
    [SerializeField] private SpriteRenderer _spriteRenderer;  
    [SerializeField] private ParticleSystem _particleSystem;
    
    
    public static Action OnFire;

    [SerializeField]private float directionVariance = 5f; // 반동(단위 : 도)
    
    private void Start()
    {
        bullets = new GameObject[poolCount];
        gunTransform = GetComponent<Transform>();
        for (int i = 0; i < poolCount; i++)
        {
            bullets[i] = Instantiate(bulletPrefab);
            bullets[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)&& Time.time > lastFireTime + fireRate)
        {
            ShotBullet();
            lastFireTime = Time.time;
        }
    }

    private void FixedUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 gunPos = gunTransform.position;

        RotateGun(mousePos, gunPos, gunTransform);
    }

    void RotateGun(Vector2 mousePos, Vector2 gunPos, Transform gunTransform)
    {
        Vector2 direction = mousePos - gunPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunTransform.rotation = Quaternion.Euler(0, 0, angle);
        FlipSprite(angle > 90 || angle < -90);
    }

    private void ShotBullet()
    {
        for (int i = 0; i < poolCount; i++)
        {
            if (!bullets[i].activeSelf)
            {
                _particleSystem.Play();
                bullets[i].SetActive(true);
                OnFire?.Invoke();

                bullets[i].transform.position = gunPos.position;

                Vector2 baseDir = gunTransform.right; // 총 기준 오른쪽 방향

                float randomAngle = Random.Range(-directionVariance, directionVariance); //랜덤 각도 생성
                
                Vector2 rotatedDir = RotateVector(baseDir, randomAngle); // 방향 벡터를 randomAngle만큼 회전
                
                float angle = Mathf.Atan2(rotatedDir.y, rotatedDir.x) * Mathf.Rad2Deg; // 총알을 rotatedDir 방향으로 회전시키기
                bullets[i].transform.rotation = Quaternion.Euler(0, 0, angle);

                bullets[i].GetComponent<Bullet>().ResetBullet();
                break;
            }
        }
    }
    
    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
    private void FlipSprite(bool val)
    {
        int flip = val ? -1 : 1;
        transform.localScale = new Vector3(transform.localScale.x, flip * Mathf.Abs(transform.localScale.y), transform.localScale.z);
    }

}