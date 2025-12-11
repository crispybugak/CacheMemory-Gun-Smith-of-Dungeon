using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image lootPanel;

    public void GameEnd()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1f);
        sequence.Append(lootPanel.DOFade(1,0.4f));
        sequence.Join(lootPanel.transform.DOScaleY(15,1f).SetEase(Ease.InExpo));
    }
}
