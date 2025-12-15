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
        if (stamina == null) return;

        float absoluteMax = Mathf.Max(1f, stamina.MaxStaminaWithPassive);
        float backMax = Mathf.Max(1f, stamina._backStamina); // 현재 캡(백바)

        // 백바: 전체 최대치 대비 (전체 길이 기준으로 백바가 어디까지 복구됐는지)
        float backFill = stamina._backStamina / absoluteMax;

        // 메인바: 백바(캡) 대비 (백바 안에서 현재가 얼마나 찼는지)
        float mainFill = stamina._currentStamina / backMax;

        if (_backStaminaBar != null) _backStaminaBar.fillAmount = Mathf.Clamp01(backFill);
        if (_staminaBar != null) _staminaBar.fillAmount = Mathf.Clamp01(mainFill);
    }
}
