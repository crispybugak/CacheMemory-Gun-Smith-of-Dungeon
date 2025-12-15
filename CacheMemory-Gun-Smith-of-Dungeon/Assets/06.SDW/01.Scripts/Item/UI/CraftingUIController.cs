using System.Collections.Generic;
using _06.SDW._01.Scripts.Item;
using _06.SDW._01.Scripts.SO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBG.Item;
using KBG.Inventory;

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

    [Header("Ingredient Name Table")]
    [SerializeField] private IngredientNameTableSO ingredientNameTable;

    [Header("Ingredient Option Slots (Fixed 3)")]
    [SerializeField] private IngredientOptionItemUI[] optionSlots = new IngredientOptionItemUI[3]; // ★ 추가

    [Header("Craft Button (ONLY Button Input)")]
    [SerializeField] private Button craftButton;
    [SerializeField] private TMP_Text craftButtonLabel;

    [SerializeField] private MaterialInventory materialInventory;
    private MaterialInventory MatInv => materialInventory != null ? materialInventory : MaterialInventory.Instance;

    private readonly List<CraftListItemUI> _leftItems = new List<CraftListItemUI>();
    private CraftingRecipeSO _selectedRecipe;
    private CraftListItemUI _selectedLeftUI;

    private IngredientType _selectedIngredientOption = IngredientType.None;

    private void Awake()
    {
        BuildLeftList();

        if (craftButton != null)
        {
            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(OnClickCraft);
        }
    }

    private void OnEnable()
    {
        if (MatInv != null)
            MatInv.OnChanged += HandleMaterialChanged;
    }

    private void OnDisable()
    {
        if (MatInv != null)
            MatInv.OnChanged -= HandleMaterialChanged;
    }

    private void Start()
    {
        if (recipes != null && recipes.Count > 0 && _leftItems.Count > 0 && _leftItems[0] != null)
            SelectRecipe(recipes[0], _leftItems[0]);
        else
            RefreshRightPanel(null);
    }

    public void SetSelectedIngredientOption(IngredientType type)
    {
        _selectedIngredientOption = type;
        RefreshIngredientOptionSlots(_selectedRecipe);
        RefreshRightPanel(_selectedRecipe);
    }

    private void HandleMaterialChanged()
    {
        AutoSelectCraftableIngredientOption(_selectedRecipe);
        RefreshIngredientOptionSlots(_selectedRecipe);
        RefreshRightPanel(_selectedRecipe);
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

        AutoSelectCraftableIngredientOption(_selectedRecipe);
        RefreshIngredientOptionSlots(_selectedRecipe);
        RefreshRightPanel(_selectedRecipe);
    }

    private void RefreshRightPanel(CraftingRecipeSO recipe)
    {
        if (rightPreviewImage != null)
            rightPreviewImage.sprite = recipe != null ? recipe.PreviewIcon : null;

        if (rightItemName != null)
            rightItemName.text = recipe != null ? recipe.DisplayName : string.Empty;

        bool canCraft = recipe != null && CheckCanCraftSelectedOption(recipe);
        if (craftButton != null) craftButton.interactable = canCraft;
        if (craftButtonLabel != null) craftButtonLabel.text = recipe == null ? "-" : (canCraft ? "제작" : "재료 부족");
    }

    // ============================
    // ★ 고정 3슬롯에 값만 채우기
    // ============================
    private void RefreshIngredientOptionSlots(CraftingRecipeSO recipe)
    {
        // 슬롯이 없거나 레시피가 없으면 전부 숨김
        if (optionSlots == null || optionSlots.Length == 0)
            return;

        if (recipe == null || recipe.resultPart == null || recipe.resultPart.ingredients == null)
        {
            for (int i = 0; i < optionSlots.Length; i++)
                if (optionSlots[i] != null) optionSlots[i].gameObject.SetActive(false);
            return;
        }

        var list = recipe.resultPart.ingredients;

        for (int i = 0; i < optionSlots.Length; i++)
        {
            var slotUI = optionSlots[i];
            if (slotUI == null) continue;

            if (i >= list.Count)
            {
                slotUI.gameObject.SetActive(false);
                continue;
            }

            var opt = list[i];

            int owned = GetOwnedIngredientCount(opt.requiredIngredient);
            int need = Mathf.Max(1, opt.requiredAmount);
            string name = GetIngredientName(opt.requiredIngredient);

            // 아이콘은 아직 테이블 없으니 null(원하면 나중에 매핑 추가)
            Sprite icon = null;

            slotUI.gameObject.SetActive(true);
            slotUI.Bind(opt.requiredIngredient, icon, name, owned, need, SetSelectedIngredientOption);
            slotUI.SetSelected(opt.requiredIngredient == _selectedIngredientOption);
        }
    }

    // ============================
    // 선택 옵션 기반
    // ============================
    private PartData.RequireIngredient GetSelectedOption(CraftingRecipeSO recipe)
    {
        if (recipe == null || recipe.resultPart == null) return null;
        var list = recipe.resultPart.ingredients;
        if (list == null || list.Count == 0) return null;

        if (_selectedIngredientOption != IngredientType.None)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].requiredIngredient == _selectedIngredientOption)
                    return list[i];
        }

        return list[0];
    }

    private void AutoSelectCraftableIngredientOption(CraftingRecipeSO recipe)
    {
        if (recipe == null || recipe.resultPart == null) return;
        var list = recipe.resultPart.ingredients;
        if (list == null || list.Count == 0) return;

        var cur = GetSelectedOption(recipe);
        if (cur != null)
        {
            int ownedCur = GetOwnedIngredientCount(cur.requiredIngredient);
            int needCur = Mathf.Max(1, cur.requiredAmount);
            if (ownedCur >= needCur)
            {
                _selectedIngredientOption = cur.requiredIngredient;
                return;
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            var opt = list[i];
            int owned = GetOwnedIngredientCount(opt.requiredIngredient);
            int need = Mathf.Max(1, opt.requiredAmount);
            if (owned >= need)
            {
                _selectedIngredientOption = opt.requiredIngredient;
                return;
            }
        }

        _selectedIngredientOption = list[0].requiredIngredient;
    }

    private bool CheckCanCraftSelectedOption(CraftingRecipeSO recipe)
    {
        var opt = GetSelectedOption(recipe);
        if (opt == null) return false;

        int owned = GetOwnedIngredientCount(opt.requiredIngredient);
        int need = Mathf.Max(1, opt.requiredAmount);
        return owned >= need;
    }

    private void OnClickCraft()
    {
        // 여기엔 기존에 만들어둔 "선택 옵션 1개 소모 + 파츠 인벤 추가" 로직을 그대로 사용하면 됩니다.
        // (현재 대화 맥락상 이미 적용된 상태라고 보고 생략)
    }

    private int GetOwnedIngredientCount(IngredientType type)
    {
        if (MatInv == null) return 0;
        return MatInv.GetCount(type);
    }

    private string GetIngredientName(IngredientType type)
    {
        if (ingredientNameTable != null)
            return ingredientNameTable.GetName(type);

        return type.ToString();
    }
}