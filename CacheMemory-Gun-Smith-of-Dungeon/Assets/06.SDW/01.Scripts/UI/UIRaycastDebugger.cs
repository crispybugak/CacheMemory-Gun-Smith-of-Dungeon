using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRaycastDebugger : MonoBehaviour
{
    private readonly List<RaycastResult> _results = new List<RaycastResult>(32);

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current == null)
        {
            Debug.LogWarning("[UIRaycastDebugger] EventSystem.current가 없습니다.");
            return;
        }

        var data = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        _results.Clear();
        EventSystem.current.RaycastAll(data, _results);

        var sb = new StringBuilder(256);
        sb.AppendLine($"[UIRaycastDebugger] Raycast hits: {_results.Count}");
        for (int i = 0; i < _results.Count; i++)
        {
            var r = _results[i];
            sb.AppendLine($"{i}. {r.gameObject.name} (module:{r.module}, depth:{r.depth}, sortingLayer:{r.sortingLayer}, sortingOrder:{r.sortingOrder})");
        }

        Debug.Log(sb.ToString());
    }
}