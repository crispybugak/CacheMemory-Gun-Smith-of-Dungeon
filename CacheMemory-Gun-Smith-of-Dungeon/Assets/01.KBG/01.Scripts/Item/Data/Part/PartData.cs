using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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
        Range,
        HandleSpeed,
        Capacity
    }
    
    [CreateAssetMenu(menuName = "SO/Item/Part")]
    public class PartData : ScriptableObject, IItemData
    {

        [Serializable]
        public struct PartEffect
        {
            public PartEffectType effectType;
            public float effectAmount;
        }
    
        [Serializable]
        public struct RequireIngredient
        {
            public IngredientType requiredIngredient;
            public int requiredAmount;
        
            public List<PartEffect> effects;
            public float durability;
        }
        
        
        [field:SerializeField] public Sprite icon { get; set; }
        [field:SerializeField] public float upScaling { get; set; }
        [field:SerializeField] public string itemName { get; set; }
        public PartType type;
        public PartType requirePartType;

        public float partDegree;
        public Vector2 localPos;
        
        [Header("Ingredient")]
        public List<RequireIngredient> ingredients;
    }
}
