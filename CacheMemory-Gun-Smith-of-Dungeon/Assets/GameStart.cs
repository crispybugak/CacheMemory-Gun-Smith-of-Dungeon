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
            sequence.Append(fadePanel.DOFade(1, 3));
            SceneManager.LoadScene("JHJ.MainScene");
        }
    }
}
