using UnityEngine;

namespace  KBG.Item
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
    public class Part : ScriptableObject
    {
        public PartData itemData;
        public int durability;
    }    
}

