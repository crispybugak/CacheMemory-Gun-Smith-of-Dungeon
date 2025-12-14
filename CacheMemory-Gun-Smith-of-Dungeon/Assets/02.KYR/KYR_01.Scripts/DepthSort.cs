using UnityEngine;

public class DepthSort : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    
    public float sortingMultiplier = 100f; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            enabled = false;
        }
    }

    void LateUpdate()
    { 
        int newOrder = Mathf.RoundToInt(-transform.position.y * sortingMultiplier);
        
        spriteRenderer.sortingOrder = newOrder;
    }
}