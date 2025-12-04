using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{

    public Image fadePanel;
    private void Update()
    {
        Sequence sequence = DOTween.Sequence();
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            fadePanel.DOFade(1f, 3f).OnComplete(() => SceneManager.LoadScene("JHJ.MainScene"));

        }
    }
}
