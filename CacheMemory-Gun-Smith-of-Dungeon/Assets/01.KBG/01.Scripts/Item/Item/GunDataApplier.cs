using System;
using System.Collections.Generic;
using UnityEngine;

namespace KBG.Item
{
    [DefaultExecutionOrder(-100)]
    public class GunDataApplier : MonoSingleton<GunDataApplier>
    {
        [SerializeField] private SpriteRenderer muzzleRenderer,
            barrelRenderer,
            frameRenderer,
            stockRenderer,
            magazineRenderer,
            sightRenderer,
            gripRenderer;

        
        [Header("Data")]
        [field: SerializeField] public GunDefaultData defaultData {get; private set;}
        [field: SerializeField] public GunData gunStatusData{get; private set;}

        protected override void Awake()
        {
            base.Awake();
            InitializeRenderer(); 
            gunStatusData.Initialize();
        }

        public void InitializeRenderer()
        {
            var part = gunStatusData.GetPart(PartType.Muzzle);
            muzzleRenderer.sprite = part ? part.partData.icon : null;
            
            part = gunStatusData.GetPart(PartType.Barrel);
            if (part)
            {
                muzzleRenderer.transform.localPosition = part.partData.localPos;
                gripRenderer.transform.localPosition = part.partData.localPos;
                barrelRenderer.sprite = part.partData.icon;
            }
            else
                barrelRenderer.sprite = null;

            part = gunStatusData.GetPart(PartType.Base);
            frameRenderer.sprite = part ? part.partData.icon : null;
            
            part = gunStatusData.GetPart(PartType.Stock);
            stockRenderer.sprite = part ? part.partData.icon : null;
            
            part = gunStatusData.GetPart(PartType.Magazine);
            if (part)
            {
                magazineRenderer.transform.localRotation = Quaternion.Euler(0, 0, part.partData.partDegree);
                magazineRenderer.transform.localPosition = part.partData.localPos;
                magazineRenderer.sprite = part.partData.icon;
            }
            else
            {
                magazineRenderer.transform.localRotation = Quaternion.identity;
                magazineRenderer.sprite = null;
            }
            
            part = gunStatusData.GetPart(PartType.Sight);
            sightRenderer.sprite = part ? part.partData.icon : null;
            
            part = gunStatusData.GetPart(PartType.Grip);
            gripRenderer.sprite = part ? part.partData.icon : null;
        }
    }
}
