using System;
using System.Text;
using _06.SDW._01.Scripts.SO;
using UnityEngine;
using TMPro;
using KBG.Item;

public class MaterialTextUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    /// <summary>
    /// 출력 형식:
    /// 재료 1 {name} {owned} / {required}
    /// 재료 2 {name} {owned} / {required}
    /// </summary>
    public void Render(
        CraftingRecipeSO recipe,
        Func<IngredientType, int> getOwned,
        Func<IngredientType, string> getName)
    {
        if (text == null) return;

        if (recipe == null || recipe.Ingredients == null || recipe.Ingredients.Count == 0)
        {
            text.text = string.Empty;
            return;
        }

        var sb = new StringBuilder(128);

        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            var req = recipe.Ingredients[i];

            int owned = getOwned != null ? getOwned(req.requiredIngredient) : 0;
            int need = Mathf.Max(1, req.requiredAmount);

            string name = getName != null ? getName(req.requiredIngredient) : req.requiredIngredient.ToString();

            sb.Append("재료 ");
            sb.Append(i + 1);
            sb.Append(' ');
            sb.Append(name);
            sb.Append(' ');
            sb.Append(owned);
            sb.Append(" / ");
            sb.Append(need);

            if (i < recipe.Ingredients.Count - 1)
                sb.AppendLine();
        }

        text.text = sb.ToString();
    }
}