using System.Collections.Generic;
using _06.SDW._01.Scripts.Item;
using _06.SDW._01.Scripts.SO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBG.Item;
using KBG.Inventory;
using UnityEngine.EventSystems;

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

    [Header("Right Detail - Row3 (Variable Resource Slider)")]
    [SerializeField] private GameObject gunpowderRow; // 3번 Row 컨테이너(가변 자원용)
    [SerializeField] private Slider gunpowderSlider;  // 슬라이더
    [SerializeField] private TMP_Text gunpowderValueText; // 라벨

    [Header("Ingredient Name Table")]
    [SerializeField] private IngredientNameTableSO ingredientNameTable;

    [Header("Ingredient Option Slots (Fixed 3)")]
    [SerializeField] private IngredientOptionItemUI[] optionSlots = new IngredientOptionItemUI[3];

    [Header("Craft Button (EventTrigger Only)")]
    [SerializeField] private EventTrigger craftButtonTrigger; // ★ Button 대신 EventTrigger
    [SerializeField] private TMP_Text craftButtonLabel;

    [SerializeField] private MaterialInventory materialInventory;
    private MaterialInventory MatInv => materialInventory != null ? materialInventory : MaterialInventory.Instance;

    private readonly List<CraftListItemUI> _leftItems = new List<CraftListItemUI>();

    private CraftingRecipeSO _selectedRecipe;
    private CraftListItemUI _selectedLeftUI;

    // ★ 3중 1택: 현재 선택된 재료 옵션
    private IngredientType _selectedIngredientOption = IngredientType.None;

    private void Awake()
    {
        BuildLeftList();
        BindCraftButtonTrigger();
        BindGunpowderSlider();
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
    // EventTrigger Craft Button
    // ============================

    private void BindCraftButtonTrigger()
    {
        if (craftButtonTrigger == null) return;

        if (craftButtonTrigger.triggers == null)
            craftButtonTrigger.triggers = new List<EventTrigger.Entry>();

        for (int i = craftButtonTrigger.triggers.Count - 1; i >= 0; i--)
        {
            var e = craftButtonTrigger.triggers[i];
            if (e != null && e.eventID == EventTriggerType.PointerClick)
                craftButtonTrigger.triggers.RemoveAt(i);
        }

        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener(_ => OnClickCraft());
        craftButtonTrigger.triggers.Add(entry);

        // ★ EventTrigger가 먹으려면 Graphic 레이캐스트 대상이 있어야 함
        var g = craftButtonTrigger.GetComponent<Graphic>();
        if (g != null) g.raycastTarget = true;
    }

    // ============================
    // Left List
    // ============================

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

    // ============================
    // Option Slots (3-way pick)
    // ============================

    public void SetSelectedIngredientOption(IngredientType type)
    {
        _selectedIngredientOption = type;
        RefreshIngredientOptionSlots(_selectedRecipe);
        RefreshRightPanel(_selectedRecipe);
    }

    private void RefreshIngredientOptionSlots(CraftingRecipeSO recipe)
    {
        if (optionSlots == null || optionSlots.Length == 0) return;

        var partDataForSlots = recipe != null ? recipe.resultPart as PartData : null;
        if (partDataForSlots == null || partDataForSlots.ingredients == null)
        {
            for (int i = 0; i < optionSlots.Length; i++)
                if (optionSlots[i] != null) optionSlots[i].gameObject.SetActive(false);
            return;
        }

        var list = partDataForSlots.ingredients;
        HashSet<IngredientType> seenIngredients = new HashSet<IngredientType>();

        int validIndex = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var opt = list[i];
            if (opt.requiredIngredient == IngredientType.None || seenIngredients.Contains(opt.requiredIngredient))
                continue;

            seenIngredients.Add(opt.requiredIngredient);

            if (validIndex >= optionSlots.Length) break;

            var slotUI = optionSlots[validIndex];
            if (slotUI == null) continue;

            int owned = GetOwnedIngredientCount(opt.requiredIngredient);
            int need = Mathf.Max(1, opt.requiredAmount);
            
            // ★ 수정됨: 이름과 아이콘을 모두 Table에서 가져옴
            string name = GetIngredientName(opt.requiredIngredient); 
            Sprite icon = GetIngredientIcon(opt.requiredIngredient); // 아래에 헬퍼 메서드 추가됨

            slotUI.gameObject.SetActive(true);
            
            // Bind에 icon 전달
            slotUI.Bind(
                opt.requiredIngredient,
                icon, 
                name,
                owned,
                need,
                (type, sprite) => SetSelectedIngredientOption(type)
            );
            slotUI.SetSelected(opt.requiredIngredient == _selectedIngredientOption);

            validIndex++;
        }

        for (int i = validIndex; i < optionSlots.Length; i++)
        {
            if (optionSlots[i] != null) optionSlots[i].gameObject.SetActive(false);
        }
    }
    
    private Sprite GetIngredientIcon(IngredientType type)
    {
        if (ingredientNameTable != null)
            return ingredientNameTable.GetIcon(type);

        return null;
    }

    private PartData.RequireIngredient GetSelectedOption(CraftingRecipeSO recipe)
    {
        var partData = recipe != null ? recipe.resultPart as PartData : null;
        if (partData == null) return null;
        var list = partData.ingredients;
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
        var partData2 = recipe != null ? recipe.resultPart as PartData : null;
        if (partData2 == null) return;
        var list = partData2.ingredients;
        if (list == null || list.Count == 0) return;

        // 현재 선택이 제작 가능하면 유지
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

        // 제작 가능한 옵션 우선 선택
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

        // 전부 불가면 첫 옵션
        _selectedIngredientOption = list[0].requiredIngredient;
    }

    // ============================
    // Right Panel / Craft
    // ============================

    private void HandleMaterialChanged()
    {
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

        bool canCraft = recipe != null && CheckCanCraftSelectedOption(recipe) && CheckEnoughVariableResource(recipe);

        if (craftButtonLabel != null)
            craftButtonLabel.text = recipe == null ? "-" : (canCraft ? "제작" : "재료 부족");

        RefreshVariableResourceRow(recipe);
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
        if (_selectedRecipe == null) return;

        if (!CheckCanCraftSelectedOption(_selectedRecipe) || !CheckEnoughVariableResource(_selectedRecipe))
        {
            RefreshRightPanel(_selectedRecipe);
            return;
        }

        if (Inventory.Instance == null || Inventory.Instance.GetEmptyInventorySlot() == null)
        {
            Debug.LogWarning("[Crafting] 인벤토리가 가득 차서 제작할 수 없습니다.");
            RefreshRightPanel(_selectedRecipe);
            return;
        }

        // 1) 선택 옵션 1개만 소모
        if (!TryConsumeSelectedOption(_selectedRecipe))
        {
            RefreshRightPanel(_selectedRecipe);
            return;
        }

        // 2) (예외 레시피) 가변 자원 소모
        int variableUse = GetVariableResourceToUse(_selectedRecipe);
        if (UseVariableResourceRow(_selectedRecipe) && variableUse > 0)
        {
            var vType = GetVariableResourceType(_selectedRecipe);
            if (MatInv == null || !MatInv.TryConsumeFromSlots(vType, variableUse))
            {
                Debug.LogWarning($"[Crafting] 가변 자원 소모 실패 ({vType}, {variableUse})");
                RefreshRightPanel(_selectedRecipe);
                return;
            }
        }

        // 3) 결과 파츠 생성 + 인벤 추가(선택 옵션 + 가변 자원 정보 기록)
        if (!TryAddCraftedPartToInventory(_selectedRecipe, variableUse))
        {
            Debug.LogWarning("[Crafting] 파츠 인벤 추가 실패.");
            RefreshRightPanel(_selectedRecipe);
            return;
        }

        RefreshIngredientOptionSlots(_selectedRecipe);
        RefreshRightPanel(_selectedRecipe);
    }

    private bool TryConsumeSelectedOption(CraftingRecipeSO recipe)
    {
        if (MatInv == null) return false;

        var opt = GetSelectedOption(recipe);
        if (opt == null) return false;

        int need = Mathf.Max(1, opt.requiredAmount);
        return MatInv.TryConsumeFromSlots(opt.requiredIngredient, need);
    }

    private bool TryAddCraftedPartToInventory(CraftingRecipeSO recipe, int variableUsed)
    {
        if (recipe == null || recipe.resultPart == null) return false;
        if (Inventory.Instance == null) return false;

        var opt = GetSelectedOption(recipe);
        if (opt == null) return false;

        // [수정 1] 무조건 Part로 만드는 게 아니라, 레시피에 등록된 원본 타입(MagazineItem 등)을 따라 생성합니다.
        // 이렇게 해야 MagazineItem일 때 gunPowderUsed 공간이 메모리에 생깁니다.
        Part newPart = (Part)ScriptableObject.CreateInstance(recipe.resultPart.GetType());
    
        var craftedPartData = recipe.resultPart as PartData;
        if (craftedPartData == null) return false;
    
        // 기본 데이터 세팅
        newPart.partData = craftedPartData;
        newPart.madeBy = opt.requiredIngredient;
        newPart.durability = opt.durability;

        // [수정 2] 생성된 파츠가 MagazineItem인 경우에만 형변환하여 gunPowderUsed를 넣습니다.
        if (newPart is MagazineItem magazineItem)
        {
            magazineItem.gunPowderUsed = Mathf.Max(0, variableUsed);
        
            // MagazineItem.cs에 정의된 bulletItem 등 추가 초기화가 필요하면 여기서 처리
            // 예: magazineItem.bulletItem = ...; 
        }

        return Inventory.Instance.AddItem(newPart);
    }

    // ============================
    // Count / Name
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

    // ============================
    // Row3 Variable Resource (Slider)
    // ============================

    private void BindGunpowderSlider()
    {
        if (gunpowderSlider == null) return;
        gunpowderSlider.onValueChanged.RemoveAllListeners();
        gunpowderSlider.onValueChanged.AddListener(_ =>
        {
            RefreshGunpowderLabel(_selectedRecipe);
            RefreshRightPanel(_selectedRecipe);
        });
    }

    private void RefreshVariableResourceRow(CraftingRecipeSO recipe)
    {
        bool show = UseVariableResourceRow(recipe);
        if (gunpowderRow != null) gunpowderRow.SetActive(show);
        if (!show) return;

        var vType = GetVariableResourceType(recipe);
        int owned = GetOwnedIngredientCount(vType);

        if (gunpowderSlider != null)
        {
            float prev = gunpowderSlider.value;
            gunpowderSlider.minValue = Mathf.Max(0, GetMinVariableUse(recipe));
            gunpowderSlider.maxValue = owned;
            gunpowderSlider.wholeNumbers = true;
            gunpowderSlider.value = Mathf.Clamp(prev, gunpowderSlider.minValue, gunpowderSlider.maxValue);
        }

        RefreshGunpowderLabel(recipe);
    }

    private void RefreshGunpowderLabel(CraftingRecipeSO recipe)
    {
        if (gunpowderValueText == null) return;
        var vType = GetVariableResourceType(recipe);
        int owned = GetOwnedIngredientCount(vType);
        int use = GetVariableResourceToUse(recipe);
        string resName = GetIngredientName(vType);
        gunpowderValueText.text = $"{resName}: {use}/{owned}";
    }

    private int GetVariableResourceToUse(CraftingRecipeSO recipe)
    {
        if (!UseVariableResourceRow(recipe) || gunpowderSlider == null) return 0;
        int v = Mathf.RoundToInt(gunpowderSlider.value);
        return Mathf.Max(GetMinVariableUse(recipe), v);
    }

    private bool CheckEnoughVariableResource(CraftingRecipeSO recipe)
    {
        if (!UseVariableResourceRow(recipe)) return true;
        var vType = GetVariableResourceType(recipe);
        int use = GetVariableResourceToUse(recipe);
        int owned = GetOwnedIngredientCount(vType);
        return owned >= use;
    }

    private bool UseVariableResourceRow(CraftingRecipeSO recipe)
    {
        return recipe != null && recipe.useVariableResourceForRow3;
    }

    private IngredientType GetVariableResourceType(CraftingRecipeSO recipe)
    {
        if (recipe == null) return IngredientType.None;
        return recipe.variableResourceType == IngredientType.None ? IngredientType.GunPowder : recipe.variableResourceType;
    }

    private int GetMinVariableUse(CraftingRecipeSO recipe)
    {
        if (recipe == null) return 0;
        return Mathf.Max(0, recipe.minVariableUse);
    }
}