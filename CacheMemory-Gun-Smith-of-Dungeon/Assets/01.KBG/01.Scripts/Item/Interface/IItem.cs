using UnityEngine;

namespace KBG.Item
{
    public abstract class IItem : ScriptableObject
    {
        public abstract IItemData ItemData {get; set;}
    }
}