using UnityEngine;

namespace KBG.Item
{
    public abstract class ItemDataBase : ScriptableObject, IItemData
    {
        // IItemData에 있는 것들을 여기서 abstract로 선언하거나
        // 이미 Ingredient/PartData가 구현하고 있으면 비워둬도 됩니다.
        // (단, Ingredient/PartData가 이 클래스를 상속해야 합니다)
        public Sprite icon { get; set; }
        public float upScaling { get; set; }
        public string itemName { get; set; }
    }
}