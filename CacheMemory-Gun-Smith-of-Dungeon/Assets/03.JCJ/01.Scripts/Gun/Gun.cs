using System;
using System.Collections;
using _01.KBG._01.Scripts.View;
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
    [SerializeField] private AgentStaminaSO agentStamina;

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
        BulletItem bullet = Inventory.Instance.GetItem(typeof(BulletItem)) as BulletItem;
        if (!bullet) yield break;
        yield return new WaitForSeconds(_gunManager.defaultData.reloadTime);
        Debug.Log("reload");
        
        if(Inventory.Instance.RemoveItem(bullet))
            Debug.Log("remove bullet");
        _isReloading = false;
    }

    private float currentAttackDeleyTime = 0;
    private bool isAttacking = false;
    private void Update()
    {
        agentStamina.
        if (Time.time - currentAttackDeleyTime > 1 / (_gunManager.defaultData.fireRate / 60) && !_isReloading && isAttacking)
        {
            currentAttackDeleyTime = Time.time;
            var dataApplier = KBG.Item.GunDataApplier.Instance;
            float recoil = Mathf.Lerp(_gunManager.defaultData.minRebound, _gunManager.defaultData.minRebound, _gunManager.gunStatusData.recoilControl/100);
                cursor.AddRecoil(new Vector2(Random.Range(-recoil,recoil),Random.Range(-recoil,recoil)));
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(cursor.transform.position);
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
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(cursor.transform.position);
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