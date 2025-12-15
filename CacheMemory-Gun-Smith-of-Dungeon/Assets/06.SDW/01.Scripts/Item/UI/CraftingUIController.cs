using System.Collections.Generic;
using _06.SDW._01.Scripts.Item;
using _06.SDW._01.Scripts.SO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBG.Item;
using KBG.Inventory; // ★ 추가

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
    
    [SerializeField] private MaterialInventory materialInventory;
    private MaterialInventory MatInv => materialInventory != null ? materialInventory : MaterialInventory.Instance;


    private readonly List<CraftListItemUI> _leftItems = new List<CraftListItemUI>();

    private CraftingRecipeSO _selectedRecipe;
    private CraftListItemUI _selectedLeftUI;

    // ============================
    // ★ 추가: "선택된 재료 옵션" (UI에서 선택하게 될 값)
    // ============================
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

    // ============================
    // ★ UI 붙일 때 사용할 API (지금은 자동선택이 기본)
    // ============================
    public void SetSelectedIngredientOption(IngredientType type)
    {
        _selectedIngredientOption = type;
        RefreshRightPanel(_selectedRecipe);
    }

    private void HandleMaterialChanged()
    {
        // 재료 수량이 바뀌면, 만들 수 있는 옵션을 자동 선택해서 버튼 상태도 자연스럽게 갱신
        AutoSelectCraftableIngredientOption(_selectedRecipe);
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

        // ★ 레시피 선택 시에도 자동으로 "제작 가능한 옵션"을 선택
        AutoSelectCraftableIngredientOption(_selectedRecipe);

        RefreshRightPanel(_selectedRecipe);
    }

    private void RefreshRightPanel(CraftingRecipeSO recipe)
    {
        if (rightPreviewImage != null)
            rightPreviewImage.sprite = recipe != null ? recipe.PreviewIcon : null;

        if (rightItemName != null)
            rightItemName.text = recipe != null ? recipe.DisplayName : string.Empty;

        // (UI는 나중에 수정 예정)
        // 지금은 기존대로 3개 옵션이 모두 보이게 둬도 되고,
        // 다음 단계에서 "선택된 옵션만" 표시하도록 바꾸면 됩니다.
        if (materialTextUI != null)
            materialTextUI.Render(recipe, GetOwnedIngredientCount, GetIngredientName);

        bool canCraft = recipe != null && CheckCanCraft(recipe);
        if (craftButton != null) craftButton.interactable = canCraft;
        if (craftButtonLabel != null) craftButtonLabel.text = recipe == null ? "-" : (canCraft ? "제작" : "재료 부족");
    }

    // ============================
    // ★ 핵심: 옵션 3개 중 "선택된 1개"만 사용
    // ============================

    private PartData.RequireIngredient GetSelectedOption(CraftingRecipeSO recipe)
    {
        if (recipe == null || recipe.resultPart == null) return null;

        var list = recipe.resultPart.ingredients;
        if (list == null || list.Count == 0) return null;

        // 현재 선택된 재료 옵션이 리스트에 있으면 그걸 사용
        if (_selectedIngredientOption != IngredientType.None)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].requiredIngredient == _selectedIngredientOption)
                    return list[i];
            }
        }

        // 없으면 첫 번째 옵션
        return list[0];
    }

    private void AutoSelectCraftableIngredientOption(CraftingRecipeSO recipe)
    {
        if (recipe == null || recipe.resultPart == null) return;
        var list = recipe.resultPart.ingredients;
        if (list == null || list.Count == 0) return;

        // 1) 현재 선택된 옵션이 제작 가능하면 유지
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

        // 2) 제작 가능한 옵션을 우선 선택
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

        // 3) 아무것도 제작 불가면 첫 옵션으로 고정
        _selectedIngredientOption = list[0].requiredIngredient;
    }

    private bool CheckCanCraft(CraftingRecipeSO recipe)
    {
        var opt = GetSelectedOption(recipe);
        if (opt == null) return false;

        int owned = GetOwnedIngredientCount(opt.requiredIngredient);
        int need = Mathf.Max(1, opt.requiredAmount);
        return owned >= need;
    }

    private void OnClickCraft()
    {
        if (_selectedRecipe == null) return;

        if (!CheckCanCraft(_selectedRecipe))
        {
            RefreshRightPanel(_selectedRecipe);
            return;
        }

        // 인벤 빈 칸 체크(재료만 날리는 사고 방지)
        if (Inventory.Instance == null || Inventory.Instance.GetEmptyInventorySlot() == null)
        {
            Debug.LogWarning("[Crafting] 인벤토리가 가득 차서 제작할 수 없습니다.");
            RefreshRightPanel(_selectedRecipe);
            return;
        }

        // 1) 선택된 옵션 1개만 소모
        if (!TryConsumeSelectedOption(_selectedRecipe))
        {
            RefreshRightPanel(_selectedRecipe);
            return;
        }

        // 2) 결과 파츠를 인벤에 추가 (선택된 옵션으로 madeBy/durability 결정)
        if (!TryAddCraftedPartToInventory(_selectedRecipe))
        {
            Debug.LogWarning("[Crafting] 파츠 인벤 추가 실패.");
            RefreshRightPanel(_selectedRecipe);
            return;
        }

        RefreshRightPanel(_selectedRecipe);
    }

    private bool TryConsumeSelectedOption(CraftingRecipeSO recipe)
    {
        if (recipe == null || MatInv == null) return false;

        var opt = GetSelectedOption(recipe);
        if (opt == null) return false;

        int need = Mathf.Max(1, opt.requiredAmount);
        return MatInv.TryConsumeFromSlots(opt.requiredIngredient, need);
    }

    private bool TryAddCraftedPartToInventory(CraftingRecipeSO recipe)
    {
        if (recipe == null || recipe.resultPart == null) return false;
        if (Inventory.Instance == null) return false;

        var opt = GetSelectedOption(recipe);
        if (opt == null) return false;

        Part newPart = ScriptableObject.CreateInstance<Part>();
        newPart.partData = recipe.resultPart;

        // ★ 선택된 옵션이 곧 "이 파츠를 만든 재료"
        newPart.madeBy = opt.requiredIngredient;
        newPart.durability = opt.durability;

        return Inventory.Instance.AddItem(newPart);
    }

    // ============================
    // 기존: 재료 수량 / 이름
    // ============================

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
