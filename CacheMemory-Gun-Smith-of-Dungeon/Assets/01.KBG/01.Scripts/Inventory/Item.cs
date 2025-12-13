using UnityEngine;

namespace KBG.Item
{
    [CreateAssetMenu(menuName = "SO/Item/Item")]
    public class Item : IItem
    {
        public override IItemData ItemData
        {
            get => itemData;
            set => itemData = value;
        }

        [SerializeField] private IItemData itemData;
    }
}
