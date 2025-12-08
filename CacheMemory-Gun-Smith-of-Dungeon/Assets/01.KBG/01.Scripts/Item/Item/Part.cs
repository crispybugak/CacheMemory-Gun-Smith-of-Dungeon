using UnityEngine;
using UnityEngine.Serialization;

namespace  KBG.Item
{
    [CreateAssetMenu(fileName = "Item", menuName = "SO/Item/Data")]
    public class Part : ScriptableObject
    {
        public PartData partData;
        public int durability;
        public IngredientType madeBy;
    }    
}

