using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void PlayFloatingText(string text, Vector3 screenPos, float duration = 1.2f)
    {
        rectTransform.position = screenPos;
    
        textMesh.text = text;
        canvasGroup.alpha = 1f;
    
        StartCoroutine(FloatAndFadeOut(duration));
    }

    private IEnumerator FloatAndFadeOut(float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = rectTransform.position;
        Vector3 endPos = startPos + Vector3.up * 80f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rectTransform.position = Vector3.Lerp(startPos, endPos, t);
            
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}