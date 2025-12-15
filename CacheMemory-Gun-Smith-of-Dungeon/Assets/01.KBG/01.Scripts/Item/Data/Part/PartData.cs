using System;
using System.Collections;
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
    public class PartData : ItemDataBase
    {

        [Serializable]
        public class PartEffect
        {
            public PartEffectType effectType;
            public float effectAmount;
        }
    
        [Serializable]
        public class RequireIngredient : IEnumerable
        {
            public IngredientType requiredIngredient;
            public int requiredAmount;
        
            public List<PartEffect> effects;
            public uint durability;
            public IEnumerator GetEnumerator()
            {
                return effects.GetEnumerator();
            }
        }
        
        
        [field: SerializeField] public override Sprite icon { get; set; }
        [field: SerializeField] public override float upScaling { get; set; }
        [field: SerializeField] public override string itemName { get; set; }

        public PartType type;
        public PartType requirePartType;

        public float partDegree;
        public Vector2 localPos;
        
        [Header("Ingredient")]
        public List<RequireIngredient> ingredients;
    }
}
