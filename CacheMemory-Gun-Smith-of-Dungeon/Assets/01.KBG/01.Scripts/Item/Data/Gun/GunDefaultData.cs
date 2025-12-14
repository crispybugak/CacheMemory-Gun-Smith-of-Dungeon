using UnityEngine;

namespace KBG.Item
{
    [CreateAssetMenu(menuName = "SO/Item/GunDefaultData")]
    public class GunDefaultData :  ScriptableObject
    {
        [Header("Spread")]
        [Range(0, 180)] public float minSpread;
        [Range(0, 180)] public float maxSpread;
        
        [Header("Rebound")]
        public float minRebound;
        public float maxRebound;
        
        [Header("Range")]
        public float minRange;
        public float maxRange;
        
        [Header("MoveSpeed")]
        public float maxMoveSpeed;
        public float minMoveSpeed;

        [Header("Extra Settings")] 
        public float fireRate;
        public float reloadTime;
    }
}