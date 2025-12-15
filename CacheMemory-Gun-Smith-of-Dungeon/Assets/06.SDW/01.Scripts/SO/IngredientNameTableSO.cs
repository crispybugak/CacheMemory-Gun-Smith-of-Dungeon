using System.Collections.Generic;
using KBG.Item;
using UnityEngine;

namespace _06.SDW._01.Scripts.SO
{
    [CreateAssetMenu(fileName = "IngredientNameTableSO", menuName = "SO/Crafting/IngredientNameTableSO")]
    public class IngredientNameTableSO : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public IngredientType type;
            public string displayName;
            public Sprite icon; // ★ [추가] 아이콘 필드
        }

        [SerializeField] private List<Entry> entries = new List<Entry>();

        // 이름 검색용 맵
        private Dictionary<IngredientType, string> _nameMap;
        // ★ [추가] 아이콘 검색용 맵
        private Dictionary<IngredientType, Sprite> _iconMap;

        private void OnEnable()
        {
            BuildMap();
        }

        private void OnValidate()
        {
            BuildMap();
        }

        private void BuildMap()
        {
            _nameMap = new Dictionary<IngredientType, string>();
            _iconMap = new Dictionary<IngredientType, Sprite>(); // 초기화

            if (entries == null) return;

            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                if (e == null) continue;

                // 1. 이름 맵핑
                string name = string.IsNullOrWhiteSpace(e.displayName) ? e.type.ToString() : e.displayName;
                
                // 중복 키 방지 (같은 타입이 리스트에 두 번 있으면 에러나므로 체크)
                if (!_nameMap.ContainsKey(e.type))
                    _nameMap.Add(e.type, name);

                // 2. 아이콘 맵핑
                if (!_iconMap.ContainsKey(e.type))
                    _iconMap.Add(e.type, e.icon);
            }
        }

        public string GetName(IngredientType type)
        {
            if (_nameMap == null) BuildMap();
            if (_nameMap != null && _nameMap.TryGetValue(type, out var name))
                return name;

            return type.ToString();
        }

        // ★ [추가] 아이콘 가져오는 메서드
        public Sprite GetIcon(IngredientType type)
        {
            if (_iconMap == null) BuildMap();
            
            if (_iconMap != null && _iconMap.TryGetValue(type, out var icon))
                return icon;

            return null; // 없으면 null 반환
        }
    }
}