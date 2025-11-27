using System;
using System.Collections.Generic;
using UnityEngine;

namespace KBG.Item
{
    public enum PartType
    {
        None,
        Muzzle,
        Barrel,
        Base,
        Stock,
        Magazine,
        Sight,
        Grip
    }

    public enum PartEffectType
    {
        Damage,
        RecoilControl,
        Accuracy,
        HandleSpeed,
        MoveSpeed,
        Capacity
    }

    [Serializable]
    public class PartEffect
    {
        [field:SerializeField] public PartEffectType EffectType { get; private set; }
        [field: SerializeField] public float EffectAmount { get; private set; }
    }
    
    [Serializable]
    public class RequireIngredient
    {
        public IngredientType requiredIngredient;
        public int requiredAmount;
        
        public float durability;
    }
    [CreateAssetMenu(menuName = "SO/Item/Part/Part", order = 0)]
    public abstract class PartData : ScriptableObject, IItemData
    {
        [field:SerializeField] public Sprite icon { get; set; }
        [field:SerializeField] public string itemName { get; set; }
        [field:SerializeField] public float mass { get; set; }
        public PartType type;
        public PartType requirePartType;

        public List<PartEffect> effects;
        
        [Header("Ingredient")]
        public List<RequireIngredient> ingredients;
    }
}
