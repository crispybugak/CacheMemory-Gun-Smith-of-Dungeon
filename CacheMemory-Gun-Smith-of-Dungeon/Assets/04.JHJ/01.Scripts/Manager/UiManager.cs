using TMPro;
using UnityEngine;
using DG;
using DG.Tweening;

public class UiManager : MonoBehaviour
{
    public TextMeshProUGUI _pressToAnyButton;


    private void Start()
    {
    }

    private void Update()
    {
        ButtonFade(_pressToAnyButton);
    }

    private void ButtonFade(TextMeshProUGUI text)
    {
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(_pressToAnyButton.DOFade(1, 3));
            mySequence.Append(_pressToAnyButton.DOFade(0, 3));
    }
}
