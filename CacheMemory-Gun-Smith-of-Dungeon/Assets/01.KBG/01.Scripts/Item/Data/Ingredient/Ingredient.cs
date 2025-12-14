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
        [field: SerializeField] public override Sprite icon { get; set; }
        [field: SerializeField] public override float upScaling { get; set; }
        [field: SerializeField] public override string itemName { get; set; }
        
        public IngredientType type;
    }
}
