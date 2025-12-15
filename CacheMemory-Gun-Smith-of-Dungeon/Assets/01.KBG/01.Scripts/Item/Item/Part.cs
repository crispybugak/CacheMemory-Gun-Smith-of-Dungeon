using UnityEngine;
using UnityEngine.Serialization;

namespace  KBG.Item
{
    [CreateAssetMenu(fileName = "Item", menuName = "SO/Item/Data")]
    public class Part : IItem
    {
        public override IItemData ItemData
        {
            get => partData;
            set => partData = value as  PartData;
        }

        public Part(PartData data, uint durability, IngredientType ingredientType)
        {
            partData = data;
            this.durability = durability;
            madeBy =  ingredientType;
        }
        
        public PartData partData;
        public uint durability;
        public IngredientType madeBy;
    }    
}

