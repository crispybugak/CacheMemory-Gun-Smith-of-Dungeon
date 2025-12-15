using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace JJM.Script
{
    public class YesNoUIMouseAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _animTime = 0.2f;
        [SerializeField] private Ease _ease = Ease.OutSine;
        [SerializeField] private float _upSize = 1.2f;
        private Vector3 _saveScale;

        private void Awake()
        {
            _saveScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(_saveScale * _upSize, _animTime).SetEase(_ease);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(_saveScale, _animTime).SetEase(_ease);

        }
    }
}
