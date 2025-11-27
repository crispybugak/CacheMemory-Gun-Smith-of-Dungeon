using UnityEngine;

[CreateAssetMenu(fileName = "AgentInfoSO", menuName = "SO/AgentInfoSO")]
public class AgentInfoSO : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }


}
