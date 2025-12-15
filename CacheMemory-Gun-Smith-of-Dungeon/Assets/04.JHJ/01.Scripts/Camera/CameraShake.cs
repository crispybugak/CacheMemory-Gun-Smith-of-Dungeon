using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalLocalPos;
    private Tween shakeTween;

    public void ShakeCamera(float duration, float intensity)
    {
        // 기존 흔들림 중이면 정리
        if (shakeTween != null && shakeTween.IsActive())
            shakeTween.Kill(false);

        originalLocalPos = transform.localPosition;

        // DOShakePosition은 내부에서 랜덤 오프셋으로 흔듦
        shakeTween = transform.DOShakePosition(
                duration: duration,
                strength: new Vector3(intensity, intensity, 0f),
                vibrato: 60,          // 흔들림 빈도(InvokeRepeating 0.01f 느낌에 가깝게)
                randomness: 90f,
                snapping: false,
                fadeOut: true
            )
            .SetUpdate(true)          // Time.timeScale 영향 없이(원하면 false로)
            .OnComplete(() =>
            {
                transform.localPosition = originalLocalPos;
            });
    }

    public void StopShaking()
    {
        if (shakeTween != null && shakeTween.IsActive())
            shakeTween.Kill(false);

        transform.localPosition = originalLocalPos;
    }
}
