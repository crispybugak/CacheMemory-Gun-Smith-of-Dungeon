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
}
