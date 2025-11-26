using System;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public enum PartType
    {
        Muzzle = 0,
        Barrel = 1,
        Sight = 2,
        Magazine = 3,
        Grip = 4,
        Stock = 5,
        Base = 6
    }
    
    public enum ItemEffectType
    {
        Damage,
        RecoilControl,
        Accuracy,
        HandleSpeed,
        MoveSpeed,
        Magazine
    }
    
    [CreateAssetMenu(menuName = "SO/Item/Part")]
    public class PartSO : ScriptableObject, IItem
    {
        public Sprite icon; 
        public string name;
        public PartType type;
        public float mass;
        public IngredientType requireIngredientType;
        public List<ItemEffect> effects;

        public float masDurability;
        public float durability;
        
        [Header("UsedIngredient")]
        public IngredientParent usedIngredient;
    }

    [Serializable]
    public class ItemEffect
    {
        public ItemEffectType type;
        public float effectAmount;
    }
}