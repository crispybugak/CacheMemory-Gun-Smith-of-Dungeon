using KBG.Item;
using UnityEngine;

namespace KBG.Item
{
    [CreateAssetMenu(fileName = "Item", menuName = "SO/Item/Magazine")]
    public class MagazineItem : Part
    {
        public int gunPowderUsed; // 가변 자원 사용량(기본: 화약)
        public int madeOptionIndex; // 필요 시 1/2번 옵션 저장
        
        public BulletItem bulletItem;
        
        public MagazineItem(PartData data, uint durability, IngredientType ingredientType) : base(data, durability, ingredientType)
        {
        }
    }
}