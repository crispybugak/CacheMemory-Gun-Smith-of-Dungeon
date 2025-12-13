using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoSingleton<UiManager>
{
    [field: SerializeField] public UiInputSO UiInputSO { get; private set; }

    [SerializeField] private TextMeshProUGUI _pressToAnyButton;

    [Header("Error Message")]
    [SerializeField] private TextMeshProUGUI _errorMessage;
    [SerializeField] private Image _errorMessageImage;

    private Coroutine _errCo;

    private void Start()
    {
        if (_errorMessageImage != null)
            _errorMessageImage.gameObject.SetActive(false);

        if (_errorMessage != null)
            _errorMessage.alpha = 1f;

        if (_pressToAnyButton != null)
        {
            _pressToAnyButton.alpha = 0f;
            _pressToAnyButton.DOFade(1f, 1.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void ShowError(Color c, string msg)
    {
        if (!_errorMessage || !_errorMessageImage) return;
        if (_errCo != null) StopCoroutine(_errCo);
        DOTween.Kill(_errorMessage); DOTween.Kill(_errorMessageImage);

        _errCo = StartCoroutine(Run());
        IEnumerator Run()
        {
            _errorMessage.text = msg;
            _errorMessage.color = new Color(c.r, c.g, c.b, 1f);
            _errorMessage.alpha = 1f;

            _errorMessageImage.gameObject.SetActive(true);
            _errorMessageImage.color = new Color(_errorMessageImage.color.r, _errorMessageImage.color.g, _errorMessageImage.color.b, 1f);

            yield return new WaitForSeconds(1f);

            _errorMessage.DOFade(0, .5f);
            _errorMessageImage.DOFade(0, .5f);

            yield return new WaitForSeconds(.5f);
            _errorMessageImage.gameObject.SetActive(false);
            _errCo = null;
        }
    }
}
