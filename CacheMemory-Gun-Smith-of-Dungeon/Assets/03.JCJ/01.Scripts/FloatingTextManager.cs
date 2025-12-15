using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingTextManager : MonoSingleton<FloatingTextManager>
{
    [SerializeField] private GameObject floatingTextPrefab;
    private Transform spawnParent;

    protected override void Awake()
    {
        base.Awake();
        spawnParent = transform.parent;
    }

    public void ShowFloatingText(string itemName, int count, Vector3 worldPos)
    {
        if (floatingTextPrefab == null || spawnParent == null) return;

        GameObject floatingTextObj = Instantiate(floatingTextPrefab, spawnParent);
        FloatingText floatingText = floatingTextObj.GetComponent<FloatingText>();

        if (floatingText != null)
        {
            string displayText = $"<color=yellow>+{count}x {itemName}</color>";
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            floatingText.PlayFloatingText(displayText, screenPos, duration: 1.2f);
        }
    }
}