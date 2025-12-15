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
        }

        [SerializeField] private List<Entry> entries = new List<Entry>();

        private Dictionary<IngredientType, string> _map;

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
            _map = new Dictionary<IngredientType, string>();
            if (entries == null) return;

            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                if (e == null) continue;

                string name = string.IsNullOrWhiteSpace(e.displayName) ? e.type.ToString() : e.displayName;
                _map[e.type] = name;
            }
        }

        public string GetName(IngredientType type)
        {
            if (_map == null) BuildMap();
            if (_map != null && _map.TryGetValue(type, out var name))
                return name;

            // 테이블에 없으면 enum 이름으로 폴백
            return type.ToString();
        }
    }
}