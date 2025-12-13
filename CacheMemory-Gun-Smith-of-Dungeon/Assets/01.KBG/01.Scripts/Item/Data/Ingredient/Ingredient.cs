using System;
using UnityEngine;

namespace KBG.Item
{
    [Flags]
    public enum IngredientType
    {
        None = 0,
        Polyamide = 1 << 0,
        Steel = 1 << 1,
        StainlessSteel = 1 << 2
    }
    
    
    [CreateAssetMenu(menuName = "SO/Item/Ingredient")]
    public class Ingredient : IItem, IItemData
    {
        [field:SerializeField] public Sprite icon { get; set; }
        [field:SerializeField] public float upScaling { get; set; }
        [field:SerializeField] public string itemName { get; set; }
        public IngredientType type;

        // 변경점 2: IItem 추상 클래스의 필수 속성을 구현합니다.
        // Ingredient는 그 자체로 데이터이므로 자기 자신(this)을 반환합니다.
        public override IItemData ItemData 
        { 
            get => this; 
            set { /* 데이터 자체가 자신이므로 set은 비워둡니다 */ } 
        }
    }
}
