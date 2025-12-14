using System;
using System.Collections.Generic;
using KBG.Inventory;
using KBG.Item;
using UnityEngine;

namespace _06.SDW._01.Scripts
{
    public class IngredientDebugSpawner : MonoBehaviour
    {
        [Serializable]
        public class IngredientSpawnEntry
        {
            public IngredientType ingredientType;

            [Tooltip("인벤 슬롯에 실제로 들어갈 재료 Item(SO)")]
            public KBG.Item.Item itemSO;

            [Min(1)]
            public int amount = 1;

            [Tooltip("Spawn All에서 제외")]
            public bool disabled;

            [Tooltip("UI 출력용 이름(선택)")]
            public string displayName;
        }

        [Header("Target Inventory (Optional)")]
        [Tooltip("비워두면 Inventory.Instance 사용")]
        [SerializeField] private Inventory targetInventory;

        [Header("Spawn Entries")]
        [SerializeField] private List<IngredientSpawnEntry> entries = new List<IngredientSpawnEntry>();

        [Header("Quick Spawn")]
        [SerializeField] private KBG.Item.Item quickItem;
        [Min(1)]
        [SerializeField] private int quickAmount = 5;

        private Inventory InventoryRef => targetInventory != null ? targetInventory : Inventory.Instance;

        // =========================
        // Context Menu (Inspector ⋮ / 우클릭 메뉴에 뜸)
        // =========================

        [ContextMenu("재료 생성/Quick Spawn")]
        private void CM_SpawnQuick()
        {
            SpawnQuick();
        }

        [ContextMenu("재료 생성/Entries 전부 생성(Enabled)")]
        private void CM_SpawnAllEnabled()
        {
            SpawnAllEnabled();
        }

        [ContextMenu("재료 생성/인벤토리 비우기")]
        private void CM_ClearInventory()
        {
            ClearInventorySlots();
        }

        // =========================
        // Public API
        // =========================

        public void SpawnQuick()
        {
            if (quickItem == null)
            {
                Debug.LogWarning("[IngredientDebugSpawner] Quick Item이 비어있습니다.", this);
                return;
            }

            SpawnItem(quickItem, Mathf.Max(1, quickAmount));
        }

        public void SpawnAllEnabled()
        {
            var inv = InventoryRef;
            if (inv == null)
            {
                Debug.LogError("[IngredientDebugSpawner] Inventory를 찾지 못했습니다. targetInventory를 지정하거나 Inventory.Instance가 있어야 합니다.", this);
                return;
            }

            if (entries == null || entries.Count == 0)
            {
                Debug.LogWarning("[IngredientDebugSpawner] Entries가 비어있습니다.", this);
                return;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                if (e == null || e.disabled) continue;

                if (e.itemSO == null)
                {
                    Debug.LogWarning($"[IngredientDebugSpawner] {i}번째 Entry의 itemSO가 비어있습니다.", this);
                    continue;
                }

                SpawnItem(e.itemSO, Mathf.Max(1, e.amount));
            }
        }

        public void ClearInventorySlots()
        {
            var inv = InventoryRef;
            if (inv == null)
            {
                Debug.LogError("[IngredientDebugSpawner] Inventory를 찾지 못했습니다.", this);
                return;
            }

            if (inv.inventorySlots == null) return;

            for (int i = 0; i < inv.inventorySlots.Count; i++)
            {
                var slot = inv.inventorySlots[i];
                if (slot != null)
                    slot.SetItem(null);
            }

            Debug.Log("[IngredientDebugSpawner] 인벤토리를 비웠습니다.", this);
        }

        // =========================
        // Internal
        // =========================

        private void SpawnItem(KBG.Item.Item item, int amount)
        {
            var inv = InventoryRef;
            if (inv == null)
            {
                Debug.LogError("[IngredientDebugSpawner] Inventory를 찾지 못했습니다.", this);
                return;
            }

            int addCount = Mathf.Max(1, amount);
            int added = 0;

            // Inventory.AddItem(IItem)만 있는 구조 대응: 반복 호출
            for (int i = 0; i < addCount; i++)
            {
                bool ok = inv.AddItem(item);
                if (!ok) break;
                added++;
            }

            if (added < addCount)
                Debug.LogWarning($"[IngredientDebugSpawner] 인벤이 가득 차서 {added}/{addCount}만 추가되었습니다. ({item.name})", this);
            else
                Debug.Log($"[IngredientDebugSpawner] {item.name} x{added} 추가 완료", this);
        }
    }
}
