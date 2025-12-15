using System.Collections.Generic;
using KBG.Item;
using UnityEditor;
using UnityEngine;

namespace _06.SDW._01.Scripts.SO
{
    [CreateAssetMenu(fileName = "CraftingRecipeSO", menuName = "SO/Crafting/CraftingRecipeSO")]
    public class CraftingRecipeSO : ScriptableObject
    {
        [Header("Result Part")]
        [Tooltip("제작 결과 파츠 데이터(PartData). 재료 요구사항은 PartData.ingredients를 사용합니다.")]
        public ItemDataBase resultPart;

        [Header("Row3 Variable Resource (Ammo 등 예외용)")]
        [Tooltip("이 레시피는 3번 Row를 슬라이더(가변 재료)로 사용합니다.")]
        public bool useVariableResourceForRow3 = false;

        [Tooltip("슬라이더로 사용할 가변 재료 타입(예: GunPowder)")]
        public IngredientType variableResourceType = IngredientType.GunPowder;

        [Tooltip("가변 재료 최소 사용량(0 또는 1 등)")]
        public int minVariableUse = 0;

        [Header("Display Overrides (Optional)")]
        [Tooltip("비워두면 PartData.itemName을 사용")]
        public string displayNameOverride;

        [Tooltip("비워두면 PartData.icon을 사용")]
        public Sprite listIconOverride;

        [Tooltip("비워두면 PartData.icon을 사용")]
        public Sprite previewIconOverride;

        [Header("Optional Rule Overrides")]
        public PartType partTypeOverride = PartType.None;
        public PartType requirePartTypeOverride = PartType.None;

        // ---------- Derived ----------
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(displayNameOverride))
                    return displayNameOverride;

                return resultPart != null ? resultPart.itemName : string.Empty;
            }
        }

        public Sprite ListIcon => listIconOverride != null ? listIconOverride : (resultPart != null ? resultPart.icon : null);
        public Sprite PreviewIcon => previewIconOverride != null ? previewIconOverride : (resultPart != null ? resultPart.icon : null);

        public PartType ResultPartType
            => partTypeOverride != PartType.None
                ? partTypeOverride
                : ((resultPart as PartData) != null ? (resultPart as PartData).type : PartType.None);

        public PartType RequirePartType
            => requirePartTypeOverride != PartType.None
                ? requirePartTypeOverride
                : ((resultPart as PartData) != null ? (resultPart as PartData).requirePartType : PartType.None);

        public List<PartData.RequireIngredient> Ingredients
            => (resultPart as PartData) != null ? (resultPart as PartData).ingredients : null;

#if UNITY_EDITOR
        [Header("Editor Convenience")]
        [Tooltip("resultPart를 넣거나 바꾸면 이 레시피 SO 에셋 이름을 자동으로 변경합니다.")]
        [SerializeField] private bool autoRenameAsset = true;

        [Tooltip("이 접두사를 붙여 저장하고 싶으면 사용. 예: RECIPE_")]
        [SerializeField] private string assetNamePrefix = "RECIPE_";

        [Tooltip("파일명에 사용할 문자열. 기본은 PartData.itemName, 비어있으면 PartData.name을 사용")]
        [SerializeField] private bool preferItemName = true;

        [SerializeField, HideInInspector] private PartData _lastResultPart;

        private void OnValidate()
        {
            if (!autoRenameAsset) return;

            // 결과 파츠가 없으면 이름 변경하지 않음
            if (resultPart == null)
            {
                _lastResultPart = null;
                return;
            }

            // 같은 값이면 불필요한 Rename 방지
            if (_lastResultPart == resultPart) return;
            _lastResultPart = (PartData)resultPart;

            // 새 파일명 결정
            string baseName = null;

            if (preferItemName)
                baseName = string.IsNullOrWhiteSpace(resultPart.itemName) ? null : resultPart.itemName;

            if (string.IsNullOrWhiteSpace(baseName))
                baseName = resultPart.name; // PartData SO 이름 fallback

            string targetName = SanitizeFileName($"{assetNamePrefix}{baseName}");

            // 에셋으로 저장된 SO만 파일명 변경 가능
            string path = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrWhiteSpace(path)) return;

            // 이미 같은 이름이면 종료
            if (this.name == targetName) return;

            // 오브젝트 이름(인스펙터 표시) 갱신
            this.name = targetName;

            // 파일명 변경 (중복되면 Unity가 (1) 붙일 수 있음)
            AssetDatabase.RenameAsset(path, targetName);
            AssetDatabase.SaveAssets();
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "RECIPE_Unknown";

            // Windows 기준 금지 문자 제거
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
                name = name.Replace(c.ToString(), "");

            // 공백 정리
            name = name.Trim();
            return string.IsNullOrWhiteSpace(name) ? "RECIPE_Unknown" : name;
        }
#endif
    }
}
