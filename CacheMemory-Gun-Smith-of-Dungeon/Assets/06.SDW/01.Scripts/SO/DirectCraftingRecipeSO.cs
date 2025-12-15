using System.Collections.Generic;
using KBG.Item;
using UnityEditor;
using UnityEngine;

namespace _06.SDW._01.Scripts.SO
{
    [CreateAssetMenu(fileName = "DirectRecipeSO", menuName = "SO/Crafting/Direct Crafting Recipe")]
    public class DirectCraftingRecipeSO : ScriptableObject
    {
        [Header("Result Info")]
        [Tooltip("제작 결과 아이템 (아이콘/이름용)")]
        public ItemDataBase resultPart;

        // [조건 1] 1번, 2번 재료 중 하나 선택 (OR 조건)
        [Header("Main Ingredients (Select One)")]
        [Tooltip("여기에 등록한 재료들은 '옵션'으로 표시되며, 유저는 이 중 하나를 선택해 제작합니다.")]
        [SerializeField] private List<PartData.RequireIngredient> directIngredients = new List<PartData.RequireIngredient>();

        // [조건 2] 3번 재료 필수 (AND 조건) + 2개 소모
        [Header("Mandatory Secondary Ingredient (Row 3)")]
        [Tooltip("체크하면 3번 슬롯(슬라이더)을 필수 추가 재료로 사용합니다.")]
        public bool useVariableResourceForRow3 = true;

        [Tooltip("3번 필수 재료 타입 (예: Screw)")]
        public IngredientType variableResourceType = IngredientType.GunPowder;

        [Tooltip("필수 최소 소모량 (여기에 '2'를 입력)")]
        public int minVariableUse = 2;

        // ================================================================
        //  Controller 호환 프로퍼티
        // ================================================================

        // UI 컨트롤러가 재료 목록을 달라고 할 때, PartData 대신 여기 적힌 직접 입력 리스트를 반환
        public List<PartData.RequireIngredient> Ingredients => directIngredients;

        public string DisplayName => !string.IsNullOrWhiteSpace(displayNameOverride) ? displayNameOverride : (resultPart ? resultPart.itemName : "");
        public Sprite ListIcon => listIconOverride ? listIconOverride : (resultPart ? resultPart.icon : null);
        public Sprite PreviewIcon => previewIconOverride ? previewIconOverride : (resultPart ? resultPart.icon : null);

        // UI 표시용 오버라이드 옵션들
        [Header("Display Overrides")]
        public string displayNameOverride;
        public Sprite listIconOverride;
        public Sprite previewIconOverride;
        
        public PartType ResultPartType => ((resultPart as PartData) != null) ? (resultPart as PartData).type : PartType.None;
        public PartType RequirePartType => ((resultPart as PartData) != null) ? (resultPart as PartData).requirePartType : PartType.None;

#if UNITY_EDITOR
        [SerializeField] private bool autoRenameAsset = true;
        private void OnValidate()
        {
            if (!autoRenameAsset || resultPart == null) return;
            string newName = "DIR_" + resultPart.name;
            if (this.name != newName)
            {
                string path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(path))
                {
                    this.name = newName;
                    AssetDatabase.RenameAsset(path, newName);
                }
            }
        }
#endif
    }
}