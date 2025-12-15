using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using KBG.Item;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace KBG.Inventory
{
    [DefaultExecutionOrder(-1000)]
    public class Inventory : MonoSingleton<Inventory>
    {

        [Header("Slot Setting")]
        public Vector2Int slotSize;
        public GameObject slotPrefab;
        public List<Slot> slots  =  new List<Slot>();
        public List<Slot> inventorySlots = new List<Slot>();
        
        [FormerlySerializedAs("draggingItem")] [Header("Dragging Item")]
        public GameObject draggingPanel;
        
        private GameObject _startParent;
        private int _lastIndex;

        private GridLayoutGroup _gridLayout;
        
        protected override void Awake()
        {
            base.Awake();
                _gridLayout = GetComponent<GridLayoutGroup>();
            _gridLayout.constraintCount = slotSize.x;
            for (int i = 0; i < slotSize.x; i++)
                for (int j = 0; j < slotSize.y; j++)
                {
                    var obj = Instantiate(slotPrefab, transform).GetComponentInChildren<Slot>();
                    slots.Add(obj);
                    inventorySlots.Add(obj);
                    obj.name = $"Slot({i},{j})";
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
            return slots.FirstOrDefault(slot => RectTransformUtility.RectangleContainsScreenPoint(slot.GetComponent<RectTransform>(), self.transform.position) && slot != self);
        }

        public Slot GetEmptyInventorySlot()
        {
            return inventorySlots.FirstOrDefault(slot => slot.item == null);
        }

        public bool AddItem(IItem item)
        {
            var emptySlot = GetEmptyInventorySlot();
            if  (emptySlot)
            {
                emptySlot.SetItem(item);
                return true;
            }
            return false;
        }
        public bool RemoveItem(IItem item)
        {
            var slot = inventorySlots.FirstOrDefault(slot => slot.item == item);
            if (slot)
            {
                slot.SetItem(null); // SetItem을 통해 카운트/아이콘/UI 갱신까지 같이 처리
                return true;
            }
            return false;
        }

        public IItem GetItem(Type t)
        {
            var item = inventorySlots.FirstOrDefault(i => i.GetType() == t);
            return item?.item;
        }
    }
}