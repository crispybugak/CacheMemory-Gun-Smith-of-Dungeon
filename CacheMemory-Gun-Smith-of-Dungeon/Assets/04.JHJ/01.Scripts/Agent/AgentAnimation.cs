using UnityEngine;

public class AgentAnimation : MonoBehaviour
{
    public Animator AnimComp { get; private set; }
    private readonly int WalkHash = Animator.StringToHash("Walk");

    private void Awake()
    {
        AnimComp = GetComponent<Animator>();
    }
    public void SetWalkAnimation(bool value)
    {
        AnimComp.SetBool(WalkHash, value);
    }
    public void AnimatePlay(float velocity)
    {
        if (velocity > 0)
            SetWalkAnimation(true);
        else
            SetWalkAnimation(false);
    }
}
