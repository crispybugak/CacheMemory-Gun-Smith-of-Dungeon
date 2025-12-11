using UnityEngine;
using DG.Tweening;

public class CameraZoom : MonoBehaviour
{
    private Camera _mainCamera;

    [SerializeField] private float targetZoomSize = 2f;
    [SerializeField] private float zoomDuration = 1.0f;

    private bool _lockZoomSize = false;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void LateUpdate()
    {
        if (_lockZoomSize && _mainCamera != null)
        {
            _mainCamera.orthographicSize = targetZoomSize;
        }
    }

    public void Zoom(GameObject target)
    {
        if (_mainCamera == null || target == null) return;

        _lockZoomSize = false;

        _mainCamera.transform.DOKill(true);
        _mainCamera.DOKill(true);

        Vector3 targetPosition = target.transform.position;
        targetPosition.z = _mainCamera.transform.position.z;

        _mainCamera.transform
            .DOMove(targetPosition, zoomDuration)
            .SetEase(Ease.OutSine);

        _mainCamera
            .DOOrthoSize(targetZoomSize, zoomDuration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _lockZoomSize = true;
                Debug.Log("줌 완료 & 사이즈 고정");
            });
    }
}
