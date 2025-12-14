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
        // Player 태그를 가진 오브젝트를 찾습니다.
        GameObject playerObject = GameObject.FindWithTag("Player");
        
        if (playerObject != null)
        {
            playerRenderer = playerObject.GetComponent<SpriteRenderer>();
        }

        if (playerRenderer == null || wallTilemapRenderer == null)
        {
            enabled = false;
        }
    }

    void LateUpdate()
    {
        if (playerRenderer == null || wallTilemapRenderer == null) return;

        float playerY = playerRenderer.transform.position.y;
        float boundaryY = transform.position.y; // 이 오브젝트(경계선)의 Y 위치가 기준이 됩니다.
        
        int currentPlayerOrder = playerRenderer.sortingOrder;

        // 플레이어가 벽의 경계선(boundaryY)보다 Y 위치가 높으면 (벽 뒤로 들어가면),
        if (playerY > boundaryY) 
        {
            // 벽 타일맵이 플레이어를 가려야 함 (벽이 앞에)
            // 벽의 Order를 플레이어 Order보다 높게 설정합니다.
            wallTilemapRenderer.sortingOrder = currentPlayerOrder + overlapOffset;
        }
        else
        {
            // 플레이어가 벽의 경계선(boundaryY)보다 Y 위치가 낮으면 (벽 앞에 서면),
            // 플레이어가 벽을 가려야 함 (벽이 뒤에)
            // 벽의 Order를 플레이어 Order보다 낮게 설정합니다.
            wallTilemapRenderer.sortingOrder = currentPlayerOrder - overlapOffset;
        }
    }
}