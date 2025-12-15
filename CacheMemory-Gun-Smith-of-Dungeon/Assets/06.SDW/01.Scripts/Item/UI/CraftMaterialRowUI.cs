using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _06.SDW._01.Scripts.Item.UI
{
    public class CraftMaterialRowUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text countText; // "소지 / 필요"

        public void Bind(Sprite matIcon, string matName, int owned, int required)
        {
            if (icon != null) icon.sprite = matIcon;
            if (nameText != null) nameText.text = matName ?? string.Empty;

            if (countText != null)
                countText.text = $"{owned} / {required}";
        }
    }
}