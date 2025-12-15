using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KBG.Item
{
    [CreateAssetMenu(menuName = "SO/Item/Bullet")]
    public class BulletData : ItemDataBase
    {
        [Serializable]
        public class PowderStatus
        {
            public IngredientType ingredientType;
            public float extraDamageRate;
        }
        
        [field: SerializeField] public override Sprite icon { get; set; }
        [field:SerializeField] public override float upScaling { get; set; }
        [field: SerializeField] public override string itemName { get; set; }
        
        [field: SerializeField] public GameObject BulletPrefab{ get; private set; }
        [field: SerializeField] public GameObject CasingPrefab{ get; private set; }
        [Header("Damage")]
        public float defaultDamage;
        public float damageRatePerPowder;
        [Header("Strain")]
        public float defaultStrain;
        public float strainRatePerPowder;
    }
}
