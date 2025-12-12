using UnityEngine;

namespace KBG.Item
{
    public interface IItemData
    {
        Sprite icon { get; set; }
        float upScaling { get; set; }
        string itemName { get; set; }
    }
}