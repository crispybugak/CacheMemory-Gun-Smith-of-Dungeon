using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalCameraPosition;

    public void ShakeCamera(float duration, float intensity)
    {
        originalCameraPosition = transform.localPosition;//디폴트 위치 저장해주기
        InvokeRepeating("DoShake",0,0.01f);
        Invoke("StopShaking",duration);
    }

    private void DoShake()
    {
        float offSetX = Random.Range(-0.1f, 0.1f);
        float offSetY = Random.Range(-0.1f, 0.1f);
        transform.localPosition = originalCameraPosition + new Vector3(offSetX, offSetY);
    }
    public void StopShaking()
    {
        CancelInvoke("DoShake");
        transform.localPosition = originalCameraPosition;
    }
}
