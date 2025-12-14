using System;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    [SerializeField] private AgentMovementSO agentMovement;
    [Header("Recoil Settings")]
    public float recoilSpeed;
    public float recoverySpeed;

    private RectTransform _rectTransform;
    private Camera _cam;

    private Vector2 _dir;
                                        

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

    private void FixedUpdate()
    {
        
    }

    public void AddRecoil(Vector2 dir)
    {
        
    }
}