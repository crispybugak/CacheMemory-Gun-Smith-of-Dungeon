using UnityEngine;
using UnityEngine.Tilemaps;

public class WallDepthSorter : MonoBehaviour
{
    // Inspector에서 벽 구조물 TilemapRenderer를 직접 할당합니다.
    public TilemapRenderer wallTilemapRenderer;
    private SpriteRenderer playerRenderer;

    public int overlapOffset = 1;

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
    
        if (playerObject != null)
        {
            playerRenderer = playerObject.GetComponent<SpriteRenderer>();
        }
        
        if (wallTilemapRenderer == null)
        {
            enabled = false; 
        }
    }

    void LateUpdate()
    {
        if (wallTilemapRenderer == null) return; 

        if (playerRenderer == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerRenderer = playerObject.GetComponentInChildren<SpriteRenderer>();
            }
            
            if (playerRenderer == null) return; 
        }
        
    
        float playerY = playerRenderer.transform.position.y;
        float boundaryY = transform.position.y; 
    
        int currentPlayerOrder = playerRenderer.sortingOrder;

        if (playerY > boundaryY) 
        {
            wallTilemapRenderer.sortingOrder = currentPlayerOrder + overlapOffset;
        }
        else
        {
            wallTilemapRenderer.sortingOrder = currentPlayerOrder - overlapOffset;
        }
    }
}