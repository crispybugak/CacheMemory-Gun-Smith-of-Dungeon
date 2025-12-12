using System.Collections.Generic;
using UnityEngine;

public class CorridorStretch : MonoBehaviour
{
    [SerializeField] private bool isHorizontal = true; //세로 false        
    [SerializeField] private List<SpriteRenderer> renderers;   

    public void SetBetween(Vector3 doorA, Vector3 doorB)
    {
        Vector3 mid = (doorA + doorB) * 0.5f;
        transform.position = mid;
        
        float length = isHorizontal
            ? Mathf.Abs(doorB.x - doorA.x)
            : Mathf.Abs(doorB.y - doorA.y);
        
        foreach (var sr in renderers)
        {
            if (sr == null) continue;

            Vector2 size = sr.size;
            if (isHorizontal)
                size.x = length;
            else
                size.y = length;

            sr.size = size;
        }
    }
}