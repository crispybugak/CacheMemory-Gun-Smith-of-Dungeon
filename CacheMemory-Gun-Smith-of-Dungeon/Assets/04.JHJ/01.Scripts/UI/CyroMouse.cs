using UnityEngine;

public class CyroMouse : MonoBehaviour
{
    [SerializeField] private RectTransform[] cards;   // 오른쪽에 나란히 있는 3개 이미지
    [SerializeField] private float smooth = 10f;      // 부드럽게 회전하는 정도

    // 중앙일 때의 기본 회전값
    [SerializeField] private Vector3 centerEuler = new Vector3(0f, 10f, 1.5f);

    private Vector3 _leftEuler;   // 마우스 왼쪽 끝일 때 회전 (0,0,0)
    private Vector3 _rightEuler;  // 마우스 오른쪽 끝일 때 회전 (0,20,3)
    private void Start()
    {
        // 왼쪽 끝 = 0,0,0
        _leftEuler = Vector3.zero;

        // 오른쪽 끝 = 중앙값의 2배 (0,20,3)
        _rightEuler = centerEuler * 2f;

        // 씬 시작할 때 카드들의 회전을 중앙 기준으로 맞춰주고 싶다면:
        Quaternion centerRot = Quaternion.Euler(centerEuler);
        foreach (var card in cards)
        {
            if (card == null) continue;
            card.localRotation = centerRot;
        }
    }
    private void Update()
    {
        // 0(왼쪽) ~ 1(오른쪽)
        float mouse01 = Mathf.Clamp01(Input.mousePosition.x / Screen.width);

        // 0~1 비율에 따라 왼쪽각 ↔ 오른쪽각 사이에서 보간
        Vector3 targetEuler = Vector3.Lerp(_leftEuler, _rightEuler, mouse01);
        Quaternion targetRot = Quaternion.Euler(targetEuler);

        // 세 카드 모두 같은 회전으로 부드럽게 따라가게 함
        float t = Time.deltaTime * smooth;

        foreach (var card in cards)
        {
            if (card == null) continue;
            card.localRotation = Quaternion.Slerp(
                card.localRotation,
                targetRot,
                t
            );
        }
    }
}
