using UnityEngine;
using UnityEngine.SceneManagement;

public class CancelDon : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
 
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
 
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        transform.SetParent(new GameObject().transform);
        gameObject.SetActive(true);
    }
}
