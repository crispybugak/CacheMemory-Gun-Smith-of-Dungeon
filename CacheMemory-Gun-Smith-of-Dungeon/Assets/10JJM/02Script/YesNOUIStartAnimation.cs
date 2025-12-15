using DG.Tweening;
using UnityEngine;

namespace JJM.Script
{
    public class YesNOUIStartAnimation : MonoBehaviour
    {
        [SerializeField] private float _animTime = 0.2f;
        [SerializeField] private Ease _ease = Ease.OutSine;
        private RectTransform _rectTransform;
        private Vector3 _savePos;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _savePos = _rectTransform.position;
            _rectTransform.position -= new Vector3(0, 1500, 0);
        }
        private void Start()
        {
            _rectTransform.DOMoveY(_rectTransform.position.y + 1500, _animTime).SetEase(_ease);
        }

        public void DownMove()
        {
            _rectTransform.DOMoveY(_rectTransform.position.y - 1500, _animTime).SetEase(_ease);
        }
    }
}
