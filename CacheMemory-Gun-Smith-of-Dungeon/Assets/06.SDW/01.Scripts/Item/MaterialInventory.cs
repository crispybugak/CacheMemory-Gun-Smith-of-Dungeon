using System;
using System.Collections.Generic;
using KBG.Inventory;
using KBG.Item;
using UnityEngine;

namespace _06.SDW._01.Scripts.Item
{
    public class MaterialInventory : MonoBehaviour
    {
        public static MaterialInventory Instance { get; private set; }

        [Header("Source Inventory (Optional)")]
        [Tooltip("비워두면 Inventory.Instance 사용")]
        [SerializeField] private Inventory inventory;

        private readonly Dictionary<IngredientType, int> _counts = new Dictionary<IngredientType, int>();
        private int _suppressHookDepth = 0;

        public event Action OnChanged;

        private Inventory Inv => inventory != null ? inventory : Inventory.Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            RebuildFromSlots();
        }

        // =========================
        // Query
        // =========================
        public int GetCount(IngredientType type)
        {
            return _counts.TryGetValue(type, out var v) ? v : 0;
        }

        public bool CanConsume(IngredientType type, int amount)
        {
            return GetCount(type) >= Mathf.Max(0, amount);
        }

        // =========================
        // Slot Hook (Slot.SetItem에서 old/new로 호출)
        // =========================
        public void NotifySlotItemChanged(IItem oldItem, IItem newItem)
        {
            if (_suppressHookDepth > 0) return;

            // 변경이 일어났으면 안전하게 전체 재집계
            RebuildFromSlots();
        }

        [ContextMenu("재료/슬롯 기준으로 재집계(Rebuild)")]
        public void RebuildFromSlots()
        {
            _counts.Clear();

            var inv = Inv;
            if (inv == null || inv.inventorySlots == null)
            {
                OnChanged?.Invoke();
                return;
            }

            for (int i = 0; i < inv.inventorySlots.Count; i++)
            {
                var slot = inv.inventorySlots[i];
                if (slot == null || slot.item == null) continue;

                if (slot.item.ItemData is Ingredient ing)
                {
                    _counts.TryGetValue(ing.type, out int cur);
                    _counts[ing.type] = cur + 1;
                }
            }

            OnChanged?.Invoke();
        }

        public bool TryConsumeFromSlots(IngredientType type, int amount)
        {
            int need = Mathf.Max(0, amount);
            if (need == 0) return true;

            if (!CanConsume(type, need)) return false;

            var inv = Inv;
            if (inv == null || inv.inventorySlots == null) return false;

            _suppressHookDepth++;
            try
            {
                for (int i = 0; i < inv.inventorySlots.Count && need > 0; i++)
                {
                    var slot = inv.inventorySlots[i];
                    if (slot == null || slot.item == null) continue;

                    if (slot.item.ItemData is Ingredient ing && ing.type == type)
                    {
                        slot.SetItem(null); // 슬롯에서 제거 → 아이템 사라짐
                        need--;
                    }
                }
            }
            finally
            {
                _suppressHookDepth--;
            }

            // suppress 동안 이벤트 막았으니, 마지막에 한 번만 재집계
            RebuildFromSlots();
            return true;
        }
    }
}
