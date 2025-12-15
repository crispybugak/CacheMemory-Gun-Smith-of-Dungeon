using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _06.SDW._01.Scripts.Item.UI
{
    public class ScrollRectEnhancer : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;

        private void Start()
        {
            // ScrollRect Content 내부의 모든 Text와 Image의 Raycast Target 비활성화
            foreach (var text in scrollRect.content.GetComponentsInChildren<Text>())
            {
                text.raycastTarget = false;
            }

            foreach (var image in scrollRect.content.GetComponentsInChildren<Image>())
            {
                image.raycastTarget = false;
            }

            // ScrollRect에 휠 스크롤 이벤트 추가
            var scrollRectGameObject = scrollRect.gameObject;
            var eventTrigger = scrollRectGameObject.GetComponent<EventTrigger>() ?? scrollRectGameObject.AddComponent<EventTrigger>();

            var scrollEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Scroll
            };
            scrollEntry.callback.AddListener((data) => OnScroll((PointerEventData)data));
            eventTrigger.triggers.Add(scrollEntry);
        }

        private void OnScroll(PointerEventData data)
        {
            // 휠 스크롤 처리
            scrollRect.OnScroll(data);
        }
    }
}