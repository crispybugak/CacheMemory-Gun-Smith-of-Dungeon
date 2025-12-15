using UnityEngine;

public class UIManagerDR : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    private void Update()
    {
        try
        {
            _target.SetActive(UiManager.Instance._lobbyUI._bag);
        }
        catch
        {

        }
    }
}
