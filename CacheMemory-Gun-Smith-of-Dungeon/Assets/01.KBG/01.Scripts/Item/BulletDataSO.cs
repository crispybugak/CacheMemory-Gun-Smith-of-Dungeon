using UnityEngine;

namespace Item
{ public enum BulletType
    {
        None,
        FMJ,
        Subsonic,
        AP
    }
    
    [CreateAssetMenu(menuName = "SO/Item/BulletData")]
    public class BulletDataSO : ScriptableObject, IItem
    {
        public Sprite icon;
        public BulletType bulletType;
        public float gunPowderAmount;
        public float damage;
        public float speed;
    }
}
