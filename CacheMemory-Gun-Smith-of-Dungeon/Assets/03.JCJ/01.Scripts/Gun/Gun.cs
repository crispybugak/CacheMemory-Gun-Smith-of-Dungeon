using System;
using System.Collections;
using KBG.Inventory;
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
    [SerializeField] private int reloadAmountPerBullet;

    private GunDataApplier _gunManager;
    
    private bool _isReloading;
    
    private void Start()
    {
        _gunManager = GunDataApplier.Instance;
    }

    private void OnEnable()
    {
        agentMovement.OnMousePressed += Fire;
        agentMovement.OnMouseReleased += FireStop;
        agentMovement.OnReloadPressed +=  () => { if (!_isReloading) StartCoroutine(Reload()); };
    }

    private void OnDisable()
    {
        agentMovement.OnMousePressed -= Fire;
        agentMovement.OnMouseReleased -= FireStop;
        agentMovement.OnReloadPressed -=  () =>
        {
            if (!_isReloading) StartCoroutine(Reload());
        };
    }

    private IEnumerator Reload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(_gunManager.defaultData.reloadTime);
        Debug.Log("reload");
        BulletItem bullet = Inventory.Instance.GetItem(typeof(BulletItem)) as BulletItem;
        for (int i = 0; i < reloadAmountPerBullet; i++)
            _gunManager.gunStatusData.Reload(bullet);
        if(Inventory.Instance.RemoveItem(bullet))
            Debug.Log("remove bullet");
        _isReloading = false;
    }

    private float currentAttackDeleyTime = 0;
    private bool isAttacking = false;
    private void Update()
    {
        if (Time.time - currentAttackDeleyTime > 1 / (_gunManager.defaultData.fireRate / 60) && !_isReloading && isAttacking)
        {
            currentAttackDeleyTime = Time.time;
            cursor.AddRecoil(new Vector2(0, -100));
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(agentMovement.mouseDir);
            float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) *
                Mathf.Rad2Deg + 180;
            ShotBullet(angle);
            
        }
    }

    private void Fire()
    {
        isAttacking = true;
    }

    private void FireStop()
    {
        isAttacking = false;
    }
    
    private void FixedUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(agentMovement.mouseDir);
        Vector2 gunPos = transform.position;
        
        RotateGun(mousePos, gunPos, transform);
    }

    void RotateGun(Vector2 mousePos, Vector2 gunPos, Transform gunTransform)
    {
        Vector2 direction = (mousePos - gunPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        gunTransform.rotation = Quaternion.Euler(0, 0, angle);
        FlipSprite(angle is > 90 and < 270);
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