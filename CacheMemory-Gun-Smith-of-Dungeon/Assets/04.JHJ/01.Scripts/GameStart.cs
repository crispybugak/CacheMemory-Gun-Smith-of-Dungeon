using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{

    public Image fadePanel;
    public Image fadePanel2;
    private void Update()
    {
        Sequence sequence = DOTween.Sequence();
        if (Keyboard.current.anyKey.wasPressedThisFrame && fadePanel != null)
        {
            fadePanel.DOFade(1f, 3f).OnComplete(() => SceneManager.LoadScene("JHJ.LobbyScene"));

        }
    }

    private void Start()
    {
        Sequence sequence = DOTween.Sequence();
        if(fadePanel2 != null)
        {
            fadePanel2.DOFade(0f, 4f);
        }
    }
}
