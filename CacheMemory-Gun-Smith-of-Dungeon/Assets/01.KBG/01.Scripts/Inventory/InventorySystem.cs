using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        [SerializeField] private Vector2Int inventorySize;
        [SerializeField] private GameObject inventorySlotPrefab;

        private GridLayoutGroup  _gridLayoutGroup;
        
        private void Awake()
        {
            _gridLayoutGroup = GetComponent<GridLayoutGroup>();
            
            _gridLayoutGroup.constraintCount = inventorySize.x;
            for (int i = 0; i < inventorySize.x; i++)
            {
                for (int j = 0; j < inventorySize.y; j++)
                {
                    Instantiate(inventorySlotPrefab, transform);
                }
            }
        }
    }
}
