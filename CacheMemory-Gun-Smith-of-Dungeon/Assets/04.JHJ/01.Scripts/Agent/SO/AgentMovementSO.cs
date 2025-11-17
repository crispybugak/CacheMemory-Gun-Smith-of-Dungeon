using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "AgentMovementSO", menuName = "SO/AgentMovementSO")]
public class AgentMovementSO : ScriptableObject, Controls.IAgentActions
{
    public Vector2 inputcDir { get; private set; }
    public Vector2 mouseDir { get;  set; }

    public Action OnMovePressed;
    public Action<bool> OnRunPressed;
    public Action OnInterractivePressed; // 상호작용 키 (F)
    public Action OnSkillPressed;//캐릭터별 전용 스킬 키(E)

    public Controls controls;
    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
        }
        controls.Agent.SetCallbacks(this);
        controls.Agent.Enable();
    }

    private void OnDisable()
    {
        controls.Agent.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        inputcDir = context.ReadValue<Vector2>();
        if (context.performed || context.canceled)
        {
            OnMovePressed?.Invoke();
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnRunPressed?.Invoke(true);
        if(context.canceled)
            OnRunPressed?.Invoke(false);
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mouseDir = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnUseSkill(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSkillPressed?.Invoke();
    }

    public void OnInterative(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnInterractivePressed?.Invoke();
    }
}
