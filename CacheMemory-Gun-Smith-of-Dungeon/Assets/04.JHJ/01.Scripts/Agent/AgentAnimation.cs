using UnityEngine;

public class AgentAnimation : MonoBehaviour
{
    public Animator AnimComp { get; private set; }
    private readonly int WalkHash = Animator.StringToHash("Walk");
    private readonly int LeftWalkHash = Animator.StringToHash("Left");
    private readonly int RightWalkHash = Animator.StringToHash("Right");

    private void Awake()
    {
        AnimComp = GetComponent<Animator>();
    }
    public void SetWalkAnimation(bool value)
    {
        AnimComp.SetBool(WalkHash, value);
    }
    public void SetFlipAnimation(float value)
    {
        AnimComp.SetFloat(LeftWalkHash, value);
        AnimComp.SetFloat(RightWalkHash, value);
    }
    public void AgentRendererX(Vector2 input)
    {
        if (input.x < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (input.x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    public void AnimatePlay(float velocity)
    {
        if (velocity > 0)
            SetWalkAnimation(true);
        else
            SetWalkAnimation(false);
    }
}
