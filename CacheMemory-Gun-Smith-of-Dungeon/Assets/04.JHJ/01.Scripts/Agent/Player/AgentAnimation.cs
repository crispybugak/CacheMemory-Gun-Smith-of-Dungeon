using UnityEngine;

public class AgentAnimation : MonoBehaviour
{
    public Animator AnimComp { get; private set; }

    private readonly int SpeedHash = Animator.StringToHash("CurrentSpeed");
    private readonly int DirXHash = Animator.StringToHash("X");

    private float _lastDirX = 1f; 

    private void Awake()
    {
        AnimComp = GetComponent<Animator>();
    }

    public void Animate(Vector2 input, float velocity)
    {
        if (input.x != 0)
            _lastDirX = Mathf.Sign(input.x);  

        AnimComp.SetFloat(DirXHash, _lastDirX);
        float speed01 = Mathf.Clamp01(velocity);      
        AnimComp.SetFloat(SpeedHash, speed01);
    }
}
