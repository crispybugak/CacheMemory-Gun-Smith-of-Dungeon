using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _06.SDW._01.Scripts.Item.UI
{
    public class ScrollWheelForwarder : MonoBehaviour, IScrollHandler
    {
        private ScrollRect _parentScrollRect;

        private void OnEnable()
        {
            _parentScrollRect = GetComponentInParent<ScrollRect>();
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (_parentScrollRect != null)
            {
                _parentScrollRect.OnScroll(eventData);
            }
        }
    }
}