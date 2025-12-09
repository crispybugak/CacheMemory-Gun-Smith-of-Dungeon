using UnityEngine;

namespace _06.SDW._01.Scripts.SO
{
    public abstract class PassiveSO : ScriptableObject
    {
        // target는 이 패시브를 적용할 캐릭터(플레이어) 오브젝트
        public abstract void Apply(GameObject target);

        // 필요하면 나중에 해제할 때 사용 (안 쓰면 비워 둬도 됨)
        public virtual void Remove(GameObject target) { }
    }
}