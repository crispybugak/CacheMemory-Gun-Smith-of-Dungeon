using UnityEngine;

namespace KBG.Item
{
    public interface IItem
    {
        IItemData ItemData { get; set; }
        int Stack { get; set; }
    }
}