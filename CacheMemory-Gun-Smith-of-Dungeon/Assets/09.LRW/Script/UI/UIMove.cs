using System;
using DG.Tweening;
using UnityEngine;

namespace _09.LRW.Script.UI
{
    public class UIMove : MonoBehaviour
    {
        private RectTransform _rectTransform;
        [SerializeField] private DoMoveData showData;
        [SerializeField] private DoMoveData hideData;
        [SerializeField] private bool hideStart = false;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            showData.pos += (Vector2)transform.position;
            hideData.pos += (Vector2)transform.position;
            if (hideStart)
            {
                _rectTransform.position =  hideData.pos;
            }
            else
            {
                _rectTransform.position = showData.pos;
            }
        }
        [ContextMenu("Hide")]
        public void Hide()
        {
            Move(hideData);
        }
        [ContextMenu("Show")]
        public void Show()
        {
            Move(showData);
        }
        
        private void Move(DoMoveData data)
        {
            _rectTransform.DOKill();
            _rectTransform.DOMove(data.pos,data.time).SetEase(data.ease);
        }
    }
    [Serializable]
    public class DoMoveData
    {
        public DoMoveData(Vector2 pos2, Ease easeType, float duration)
        {
            pos =  pos2;
            ease = easeType;
            time = duration;
        }
        public Vector2 pos;
        public Ease ease;
        public float time;
    }
}