using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    private Coroutine _timeScaleCo;

    private void Awake()
    {
        // 씬이 바뀔 때마다 무조건 TimeScale 복구
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ResetTimeScale();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetTimeScale();
    }

    public void ResetTimeScale()
    {
        if (_timeScaleCo != null)
        {
            StopCoroutine(_timeScaleCo);
            _timeScaleCo = null;
        }

        Time.timeScale = 1f;
    }

    private IEnumerator TimeScaleCT(float duration, float timeScale)
    {
        Time.timeScale = timeScale;

        // timeScale 영향을 받지 않는 실제 시간 기준 대기
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        _timeScaleCo = null;
    }

    public void HitTimeScale()
    {
        // 겹치지 않게 기존 코루틴 정리 후 시작
        if (_timeScaleCo != null) StopCoroutine(_timeScaleCo);
        _timeScaleCo = StartCoroutine(TimeScaleCT(0.1f, 0.7f));
    }

    public void DeadTimeScale()
    {
        if (_timeScaleCo != null) StopCoroutine(_timeScaleCo);
        _timeScaleCo = StartCoroutine(TimeScaleCT(3f, 0.3f));
    }

    public void SceneMoveToLobby()
    {
        // 씬 전환 직전에 무조건 복구 (다음 씬에 절대 안 남음)
        ResetTimeScale();
        SceneManager.LoadScene("LobbyScene");
    }
}
