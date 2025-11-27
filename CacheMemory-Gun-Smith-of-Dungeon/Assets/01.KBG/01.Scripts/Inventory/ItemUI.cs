using System;
using UnityEngine;
using Item;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    public enum ItemType
    {
        Ingredient,
        Part,
        Bullet
    }
    
    [RequireComponent(typeof(Image), typeof(CanvasGroup))]
    public class ItemUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public ItemType itemType;
        
        [Header("Item Data")]
        public IngredientParent ingredientParent;
        public PartSO part;
        public BulletDataSO bullet;
        
        public DropableUI currentSlot { get; private set; }
        private Vector2 _lastPos;
        private CanvasGroup _canvasGroup;
        
        private Image _image;

        private void Awake()
        {
            _image =  GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            switch (itemType)
            {
                case ItemType.Ingredient:
                    _image.sprite = ingredientParent.icon;
                    break;
                case ItemType.Part:
                    _image.sprite = part.icon;
                    break;
                case ItemType.Bullet:
                    _image.sprite = bullet.icon;
                    break;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _lastPos = transform.position;
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position =  _lastPos;
            _canvasGroup.blocksRaycasts = true;
        }

        public void SuccessDrop(DropableUI drop)
        {
            if (currentSlot)
                currentSlot.IsFull = false;
            
            currentSlot = drop;
            currentSlot.IsFull = true;
            _lastPos = currentSlot.transform.position;
        }
    }
}

