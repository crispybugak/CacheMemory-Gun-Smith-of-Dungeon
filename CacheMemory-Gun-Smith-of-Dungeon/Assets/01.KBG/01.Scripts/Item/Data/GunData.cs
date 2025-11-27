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
        [SerializeField] [Range(0, 150)] private float damage;
        [SerializeField] [Range(0, 100)] private float
            recoilControl,
            accuracy,
            handleSpeed,
            moveSpeed,
            capacity;

        public void Initialize()
        {
            damage = 0;
            recoilControl = 0;
            accuracy = 0;
            handleSpeed = 0;
            moveSpeed = 0;
            capacity = 0;
            
            partsDict.Clear();
            foreach (var effect in parts.Where(partData =>
                         partData.itemData.type != PartType.None &&
                         partsDict.TryAdd(partData.itemData.type, partData))) ;
            InitializeStatus();
        }

        public Part ChangePart([NotNull]Part part)
        {
            Part temp = null;
            if (partsDict[part.itemData.type] != null)
            {
                temp = partsDict[part.itemData.type];
            }

            partsDict[part.itemData.type] = part;
            
            InitializeStatus();
            return temp;
        }

        public Part RemovePart(PartType partType)
        {
            if (partsDict.Remove(partType, out Part temp))
            {
                InitializeStatus();
                return temp;
            }
            return null;
        }

        public Part GetPart(PartType partType)
        {
            return partsDict.GetValueOrDefault(partType);
        }

        private void InitializeStatus()
        {
            foreach (var effect in partsDict.Values.SelectMany(part => part.itemData.effects))
            {
                switch (effect.EffectType)
                {
                    case PartEffectType.Damage:
                        damage += effect.EffectAmount;
                        break;
                    case PartEffectType.RecoilControl:
                        recoilControl += effect.EffectAmount;
                        break;
                    case PartEffectType.Accuracy:
                        accuracy += effect.EffectAmount;
                        break;
                    case PartEffectType.HandleSpeed:
                        handleSpeed += effect.EffectAmount;
                        break;
                    case PartEffectType.MoveSpeed:
                        moveSpeed += effect.EffectAmount;
                        break;
                    case PartEffectType.Capacity:
                        capacity += effect.EffectAmount;
                        break;
                }
            }
        }
    }
}
