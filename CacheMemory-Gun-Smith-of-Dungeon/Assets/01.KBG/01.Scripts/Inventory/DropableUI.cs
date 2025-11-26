using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
    public class DropableUI : MonoBehaviour,  IDropHandler
    {
        [field: SerializeField] public bool IsFull {get; set;}
        public void OnDrop(PointerEventData eventData)
        {
            var dragged =  eventData.pointerDrag;
        
            if (dragged.TryGetComponent(out ItemUI itemUI)&& !IsFull)
                itemUI.SuccessDrop(this);
        }
    }
}