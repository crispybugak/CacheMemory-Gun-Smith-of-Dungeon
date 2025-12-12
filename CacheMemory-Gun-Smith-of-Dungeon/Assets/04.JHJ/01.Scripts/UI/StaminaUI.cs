using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _staminaBar;
    [SerializeField] private Image _backStaminaBar;
    public Stamina stamina;

    public void UpdateUI()
    {
        // ★ 패시브 포함 최대 스태미나 기준으로 비율 계산
        float max = Mathf.Max(1f, stamina.MaxStaminaWithPassive);

        float mainst = stamina._currentStamina / max;
        float backst = stamina._backStamina   / max;

        if (_backStaminaBar != null) _backStaminaBar.fillAmount = backst;
        if (_staminaBar != null)     _staminaBar.fillAmount     = mainst;
    }
}

