using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{

    public Image fadePanel;
    public Image fadePanel2;
    public Image gameLogoTop;
    public Image gameLogoBottom;
    private void Update()
    {
        Sequence sequence = DOTween.Sequence();
        if (Keyboard.current.anyKey.wasPressedThisFrame && fadePanel != null)
        {
            fadePanel.DOFade(1f, 3f).OnComplete(() => SceneManager.LoadScene("TutorialScene"));
        }
    }

    
    private void Start()
    {
        Debug.Log("ddd");
        Sequence sequence = DOTween.Sequence();
        if(gameLogoTop != null && gameLogoBottom != null)
        {
            sequence.Append(gameLogoTop.DOFade(1,1));
            sequence.Join(gameLogoBottom.DOFade(1,1));
        }
        if(fadePanel2 != null)
        {
            fadePanel2.DOFade(0f, 4f);
        }
    }
}
