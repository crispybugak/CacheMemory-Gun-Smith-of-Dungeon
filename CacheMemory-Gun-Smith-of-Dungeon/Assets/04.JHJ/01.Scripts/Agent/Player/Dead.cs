using DG.Tweening;
using UnityEngine;

public class Dead : MonoBehaviour
{
    public CameraZoom _zoom;
    public Health health;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }
    public void PlayerDead(GameObject player)
    {
        Sequence sequence = DOTween.Sequence();
        player.GetComponent<AgentMovement>().enabled = false;
        GameManager.Instance.DeadTimeScale();
        _zoom.Zoom(gameObject);
        sequence.Append(player.GetComponentInChildren<SpriteRenderer>().DOFade(0, 1.5f).OnComplete(() => Destroy(player.gameObject)));  
    }
}
