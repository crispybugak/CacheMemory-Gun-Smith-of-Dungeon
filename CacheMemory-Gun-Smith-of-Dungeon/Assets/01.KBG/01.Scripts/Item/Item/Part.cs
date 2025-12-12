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

        public PartData partData;
        public int durability;
        public IngredientType madeBy;
    }    
}

