using System.Text;
using _06.SDW._01.Scripts.SO;
using KBG.Item;
using TMPro;
using UnityEngine;

public class MaterialTextUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void Render(
        CraftingRecipeSO recipe,
        System.Func<IngredientType, int> getOwned,
        System.Func<IngredientType, string> getName)
    {
        if (text == null) return;

        if (recipe == null || recipe.Ingredients == null || recipe.Ingredients.Count == 0)
        {
            text.text = string.Empty;
            return;
        }

        StringBuilder sb = new StringBuilder(128);

        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            var req = recipe.Ingredients[i];

            string name = getName != null ? getName(req.requiredIngredient) : req.requiredIngredient.ToString();
            int owned = getOwned != null ? getOwned(req.requiredIngredient) : 0;
            int need = Mathf.Max(1, req.requiredAmount);

            sb.Append(name).Append(' ')
                .Append(owned).Append(" / ")
                .Append(need);

            if (i < recipe.Ingredients.Count - 1)
                sb.AppendLine();
        }

        text.text = sb.ToString();
    }
}