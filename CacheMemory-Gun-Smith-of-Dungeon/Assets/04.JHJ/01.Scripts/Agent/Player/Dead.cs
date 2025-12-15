using DG.Tweening;
using UnityEngine;

public class Dead : MonoBehaviour
{
    public CameraZoom _zoom;
    public Health health;

    private bool _deadOnce;

    public void PlayerDead(GameObject player)
    {
        if (_deadOnce) return;
        _deadOnce = true;

        Sequence sequence = DOTween.Sequence();

        var movement = player.GetComponent<AgentMovement>();
        if (movement != null) movement.enabled = false;

        // 죽을 때 슬로우
        if (GameManager.Instance != null)
            GameManager.Instance.DeadTimeScale();

        // 줌 대상은 player가 자연스러움 (원래 gameObject 넘기던 것 수정)
        if (_zoom != null)
            _zoom.Zoom(player);

        var sr = player.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sequence.Append(
                sr.DOFade(0f, 1.5f)
                  .OnComplete(() => Destroy(player.gameObject))
            );
        }
        else
        {
            sequence.AppendInterval(1.5f).OnComplete(() => Destroy(player.gameObject));
        }
    }
}
