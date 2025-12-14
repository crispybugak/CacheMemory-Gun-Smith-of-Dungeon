using UnityEngine;
using KBG.Item;
using KBG.Inventory;

[RequireComponent(typeof(BaseEnemy))]
public class EnemyDropper : MonoBehaviour
{
    [Header("드롭 설정")] 
    [SerializeField] private Ingredient itemSo;

    private BaseEnemy enemy;

    private void Awake()
    {
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

        var obj = new Item();
        obj.ItemData = itemSo;
        Inventory.Instance.AddItem(obj);

        FloatingTextManager.Instance.ShowFloatingText(
            itemSo.itemName,
            1, 
            dead.transform.position
        );
    }
}