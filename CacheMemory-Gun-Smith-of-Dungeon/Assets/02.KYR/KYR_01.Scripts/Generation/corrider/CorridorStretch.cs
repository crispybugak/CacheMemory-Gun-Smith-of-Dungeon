using System.Collections.Generic;
using UnityEngine;

public class CorridorStretch : MonoBehaviour
{
    [SerializeField] private bool isHorizontal = true;              // 세로 복도면 false
    [SerializeField] private List<SpriteRenderer> renderers;        
    [SerializeField] private List<BoxCollider2D> colliders;        

    public void SetBetween(Vector3 doorA, Vector3 doorB)
    {
        Vector3 mid = (doorA + doorB) * 0.5f;
        transform.position = mid;

        float length = isHorizontal
            ? Mathf.Abs(doorB.x - doorA.x)
            : Mathf.Abs(doorB.y - doorA.y);

        for (int i = 0; i < renderers.Count; i++)
        {
            var sr = renderers[i];
            if (sr == null) continue;

            var s = sr.size;
            if (isHorizontal)
                s.x = length;
            else
                s.y = length;
            sr.size = s;

            if (i < colliders.Count && colliders[i] != null)
            {
                var col = colliders[i];

                var colSize   = col.size;
                var colOffset = col.offset;

                if (isHorizontal)
                {
                    colSize.x   = length;
                    colOffset.x = 0f;     
                }
                else
                {
                    colSize.y   = length;
                    colOffset.y = 0f;
                }

                col.size   = colSize;
                col.offset = colOffset;
            }
        }
    }
}