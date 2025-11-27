using System;
using UnityEngine;

namespace KBG.Item
{
    [Flags]
    public enum IngredientType
    {
        None = 0,
        Fe = 1 << 0,
        Cr = 1 << 1,
        Polyamide = 1 << 2,
        Steel = 1 << 3,
        StainlessSteel = 1 << 4
    }
    
    
    [CreateAssetMenu(menuName = "SO/Item/Ingredient")]
    public class Ingredient : ScriptableObject, IItemData
    {
        [field:SerializeField] public Sprite icon { get; set; }
        [field:SerializeField] public string itemName { get; set; }
        [field:SerializeField] public float mass { get; set; }
        public int maxStack;
        public IngredientType type;
    }
}
