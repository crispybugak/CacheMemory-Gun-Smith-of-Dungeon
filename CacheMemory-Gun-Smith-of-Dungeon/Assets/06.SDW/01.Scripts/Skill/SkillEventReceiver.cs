using System;
using UnityEngine;

namespace _06.SDW._01.Scripts.Skill
{
    public class SkillEffectReceiver : MonoBehaviour
    {
        // 스킬 로직(SO)에게 신호를 보내기 위한 액션
        public event Action OnHitSignal;

        // 1. 애니메이션 이벤트에서 이 함수를 부르세요 (Function: OnAnimEvent)
        public void OnAnimEvent()
        {
            OnHitSignal?.Invoke();
        }

        // 2. 애니메이션 끝나는 시점에 이벤트를 넣어 자동 파괴하거나, 
        //    그냥 Animator의 "Exit Time" 이후 별도 스크립트로 파괴해도 됩니다.
        public void OnAnimEnd()
        {
            Destroy(gameObject);
        }
    }
}