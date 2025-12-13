using System;
using System.Collections;
using KBG.Item;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform gunPos;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private AgentMovementSO agentMovement;
    [SerializeField] private MousePointer cursor;

    private GunDataApplier _gunManager;
    
    private bool _isReloading;
    
    private void Start()
    {
        _gunManager = GunDataApplier.Instance;
    }

    private void OnEnable()
    {
        agentMovement.OnMousePressed += () => StartCoroutine(StartFire());
        agentMovement.OnMouseReleased += () => StopCoroutine(StartFire());
        agentMovement.OnReloadPressed +=  Reload;
    }

    private void OnDisable()
    {
        agentMovement.OnMousePressed -= () => StartCoroutine(StartFire());
        agentMovement.OnMouseReleased -= () => StopCoroutine(StartFire());
        agentMovement.OnReloadPressed -=  Reload;
    }

    private void Reload()
    {
        _isReloading = true;
    }
    private IEnumerator StartFire()
    {
        if (_isReloading) yield break;
        cursor.AddRecoil(new Vector2(0, -100));
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(agentMovement.mouseDir);
        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) *
            Mathf.Rad2Deg + 180;
        ShotBullet(angle);
        yield return new WaitForSeconds(1 / (_gunManager.defaultData.fireRate / 60));
        StartCoroutine(StartFire());
    }
    
    private void FixedUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(cursor.position);
        Vector2 gunPos = transform.position;
        
        RotateGun(mousePos, gunPos, transform);
    }

    void RotateGun(Vector2 mousePos, Vector2 gunPos, Transform gunTransform)
    {
        Vector2 direction = mousePos - gunPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        gunTransform.rotation = Quaternion.Euler(0, 0, angle);
        FlipSprite(angle > 90 || angle < -90);
    }

    private void ShotBullet(float dir)
    {
        float accuracy = Mathf.Lerp(_gunManager.defaultData.maxSpread, _gunManager.defaultData.minSpread, _gunManager.gunStatusData.accuracy/100);
        dir += Random.Range(-accuracy, accuracy);
        BulletItem bullet = _gunManager.gunStatusData.ShootBullet();
        Instantiate(bullet.bulletData.BulletPrefab, transform.position, Quaternion.Euler(0, 0, dir));
    }
    
    private void FlipSprite(bool val)
    {
        int flip = val ? -1 : 1;
        transform.localScale = new Vector3(transform.localScale.x, flip * Mathf.Abs(transform.localScale.y), transform.localScale.z);
    }

}