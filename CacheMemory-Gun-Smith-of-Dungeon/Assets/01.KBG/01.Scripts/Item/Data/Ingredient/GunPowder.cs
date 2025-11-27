using UnityEngine;

namespace KBG.Item
{
    enum GunPowderType4
    {
        
        BlackPowder,
        SmokeLessPowder
    }
    
    [CreateAssetMenu(menuName = "SO/Item/GunPowder")]
    public class GunPowder : ScriptableObject, IItemData
    {
        public Sprite icon { get; set; }
        public string itemName { get; set; }
        public float mass { get; set; }
    }
}
