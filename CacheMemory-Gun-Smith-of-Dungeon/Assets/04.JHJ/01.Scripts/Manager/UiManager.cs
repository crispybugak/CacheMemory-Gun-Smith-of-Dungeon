using DG.Tweening;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [field: SerializeField] public UiInputSO UiInputSO { get; private set; }

    [SerializeField] private TextMeshProUGUI _pressToAnyButton;

    [Header("Panel")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject characterSelectPanel;

    [Header("Image")]
    [SerializeField] private RectTransform _riaImage;
    [SerializeField] private RectTransform _rinImage;

    private void Awake()
    {
        if (UiInputSO == null) return;
        UiInputSO.SetOptionPanel(optionPanel);
        UiInputSO.SetcharacterSelectPanel(characterSelectPanel);
    }

    
    private void Start()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);
        if (characterSelectPanel != null)
            characterSelectPanel.SetActive(false);

        if (_pressToAnyButton == null) return;

        _pressToAnyButton.alpha = 0f;
        _pressToAnyButton
            .DOFade(1f, 1.5f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void PointerEnterImage(int id)
    {
        if(id == 1)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_riaImage.DOScale(1.2f, 0.3f));           
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_rinImage.DOScale(1.2f, 0.3f));
        }
    }
    public void PointerOutImage(int id)
    {
        if (id == 1)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_riaImage.DOScale(1, 0.2f));
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_rinImage.DOScale(1, 0.2f));
        }

    }
}
