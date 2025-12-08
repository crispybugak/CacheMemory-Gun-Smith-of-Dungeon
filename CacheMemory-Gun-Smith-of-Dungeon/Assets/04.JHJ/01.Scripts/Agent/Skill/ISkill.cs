using System.Collections;
using UnityEngine;

public interface ISkill
{
    float CoolTime { get; }
    
    IEnumerator UseSkill(GameObject owner);
}
