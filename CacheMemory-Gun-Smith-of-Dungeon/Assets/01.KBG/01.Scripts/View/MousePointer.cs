using UnityEngine;

public class MousePointer : MonoBehaviour
{
    [SerializeField] private AgentMovementSO agentMovement;

    private RectTransform _rectTransform;
    private Camera _cam;

    public Vector2 position
    {
        get => _rectTransform.anchoredPosition;
        set => _rectTransform.anchoredPosition = value;
    }

    private void Awake()
    {
        _cam = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    public void AddRecoil(Vector2 dir)
    {
    }
}
