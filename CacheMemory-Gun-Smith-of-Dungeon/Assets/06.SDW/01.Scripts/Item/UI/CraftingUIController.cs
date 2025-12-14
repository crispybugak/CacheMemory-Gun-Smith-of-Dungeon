using System.Collections.Generic;
using _06.SDW._01.Scripts.SO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBG.Item;

public class CraftingUIController : MonoBehaviour
{
    [Header("Recipes (PartData based)")]
    [SerializeField] private List<CraftingRecipeSO> recipes = new List<CraftingRecipeSO>();

    [Header("Left List")]
    [SerializeField] private Transform leftListContent;
    [SerializeField] private CraftListItemUI leftItemPrefab;

    [Header("Right Detail")]
    [SerializeField] private Image rightPreviewImage;
    [SerializeField] private TMP_Text rightItemName;

    [Header("Materials (Single Text)")]
    [SerializeField] private MaterialTextUI materialTextUI;

    [Header("Ingredient Name Table")]
    [SerializeField] private IngredientNameTableSO ingredientNameTable; // ★ 추가

    [Header("Craft Button (ONLY Button Input)")]
    [SerializeField] private Button craftButton;
    [SerializeField] private TMP_Text craftButtonLabel;

    private readonly List<CraftListItemUI> _leftItems = new List<CraftListItemUI>();

    private CraftingRecipeSO _selectedRecipe;
    private CraftListItemUI _selectedLeftUI;

    private void Awake()
    {
        BuildLeftList();

        if (craftButton != null)
        {
            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(OnClickCraft);
        }
    }

    private void Start()
    {
        if (recipes != null && recipes.Count > 0 && _leftItems.Count > 0 && _leftItems[0] != null)
            SelectRecipe(recipes[0], _leftItems[0]);
        else
            RefreshRightPanel(null);
    }

    private void BuildLeftList()
    {
        for (int i = 0; i < _leftItems.Count; i++)
            if (_leftItems[i] != null) Destroy(_leftItems[i].gameObject);
        _leftItems.Clear();

        _selectedLeftUI = null;
        _selectedRecipe = null;

        if (leftListContent == null || leftItemPrefab == null || recipes == null) return;

        for (int i = 0; i < recipes.Count; i++)
        {
            var r = recipes[i];
            if (r == null) continue;

            var ui = Instantiate(leftItemPrefab, leftListContent);
            ui.Bind(r, SelectRecipe);
            ui.SetSelected(false);

            _leftItems.Add(ui);
        }
    }

    private void SelectRecipe(CraftingRecipeSO recipe, CraftListItemUI clickedUI)
    {
        if (recipe == null || clickedUI == null) return;

        if (_selectedLeftUI != null)
            _selectedLeftUI.SetSelected(false);

        _selectedLeftUI = clickedUI;
        _selectedLeftUI.SetSelected(true);

        _selectedRecipe = recipe;
        RefreshRightPanel(_selectedRecipe);
    }

    private void RefreshRightPanel(CraftingRecipeSO recipe)
    {
        if (rightPreviewImage != null)
            rightPreviewImage.sprite = recipe != null ? recipe.PreviewIcon : null;

        if (rightItemName != null)
            rightItemName.text = recipe != null ? recipe.DisplayName : string.Empty;

        // ★ 단일 텍스트로 재료 출력(재료명 포함)
        if (materialTextUI != null)
            materialTextUI.Render(recipe, GetOwnedIngredientCount, GetIngredientName);

        bool canCraft = recipe != null && CheckCanCraft(recipe);
        if (craftButton != null) craftButton.interactable = canCraft;
        if (craftButtonLabel != null) craftButtonLabel.text = recipe == null ? "-" : (canCraft ? "제작" : "재료 부족");
    }

    private bool CheckCanCraft(CraftingRecipeSO recipe)
    {
        if (recipe == null || recipe.Ingredients == null) return false;

        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            var req = recipe.Ingredients[i];
            int owned = GetOwnedIngredientCount(req.requiredIngredient);
            int need = Mathf.Max(1, req.requiredAmount);
            if (owned < need) return false;
        }
        return true;
    }

    private void OnClickCraft()
    {
        if (_selectedRecipe == null) return;

        if (!CheckCanCraft(_selectedRecipe))
        {
            RefreshRightPanel(_selectedRecipe);
            return;
        }

        // TODO: 재료 차감 / 결과 지급 로직은 네 인벤 구조에 맞게 유지
    }

    // ============================
    // 프로젝트에 맞게 채워야 하는 부분
    // ============================

    private int GetOwnedIngredientCount(IngredientType type)
    {
        // TODO: IngredientType 소지량 조회 로직으로 교체
        return 0;
    }

    private string GetIngredientName(IngredientType type)
    {
        if (ingredientNameTable != null)
            return ingredientNameTable.GetName(type);

        // 테이블이 없으면 enum 이름 폴백
        return type.ToString();
    }
}
