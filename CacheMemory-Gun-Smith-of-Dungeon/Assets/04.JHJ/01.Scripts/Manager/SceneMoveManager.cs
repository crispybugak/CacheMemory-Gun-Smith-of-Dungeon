using UnityEngine.SceneManagement;

public class SceneMoveManager : MonoSingleton<SceneMoveManager>
{
    public void MoveToGameScene()
    {
        SceneManager.LoadScene("JHJGameScene");
    }
}
