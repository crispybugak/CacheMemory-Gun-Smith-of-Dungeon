using System;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [CreateAssetMenu(menuName = "SO/Item/GunData")]
    public class GunDataSO : ScriptableObject
    {
        [Header("Status")] [Range(0,150)]
        public float Damage;
        [Range(0, 100)] public float RecoilControl, Accuracy, HandleSpeed, MoveSpeed, Magazine = 0;
        
        [Header("Ammo")]
        public BulletDataSO chamber;
        private Stack<BulletDataSO> magazine = new Stack<BulletDataSO>();
        
        private PartSO magazinePart;

        private Dictionary<PartType, PartSO> mountedParts = new Dictionary<PartType, PartSO>();

        private void OnEnable()
        {
            for (int i = 0; i < 7; i++)
                mountedParts[(PartType)i] = null;
        }

        public PartSO MountPart(PartSO part)
        {
            if (mountedParts[part.type] != null)
            {
                var mountedPart = mountedParts[part.type];
                mountedParts[part.type] = part;
                return mountedPart;
            }
            mountedParts[part.type] = part;
            InitState();
            return null;
        }

        public PartSO RemovePart(PartType type)
        {
            if (mountedParts[type] == null) return null;
            var mountedPart = mountedParts[type];
            mountedParts[type] = null;
            InitState();
            return mountedPart;
        }

        public PartSO GetPart(PartType type)
        {
            return mountedParts[type] == null ? null : mountedParts[type];
        }

        public void InitState()
        {
            Damage = 0;
            RecoilControl = 0;
            Accuracy = 0;
            HandleSpeed = 0;
            MoveSpeed = 0;
            Magazine = 0;
            foreach (var mountedPart in mountedParts.Values)
            {
                foreach (var effect in mountedPart.effects)
                {
                    switch (effect.type)
                    {
                        case ItemEffectType.Damage:
                            Damage += effect.effectAmount;
                            break;
                        case ItemEffectType.RecoilControl:
                            RecoilControl += effect.effectAmount;
                            break;
                        case ItemEffectType.Accuracy:
                            Accuracy += effect.effectAmount;
                            break;
                        case ItemEffectType.HandleSpeed:
                            HandleSpeed += effect.effectAmount;
                            break;
                        case ItemEffectType.MoveSpeed:
                            MoveSpeed += effect.effectAmount;
                            break;
                        case ItemEffectType.Magazine:
                            Magazine += effect.effectAmount;
                            break;
                    }
                }
            }
        }
    }
}
