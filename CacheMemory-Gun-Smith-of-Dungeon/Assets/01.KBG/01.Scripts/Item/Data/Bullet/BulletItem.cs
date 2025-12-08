using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KBG.Item
{
    [CreateAssetMenu(menuName = "SO/Item/BulletItem")]
    public class BulletItem : IItem
    {
        public BulletData bulletData;
        public override IItemData ItemData
        {
            get { return bulletData;}
            set { bulletData = value as BulletData; }
        }
        [field: SerializeField] public float Damage { get; private set; }
        [Header("Ingredient")] public IngredientType usedIngredientType;
        public float usedAmount;

        public void Initialize()
        {
            Damage = bulletData.powderStatuses.Find(powder => powder.ingredientType == usedIngredientType).extraDamageRate *  usedAmount * bulletData.damageRatePerPowder;
        }
    }
}
