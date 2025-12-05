using UnityEngine;

[System.Serializable]
public class PatrolPoint
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 1f;
    
    public bool IsValid() => pointA != null && pointB != null;
}