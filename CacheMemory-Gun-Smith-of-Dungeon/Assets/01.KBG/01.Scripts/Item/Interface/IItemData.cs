using UnityEngine;

namespace KBG.Item
{
    public interface IItemData
    {
        Sprite icon { get; set; }
        string itemName { get; set; }
        float mass { get; set; }
    }
}