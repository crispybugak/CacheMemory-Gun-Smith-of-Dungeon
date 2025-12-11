using DG.Tweening;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [field: SerializeField] public UiInputSO UiInputSO { get; private set; }

    [SerializeField] private TextMeshProUGUI _pressToAnyButton;


    private void Start()
    {


        if (_pressToAnyButton == null) return;

        _pressToAnyButton.alpha = 0f;
        _pressToAnyButton
            .DOFade(1f, 1.5f)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
