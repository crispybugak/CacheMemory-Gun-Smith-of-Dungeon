using System.Collections;
using UnityEngine;
using Skill;

public class CharacterSkillSet : MonoBehaviour, ICharacterSet
{
    [Header("참조")]
    [SerializeField] private Agent agent;

    [Header("스킬")]
    [SerializeField] private SkillSO skillData;

    private ISkill _skill1;
    private bool _isCastingSkill1;
    private float _lastSkill1UseTime;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<Agent>();

        if (skillData is ISkill s)
            _skill1 = s;
    }

    private void OnEnable()
    {
        if (agent != null && agent.MovementSOCompo != null)
            agent.MovementSOCompo.OnSkillPressed += UseSkill;
    }

    private void OnDisable()
    {
        if (agent != null && agent.MovementSOCompo != null)
            agent.MovementSOCompo.OnSkillPressed -= UseSkill;
    }

    // ICharacterSet 구현 - 스킬 사용
    public void UseSkill()
    {
        if (_skill1 == null || agent == null) return;
        if (_isCastingSkill1) return;

        if (Time.time < _lastSkill1UseTime + _skill1.CoolTime)
            return;

        StartCoroutine(UseSkill1Coroutine());
    }

    private IEnumerator UseSkill1Coroutine()
    {
        _isCastingSkill1 = true;
        _lastSkill1UseTime = Time.time;

        yield return _skill1.UseSkill(agent.gameObject);

        _isCastingSkill1 = false;
    }

    public void Passive()
    {
    }
}