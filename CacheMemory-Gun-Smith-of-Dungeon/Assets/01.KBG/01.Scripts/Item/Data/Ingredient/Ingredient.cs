using System;
using UnityEngine;

namespace KBG.Item
{
    [Flags]
    public enum IngredientType
    {
        None = 0,
        Polyamide = 1 << 0,
        Steel = 1 << 1,
        StainlessSteel = 1 << 2
    }
    
    
    [CreateAssetMenu(menuName = "SO/Item/Ingredient")]
    public class Ingredient : ScriptableObject, IItemData
    {
        [field:SerializeField] public Sprite icon { get; set; }
        [field:SerializeField] public float upScaling { get; set; }
        [field:SerializeField] public string itemName { get; set; }
        public IngredientType type;
    }
}
