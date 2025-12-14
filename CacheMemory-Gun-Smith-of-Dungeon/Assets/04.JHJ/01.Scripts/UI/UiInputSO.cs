using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[CreateAssetMenu(fileName = "UiInputSO", menuName = "Scriptable Objects/UiInputSO")]
public class UiInputSO : ScriptableObject, Controls.IUIActions
{
    public event Action onInventoryPressed;
    
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
    private void OnDisable()
    {
        controls.Agent.Disable();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
            onInventoryPressed?.Invoke();
    }
}
