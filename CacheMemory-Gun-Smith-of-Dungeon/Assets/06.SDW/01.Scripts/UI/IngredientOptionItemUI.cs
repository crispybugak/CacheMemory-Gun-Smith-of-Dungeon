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
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject selectedOutline;

    private IngredientType _type;
    private Sprite _iconSprite;
    private Action<IngredientType, Sprite> _onClick;

    private void Awake()
    {
        if (eventTrigger == null) eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null) eventTrigger = gameObject.AddComponent<EventTrigger>();
        if (eventTrigger.triggers == null) eventTrigger.triggers = new List<EventTrigger.Entry>();

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
        Action<IngredientType, Sprite> onClick)
    {
        gameObject.SetActive(true);

        _type = type;
        _iconSprite = matIcon;
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
        entry.callback.AddListener(_ => _onClick?.Invoke(_type, _iconSprite));
        eventTrigger.triggers.Add(entry);
    }

    public void SetSelected(bool selected)
    {
        if (selectedOutline != null)
            selectedOutline.SetActive(selected);
    }
}