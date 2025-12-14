using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using KBG.Item;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace KBG.Inventory
{
    public class Inventory : MonoSingleton<Inventory>
    {
        [Header("Slot Setting")]
        public Vector2Int slotSize;
        public GameObject slotPrefab;
        public List<Slot> slots = new List<Slot>();
        public List<Slot> inventorySlots = new List<Slot>();

        [FormerlySerializedAs("draggingItem")]
        [Header("Dragging Item")]
        public GameObject draggingPanel;

        private GameObject _startParent;
        private int _lastIndex;

        private GridLayoutGroup _gridLayout;

        protected override void Awake()
        {
            base.Awake();

            _gridLayout = GetComponent<GridLayoutGroup>();
            _gridLayout.constraintCount = slotSize.x;

            slots.Clear();
            inventorySlots.Clear();

            for (int i = 0; i < slotSize.x; i++)
            {
                for (int j = 0; j < slotSize.y; j++)
                {
                    var slot = Instantiate(slotPrefab, transform).GetComponentInChildren<Slot>();
                    if (slot == null)
                    {
                        Debug.LogError("[Inventory] slotPrefab 안에 Slot(InventorySlot 등) 컴포넌트가 없습니다.", this);
                        continue;
                    }

                    slot.name = $"Slot({i},{j})";
                    slots.Add(slot);
                    inventorySlots.Add(slot);
                }
            }
        }

        public void SetParent(GameObject child)
        {
            _gridLayout.enabled = false;

            if (_startParent)
            {
                child.transform.SetParent(_startParent.transform);
                child.transform.SetSiblingIndex(_lastIndex);
                _startParent = null;

                _gridLayout.enabled = true;
            }
            else
            {
                _lastIndex = child.transform.GetSiblingIndex();
                _startParent = child.transform.parent.gameObject;
                child.transform.SetParent(draggingPanel.transform);
            }
        }

        public Slot RequestDropable(Slot self)
        {
            return slots.FirstOrDefault(slot =>
                slot != null &&
                RectTransformUtility.RectangleContainsScreenPoint(slot.GetComponent<RectTransform>(), self.transform.position) &&
                slot != self
            );
        }

        // 진짜 "빈 슬롯"은 slot.item == null
        public Slot GetEmptyInventorySlot()
        {
            return inventorySlots.FirstOrDefault(slot => slot != null && slot.item == null);
        }

        public bool AddItem(IItem item)
        {
            var emptySlot = GetEmptyInventorySlot();
            if (emptySlot != null)
            {
                emptySlot.SetItem(item);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 인벤에서 특정 IItem 타입(TItem) 하나를 찾습니다. (예: BulletItem)
        /// </summary>
        public bool TryGetFirstItem<TItem>(out TItem item) where TItem : IItem
        {
            item = null;

            if (inventorySlots == null) return false;

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                var slot = inventorySlots[i];
                if (slot == null || slot.item == null) continue;

                if (slot.item is TItem found)
                {
                    item = found;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 인벤에서 특정 IItem 타입(TItem) 1개를 제거합니다(슬롯 1칸 비움).
        /// </summary>
        public bool TryRemoveFirstItem<TItem>() where TItem : IItem
        {
            if (inventorySlots == null) return false;

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                var slot = inventorySlots[i];
                if (slot == null || slot.item == null) continue;

                if (slot.item is TItem)
                {
                    slot.SetItem(null);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 네가 쓰던 기존 스타일 호환: Inventory.GetItem(typeof(BulletItem)) 지원
        /// </summary>
        public IItem GetItem(Type type)
        {
            if (type == null || inventorySlots == null) return null;

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                var slot = inventorySlots[i];
                if (slot == null || slot.item == null) continue;

                if (type.IsInstanceOfType(slot.item))
                    return slot.item;
            }

            return null;
        }
    }
}
