using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KBG.Item
{
    [CreateAssetMenu(menuName = "SO/Item/Bullet")]
    public class BulletData : ScriptableObject, IItemData
    {
        [Serializable]
        public class PowderStatus
        {
            public IngredientType ingredientType;
            public float extraDamageRate;
        }
        
        [field: SerializeField] public Sprite icon { get; set; }
        [field:SerializeField] public float upScaling { get; set; }
        [field: SerializeField] public string itemName { get; set; }
        [field: SerializeField] public float mass { get; set; }
        
        [field: SerializeField] public GameObject BulletPrefab{ get; private set; }
        [field: SerializeField] public GameObject CasingPrefab{ get; private set; }
        [Header("Damage")]
        public float defaultDamage;
        public float damageRatePerPowder;
        public List<PowderStatus> powderStatuses;
        [Header("Strain")]
        public float defaultStrain;
        public float strainRatePerPowder;
    }
}
