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

    [Header("Scene Guards")]
    [SerializeField] private string lobbySceneName = "LobbyScene";
    [SerializeField] private string nextSceneName = "TutorialScene";

    private bool _isLoading;
    private Sequence _logoSeq;

    private void Update()
    {
        if (_isLoading)
            return;

        string sceneName = SceneManager.GetActiveScene().name;
        if (Keyboard.current == null)
            return;

        // 로비에서는 Space만 허용, 그 외 키 입력은 무시
        if (sceneName == lobbySceneName)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                StartLoad();
            return;
        }

        // 로비가 아닌 씬에서는 AnyKey로 진행(필요 없으면 지워)
        if (Keyboard.current.anyKey.wasPressedThisFrame)
            StartLoad();
    }

    private void StartLoad()
    {
        if (_isLoading)
            return;

        _isLoading = true;

        if (fadePanel == null)
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        fadePanel.DOKill();
        fadePanel.DOFade(1f, 3f)
            .SetUpdate(true)
            .OnComplete(() => SceneManager.LoadScene(nextSceneName));
    }

    private void Start()
    {
        Debug.Log("ddd");

        _logoSeq?.Kill();
        _logoSeq = DOTween.Sequence();

        if (gameLogoTop != null)
            _logoSeq.Append(gameLogoTop.DOFade(1f, 1f));

        if (gameLogoBottom != null)
            _logoSeq.Join(gameLogoBottom.DOFade(1f, 1f));

        if (fadePanel2 != null)
        {
            fadePanel2.DOKill();
            fadePanel2.DOFade(0f, 4f);
        }
    }

    private void OnDisable()
    {
        _logoSeq?.Kill();
    }
}
