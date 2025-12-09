using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MonsterListSO", menuName = "Dungeon/Monster List")]
public class MonsterListSO : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public GameObject prefab;  
        public float weight = 1f;  
    }

    [Header("리스트에 포함된 몬스터")]
    public List<Entry> entries = new();
}
