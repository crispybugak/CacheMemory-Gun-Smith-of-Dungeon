using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image lootPanel;
    public Image exitToLobbyBtn;
    public TextMeshProUGUI exitToLobbyBtnText;

    [Header("Item")]
    public int item;

    public void GameEnd()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1f);
        sequence.Append(lootPanel.DOFade(1,0.4f));
        sequence.Join(exitToLobbyBtn.transform.DOScaleX(2f, 0.5f).SetEase(Ease.InOutExpo));
        sequence.Join(lootPanel.transform.DOScaleY(15,1f).SetEase(Ease.InOutExpo));
        sequence.Join(exitToLobbyBtn.DOFade(1, 0.4f));
        sequence.Join(exitToLobbyBtn.transform.DOScaleY(0.3f,0.5f).SetEase(Ease.InOutExpo));
        sequence.Join(exitToLobbyBtnText.DOFade(1, 0.4f));
    }

    public void GetItem()
    {

    }
}
