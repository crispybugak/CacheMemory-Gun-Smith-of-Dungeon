using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UiInputSO", menuName = "Scriptable Objects/UiInputSO")]
public class UiInputSO : ScriptableObject, Controls.IUIActions
{
    [field:SerializeField] private GameObject optionPanel;
    [field: SerializeField] private GameObject characterSelectPanel;
    public Controls controls;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
        }
        controls.UI.SetCallbacks(this);
        controls.UI.Enable();
    }

    public void SetOptionPanel(GameObject panel)
    {
        optionPanel = panel;
    }

    public void SetcharacterSelectPanel(GameObject panel)
    {
        characterSelectPanel = panel;
    }

    public void OnOption(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (optionPanel == null) return;

        bool isActive = optionPanel.activeSelf;
        optionPanel.SetActive(!isActive);
    }

    public void OnCharacterSelect(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (characterSelectPanel == null) return;

        bool isActive = characterSelectPanel.activeSelf;
        characterSelectPanel.SetActive(!isActive);
    }
}
