using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _staminaBar;
    [SerializeField] private Image _backStaminaBar;
    [SerializeField] private  Stamina _stamina;

    public void UpdateUI()
    {
        // ★ 패시브 포함 최대 스태미나 기준으로 비율 계산
        float max = Mathf.Max(1f, _stamina.MaxStaminaWithPassive);

        float mainst = _stamina._currentStamina / max;
        float backst = _stamina._backStamina   / max;

        if (_backStaminaBar != null) _backStaminaBar.fillAmount = backst;
        if (_staminaBar != null)     _staminaBar.fillAmount     = mainst;
    }
}

