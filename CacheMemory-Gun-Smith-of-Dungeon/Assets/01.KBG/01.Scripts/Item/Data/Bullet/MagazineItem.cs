using KBG.Item;
using UnityEngine;

namespace KBG.Item
{
    public class MagazineItem : IItem
    {
        public override IItemData ItemData { get; set; }

        public PartData PartData
        {
            get => ItemData as PartData;
            set { if (value.type == PartType.Magazine) ItemData = value; }
        }

        public uint remain;
        public BulletItem bullet;

        public MagazineItem(PartData data, uint durability, BulletItem bullet)
        {
            
        }
    }
}