using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMoveManager : MonoBehaviour
{
    public void SelectRin()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Rin");
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }

    public void SelectRia()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Ria");
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }
}