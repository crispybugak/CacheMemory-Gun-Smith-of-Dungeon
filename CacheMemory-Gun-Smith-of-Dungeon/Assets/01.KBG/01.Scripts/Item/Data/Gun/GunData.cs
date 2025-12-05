using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace KBG.Item
{
    [CreateAssetMenu(menuName = "SO/Item/Gun")]
    public class GunData :  ScriptableObject
    {
        [Header("Parts")]
        [SerializeField] private List<Part> parts = new List<Part>();
        private Dictionary<PartType, Part> partsDict = new Dictionary<PartType, Part>();
        [Header("Status")]
        [Range(0, 150)] public float damage;

        [Range(0, 100)] public float
            recoilControl,
            accuracy,
            range,
            handleSpeed;
        public float capacity;
        
        public BulletItem Chamber { get; private set; }
        private Stack<BulletItem> _magazine = new Stack<BulletItem>();

        public void Initialize()
        {
            
            partsDict.Clear();
            
            foreach (var part in Enum.GetValues(typeof(PartType)).Cast<PartType>())
                partsDict.Add(part, null);
            InitializeStatus();
        }

        public Part ChangePart([NotNull] Part part)
        {
            if (!partsDict[part.partData.requirePartType])
                return part;
            Part temp = null;

            (partsDict[part.partData.type], temp) = (part, partsDict[part.partData.type]);

            InitializeStatus();
            return temp;
        }

        public Part RemovePart(PartType partType)
        {
            Part temp = partsDict[partType];
            if (!temp) return null;
            InitializeStatus();
            return temp;
        }

        public Part GetPart(PartType partType)
        {
            return partsDict[partType];
        }

        public BulletItem ShootBullet()
        {
            var temp = Chamber;
            ReloadChamber();
            return temp;
        }

        public bool ReloadChamber()
        {
            if (_magazine.Count <= 0) return false;
            Chamber = _magazine.Pop();
            return true;
        }

        public bool Reload(BulletItem bullet)
        {
            if (!(_magazine.Count < capacity)) return false;
            _magazine.Push(Chamber);
            return true;
        }

        private void InitializeStatus()
        {
            damage = 0;
            recoilControl = 0;
            accuracy = 0;
            range = 0;
            handleSpeed = 0;
            capacity = 0;
            
            foreach (var effect in partsDict.Values
                         .SelectMany(part => part.partData.ingredients
                             .Where(ingredient => ingredient.requiredIngredient == part.madeBy)
                             .SelectMany(ingredient => ingredient.effects)))
            {
                switch (effect.effectType)
                {
                    case PartEffectType.Damage:
                        damage = Mathf.Clamp(damage + effect.effectAmount, 0 , 150);
                        break;
                    case PartEffectType.RecoilControl:
                        recoilControl =  Mathf.Clamp(recoilControl + effect.effectAmount, 0 , 100);
                        break;
                    case PartEffectType.Accuracy:
                        accuracy =  Mathf.Clamp(accuracy + effect.effectAmount, 0 , 100);
                        break;
                    case PartEffectType.Range:
                        range =   Mathf.Clamp(range + effect.effectAmount, 0 , 100);
                        break;
                    case PartEffectType.HandleSpeed:
                        handleSpeed =   Mathf.Clamp(handleSpeed + effect.effectAmount, 0 , 100);
                        break;
                    case PartEffectType.Capacity:
                        capacity = Mathf.Clamp(capacity + effect.effectAmount, 0 , 100);
                        break;
                }
            }
        }
    }
}
