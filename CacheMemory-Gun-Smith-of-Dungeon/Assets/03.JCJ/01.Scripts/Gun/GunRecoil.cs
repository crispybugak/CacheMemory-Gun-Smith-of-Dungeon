using System;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public float recoilAmount = 0.1f; // 밀리는 거리
    public float recoilSpeed = 1f; // 밀리고 원복되는 속도

    private Vector3 originalPosition;
    private Vector3 recoilTarget;
    private bool recoiling = false;

    void Start()
    {
        originalPosition = transform.localPosition;
        Gun.OnFire += StartRecoil;
    }

    private void OnDestroy()
    {
        Gun.OnFire -= StartRecoil;
    }

    void Update()
    {   

        if (recoiling)
        {
            // 원래 위치로 부드럽게 복귀
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * recoilSpeed);
            if (Vector3.Distance(transform.localPosition, originalPosition) < 0.01f)
            {
                transform.localPosition = originalPosition;
                recoiling = false;
            }
        }
    }

    void StartRecoil()
    {
        // 총을 반대 방향으로 잠시 밀기
        recoilTarget = originalPosition - transform.right * recoilAmount;
        transform.localPosition = recoilTarget;
        recoiling = true;
    }
}