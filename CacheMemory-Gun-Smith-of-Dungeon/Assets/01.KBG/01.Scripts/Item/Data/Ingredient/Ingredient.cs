using System;
using UnityEngine;

namespace KBG.Item
{
    public enum IngredientType
    {
        None,
        Polyamide,
        Steel,
        StainlessSteel,
        GunPowder
    }
    
    
    [CreateAssetMenu(menuName = "SO/Item/Ingredient")]
    public class Ingredient : ItemDataBase
    {
        [field:SerializeField] public Sprite icon { get; set; }
        [field:SerializeField] public float upScaling { get; set; }
        [field:SerializeField] public string itemName { get; set; }
        
        public IngredientType type;
    }
}
