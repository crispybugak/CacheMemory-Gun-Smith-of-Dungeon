using KBG.Item;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KBG.Inventory
{
    
    public class GunTooltip : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
    {
        public void OnPointerMove(PointerEventData eventData)
        {
            Tooltip.Instance.OpenTooltip(GunDataApplier.Instance.gunStatusData, eventData.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.Instance.gameObject.SetActive(false);
        }
    }
}