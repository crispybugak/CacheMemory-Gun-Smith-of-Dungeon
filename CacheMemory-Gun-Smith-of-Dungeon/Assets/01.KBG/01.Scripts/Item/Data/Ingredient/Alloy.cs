    using System;
using System.Collections.Generic;
using UnityEngine;

namespace KBG.Item
{
    public enum AlloyType
    {
        None,
        Steel,
        StainlessSteel
    }
    
    
    [CreateAssetMenu(menuName = "SO/Item/Alloy")]
    public class Alloy : ScriptableObject, IItemData
    {
        [Serializable]
        public class RequireIngredient
        {
            public IngredientType IngredientType;
            public int amount;
        }
        
        
        [field:SerializeField] public Sprite icon { get; set; }
        [field:SerializeField] public string itemName { get; set; }
        [field:SerializeField] public float mass { get; set; }
        [field:SerializeField] public int maxStack { get; set; }
        public AlloyType type;
        public List<RequireIngredient> requireIngredients;
    }
}
