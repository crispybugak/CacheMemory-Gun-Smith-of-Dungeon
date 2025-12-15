using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KBG.Item;

public class IngredientOptionItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private EventTrigger eventTrigger;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;          // "재료이름 n/n"
    [SerializeField] private GameObject selectedOutline;

    private IngredientType _type;
    private Action<IngredientType> _onClick;

    private void Awake()
    {
        if (eventTrigger == null) eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null) eventTrigger = gameObject.AddComponent<EventTrigger>();
        if (eventTrigger.triggers == null) eventTrigger.triggers = new List<EventTrigger.Entry>();

        // 클릭 받을 배경 Graphic 보장 (레이캐스트 타겟이 있어야 EventTrigger가 먹음)
        var bg = GetComponent<Image>();
        if (bg == null) bg = gameObject.AddComponent<Image>();
        bg.raycastTarget = true;
    }

    public void Bind(
        IngredientType type,
        Sprite matIcon,
        string matName,
        int owned,
        int required,
        Action<IngredientType> onClick)
    {
        gameObject.SetActive(true); // ★ 고정 3슬롯이지만 혹시 꺼져있을 수 있으니 보장

        _type = type;
        _onClick = onClick;

        if (icon != null) icon.sprite = matIcon;
        if (nameText != null) nameText.text = $"{matName} {owned}/{required}";

        BindPointerClick();
        SetSelected(false);
    }

    private void BindPointerClick()
    {
        for (int i = eventTrigger.triggers.Count - 1; i >= 0; i--)
        {
            var e = eventTrigger.triggers[i];
            if (e != null && e.eventID == EventTriggerType.PointerClick)
                eventTrigger.triggers.RemoveAt(i);
        }

        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener(_ => _onClick?.Invoke(_type));
        eventTrigger.triggers.Add(entry);
    }

    public void SetSelected(bool selected)
    {
        if (selectedOutline != null)
            selectedOutline.SetActive(selected);
    }
}