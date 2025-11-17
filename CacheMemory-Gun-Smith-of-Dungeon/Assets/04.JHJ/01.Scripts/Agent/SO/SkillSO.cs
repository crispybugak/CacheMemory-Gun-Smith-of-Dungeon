using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "SO/SkillSO")]
public class SkillSO : ScriptableObject
{
    [SerializeField] private int damage;
    [SerializeField] private float coolTime;
    [SerializeField] private Sprite skillIcon;
    [SerializeField] private string animationName;

    public int Damage => damage;
    public float CoolTime => coolTime;
    public Sprite SkillIcon => skillIcon;
    public string AnimationName => animationName;

}
