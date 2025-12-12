using UnityEngine;

namespace KBG.Item
{
    public enum GunPowderType
    {
        
        BlackPowder,
        SmokeLessPowder
    }
    
    [CreateAssetMenu(menuName = "SO/Item/GunPowder")]
    public class GunPowder : ScriptableObject, IItemData
    {
        [field:SerializeField] public Sprite icon { get; set; }
        [field:SerializeField] public float upScaling { get; set; }
        [field:SerializeField] public string itemName { get; set; }
    }
}
