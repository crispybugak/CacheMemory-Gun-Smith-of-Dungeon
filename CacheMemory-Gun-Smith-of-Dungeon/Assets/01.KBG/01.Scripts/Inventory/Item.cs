using UnityEngine;

namespace KBG.Item
{
    [CreateAssetMenu(menuName = "SO/Item/Item")]
    public class Item : IItem
    {
        [SerializeField] private ItemDataBase itemData;

        public override IItemData ItemData
        {
            get => itemData;
            set => itemData = value as ItemDataBase;
        }
    }
}