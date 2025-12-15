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
        
        // 제작 시 추가 정보(총알 등 예외 레시피에서 사용)
        public int gunPowderUsed; // 가변 자원 사용량(기본: 화약)
        public int madeOptionIndex; // 필요 시 1/2번 옵션 저장
    }    
}

