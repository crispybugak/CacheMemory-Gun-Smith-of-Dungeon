using System;
using System.Collections.Generic;
using _06.SDW._01.Scripts.SO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CraftListItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private EventTrigger eventTrigger;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject selectedOutline;

    private CraftingRecipeSO _recipe;
    private Action<CraftingRecipeSO, CraftListItemUI> _onClick;

    private void Reset()
    {
        if (eventTrigger == null) eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null) eventTrigger = gameObject.AddComponent<EventTrigger>();
    }

    public void Bind(CraftingRecipeSO recipe, Action<CraftingRecipeSO, CraftListItemUI> onClick)
    {
        _recipe = recipe;
        _onClick = onClick;

        if (icon != null) icon.sprite = recipe != null ? recipe.ListIcon : null;
        if (nameText != null) nameText.text = recipe != null ? recipe.DisplayName : string.Empty;

        EnsureEventTrigger();
        BindPointerClick();

        SetSelected(false);
    }

    private void EnsureEventTrigger()
    {
        if (eventTrigger == null)
        {
            eventTrigger = GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        if (eventTrigger.triggers == null)
            eventTrigger.triggers = new List<EventTrigger.Entry>();
    }

    private void BindPointerClick()
    {
        // PointerClick만 제거 후 재등록(중복 방지)
        for (int i = eventTrigger.triggers.Count - 1; i >= 0; i--)
        {
            var e = eventTrigger.triggers[i];
            if (e != null && e.eventID == EventTriggerType.PointerClick)
                eventTrigger.triggers.RemoveAt(i);
        }

        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener(_ =>
        {
            if (_recipe != null)
                _onClick?.Invoke(_recipe, this);
        });

        eventTrigger.triggers.Add(entry);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedOutline != null)
            selectedOutline.SetActive(isSelected);
    }
}
