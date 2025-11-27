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
        float mainst = _stamina._currentStamina / _stamina._baseMaxStamina;
        float backst = _stamina._backStamina / _stamina._baseMaxStamina;

        if (_backStaminaBar != null) _backStaminaBar.fillAmount = backst;
        if (_staminaBar != null) _staminaBar.fillAmount = mainst;
    }

}
