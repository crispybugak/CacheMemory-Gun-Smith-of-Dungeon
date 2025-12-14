using KBG.Item;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KBG.Inventory
{
    public class GunSlot : Slot
    {
        public PartType slotType;


        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging) return;
            Inventory.Instance.SetParent(gameObject);
            var dropable = Inventory.Instance.RequestDropable(this);
            transform.position =  _startPos;
            if (dropable)
            {
                if (!dropable.item || dropable.item is Part part && part.partData.type == slotType)
                {
                    (dropable.item, item) = (item, dropable.item);
                    SetIcon();
                    dropable.SetIcon();
                }
            }
            isDragging = false;
        }
        
        public override bool RequestCanChangeItem(IItem item)
        {
            if (item is Part part && (GunDataApplier.Instance.gunStatusData.GetPart(part.partData.requirePartType) || part.partData.requirePartType == PartType.None))
                return part.partData.type == slotType;
            return false;
        }

        public override void SetIcon()
        {
            base.SetIcon();
            GunDataApplier.Instance.gunStatusData.ChangePart(item as Part, slotType);
        }
    }
}
