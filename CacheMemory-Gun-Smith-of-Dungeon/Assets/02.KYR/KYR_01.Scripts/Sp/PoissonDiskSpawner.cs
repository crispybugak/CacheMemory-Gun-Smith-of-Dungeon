using System.Collections.Generic;
using UnityEngine;

public class PoissonDiskSpawner : MonoBehaviour
{
    [Header("포아송 디스크 샘플링 영역")]
    public Vector2 areaSize = new(10, 10); // 방 안 사용 영역 
    public float r = 1f;                   // 최소 간격
    public int k = 30;                     // 시도 횟수(클수록 더 촘촘히 탐색)
    
    public List<Vector3> GetPositions(int desiredCount)
    {
        var result = new List<Vector3>();
        var pts = Generate(areaSize, r, k, desiredCount);

        foreach (var p in pts)
        {
            
            Vector3 world = (Vector2)transform.position + p - areaSize * 0.5f;
            result.Add(world);
        }

        return result;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(areaSize.x, areaSize.y, 0);
        Gizmos.DrawWireCube(center, size);
    }
    

    List<Vector2> Generate(Vector2 size, float minDist, int kk, int desired)
    {
        float cell = minDist / Mathf.Sqrt(2f);
        int gw = Mathf.CeilToInt(size.x / cell);
        int gh = Mathf.CeilToInt(size.y / cell);

        int[,] grid = new int[gw, gh];
        for (int x = 0; x < gw; x++)
        for (int y = 0; y < gh; y++)
            grid[x, y] = -1;

        var samples = new List<Vector2>();
        var active  = new List<int>();

        void AddSample(Vector2 p)
        {
            samples.Add(p);
            int id = samples.Count - 1;
            int gx = Mathf.FloorToInt(p.x / cell);
            int gy = Mathf.FloorToInt(p.y / cell);
            grid[gx, gy] = id;
            active.Add(id);
        }

        bool InBounds(Vector2 p) => p.x >= 0 && p.y >= 0 && p.x < size.x && p.y < size.y;

        bool FarEnough(Vector2 q)
        {
            int gx = Mathf.FloorToInt(q.x / cell);
            int gy = Mathf.FloorToInt(q.y / cell);
            float r2 = minDist * minDist;

            for (int y = Mathf.Max(0, gy - 2); y <= Mathf.Min(gh - 1, gy + 2); y++)
            for (int x = Mathf.Max(0, gx - 2); x <= Mathf.Min(gw - 1, gx + 2); x++)
            {
                int id = grid[x, y];
                if (id == -1) continue;

                Vector2 d = samples[id] - q;
                float d2 = d.x * d.x + d.y * d.y;
                if (d2 < r2) return false;
            }
            return true;
        }
        AddSample(new Vector2(Random.value * size.x, Random.value * size.y));

        while (active.Count > 0 && samples.Count < desired)
        {
            int aidx = active[Random.Range(0, active.Count)];
            Vector2 a = samples[aidx];
            bool found = false;

            for (int i = 0; i < kk; i++)
            {
                float theta = Random.value * Mathf.PI * 2f;
                float rho   = minDist * (1f + Random.value); // [r, 2r)
                Vector2 q   = a + new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * rho;

                if (InBounds(q) && FarEnough(q))
                {
                    AddSample(q);
                    found = true;
                    if (samples.Count >= desired) break;
                }
            }

            if (!found) active.Remove(aidx);
        }

        return samples;
    }
}
