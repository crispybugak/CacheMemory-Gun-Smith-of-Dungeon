using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "SO/Health")]
public class HealthDataSO : ScriptableObject
{
    [field: SerializeField] public float health { get;  set; }

    [Header("Self-healing")]
    private float _healingdelay { get; set; } = 5f;
    private float _healAmount { get; set; }
    private float _currentTime { get; set; }
    private bool _isHealing { get; set; }
}
