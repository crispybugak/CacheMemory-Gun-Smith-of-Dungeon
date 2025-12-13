using System;
using DG.Tweening;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    [SerializeField] private AgentMovementSO agentMovement;
    [SerializeField] private float restorationTime;
    private RectTransform _rectTransform;

    private Camera _cam;
    private Vector2 _distance =  Vector2.zero; 
    private Vector2 _previousPosition = Vector2.zero;
    private float time = 0;
    
    
    public Vector2 position
    {
        get => _rectTransform.position;
        set => _rectTransform.position = value;
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
        // time += Time.deltaTime * restorationTime;
        // Vector2 currentDistance;
        // if (time >= 1)
        //     currentDistance = Vector2.Lerp(_previousPosition, _distance, time);
        //
        // position = agentMovement.mouseDir + currentDistance;
    }

    private void LateUpdate()
    {
        Vector2 pos = position;
        Vector2 minLimit = _cam.ViewportToScreenPoint(Vector2.zero);
        Vector2 maxLimit = _cam.ViewportToScreenPoint(Vector2.one);
        pos.x = Mathf.Clamp(pos.x, minLimit.x, maxLimit.x);
        pos.y = Mathf.Clamp(pos.y, minLimit.y, maxLimit.y);
        position = pos;
    }

    public void AddRecoil(Vector2 dir)
    {
        _previousPosition = _distance;
        _distance += dir;
        time = 0;
    }
}
