using System.Collections.Generic;
using UnityEngine;
using KBG.Item;
using KBG.Inventory;

[RequireComponent(typeof(BaseEnemy))]
public class EnemyDropper : MonoBehaviour
{
    [Header("드롭 설정")] 
    [SerializeField] private Ingredient[]  itemSo;

    private BaseEnemy enemy;

    private void Awake()
    {
        var droppers = GetComponents<EnemyDropper>();
        if (droppers.Length > 1)
        {
            Debug.LogWarning($"[EnemyDropper] 중복 감지 {name}에 EnemyDropper가 여러 개 있습니다. 하나만 남기고 나머지 제거.", this);
            Destroy(this);
            return;
        }

        enemy = GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.OnDeath += HandleEnemyDeath;
        }
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnDeath -= HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(BaseEnemy dead)
    {
        if (itemSo == null) return;
        int i = Random.Range(0, itemSo.Length);
        var obj = new Item();
        obj.ItemData = itemSo[i];
        Inventory.Instance.AddItem(obj);

        FloatingTextManager.Instance.ShowFloatingText(
            itemSo[i].itemName,
            1, 
            dead.transform.position
        );
    }
}