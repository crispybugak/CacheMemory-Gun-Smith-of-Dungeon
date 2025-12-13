using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    
    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    
    [Header("Scene Entry")]
    [SerializeField] private bool fadeInOnStart = true;
    
    private bool isTransitioning = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        if (fadeInOnStart)
        {
            Time.timeScale = 1;
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.blocksRaycasts = true;
            
            fadeCanvasGroup.DOFade(0f, fadeDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => fadeCanvasGroup.blocksRaycasts = false);
        }
        else
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }
    
    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isTransitioning = true;
        fadeCanvasGroup.blocksRaycasts = true;
        
        yield return fadeCanvasGroup.DOFade(1f, fadeDuration)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        
        while (operation.progress < 0.9f)
        {
            yield return null;
        }
        
        operation.allowSceneActivation = true;
        yield return new WaitUntil(() => operation.isDone);
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
