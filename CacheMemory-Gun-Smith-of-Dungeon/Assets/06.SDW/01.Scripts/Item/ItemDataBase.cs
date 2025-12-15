using UnityEngine;

namespace KBG.Item
{
    public abstract class ItemDataBase : ScriptableObject, IItemData
    {
        // ★ 인터페이스 멤버를 여기서 '추상'으로 선언해야
        //   파생 클래스(Ingredient/PartData)가 override한 값이 IItemData로 접근해도 정상 반환됩니다.
        public abstract Sprite icon { get; set; }
        public abstract float upScaling { get; set; }
        public abstract string itemName { get; set; }
    }
}