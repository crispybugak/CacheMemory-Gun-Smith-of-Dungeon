using System;
using UnityEngine;

namespace Item
{
    [Flags]
    public enum IngredientType
    {
        None = 0,
        Fe = 1 << 0,
        Cr =  1 << 1,
        BlackPowder  = 1 << 2,
        SmokeLessPowder = 1 << 3,
        Polyamide = 1 << 4
    }

    public enum AlloyType
    {
        None,
        Steel,
        StainlessSteel
    }
    public abstract class IngredientParent : ScriptableObject, IItem
    {
        public Sprite icon;
        public string name;
        public float mass;
        public int maxStack;
        public IngredientType ingredientType;
    }


    [CreateAssetMenu(menuName = "SO/Item/Ingredient/Ingredient")]
    public class IngredientSO : IngredientParent
    {
    }
    
    [CreateAssetMenu(menuName = "SO/Item/Ingredient/Alloy")]
    public class AlloySO : IngredientParent
    {
        public AlloyType type;
    }
}