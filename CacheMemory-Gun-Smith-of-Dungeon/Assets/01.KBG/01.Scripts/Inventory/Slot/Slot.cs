using System;
using KBG.Item;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KBG.Inventory
{
    public abstract class Slot : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public IItem item;
        public Sprite defaultSprite;

        protected bool isDragging;
        
        protected Vector2 _startPos;
        
        protected Image _image;

        private void OnEnable()
        {
            _image = transform.GetChild(0).GetComponent<Image>();
            
            SetIcon();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (item)
            {
                Inventory.Instance.SetParent(gameObject);
                isDragging = true;
            }
            _startPos = transform.position;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
                transform.position = eventData.position;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging) return;
            Inventory.Instance.SetParent(gameObject);
            var dropable = Inventory.Instance.RequestDropable(this);
            transform.position =  _startPos;
            if (dropable)
            {
                if (dropable.RequestCanChangeItem(item))
                {
                    (dropable.item, item) = (item, dropable.item);
                    SetIcon();
                    dropable.SetIcon();
                }
            }
            isDragging = false;
        }

        public virtual bool RequestCanChangeItem(IItem item)
        {
            return true;
        }
        
        public virtual void SetIcon()
        {
            _image.sprite = item? item.ItemData.icon: defaultSprite;
            _image.SetNativeSize();
            var rect = _image.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one * (item ? item.ItemData.upScaling : 1);
        }
    }
}

