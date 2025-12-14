using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    [Header("프리팹당 초기 생성 개수")]
    [SerializeField] private int initialSizePerPrefab = 10;

    private class Pool
    {
        public Queue<Projectile> queue = new Queue<Projectile>();
    }

    private readonly Dictionary<GameObject, Pool> pools = new Dictionary<GameObject, Pool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private Pool GetOrCreatePool(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out var pool))
        {
            pool = new Pool();
            pools[prefab] = pool;

            for (int i = 0; i < initialSizePerPrefab; i++)
            {
                var proj = CreateNewInstance(prefab);
                if (proj != null)
                    pool.queue.Enqueue(proj);
            }
        }
        return pool;
    }

    private Projectile CreateNewInstance(GameObject prefab)
    {
        GameObject go = Instantiate(prefab, transform);
        Projectile proj = go.GetComponent<Projectile>();
        if (proj == null)
        {
            Debug.LogError($"{prefab.name} 에 Projectile 컴포넌트가 없습니다");
            return null;
        }

        proj.SetPool(this, prefab);
        go.SetActive(false);
        return proj;
    }

    public Projectile Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var pool = GetOrCreatePool(prefab);

        if (pool.queue.Count == 0)
        {
            var extra = CreateNewInstance(prefab);
            if (extra != null)
                pool.queue.Enqueue(extra);
        }

        var proj = pool.queue.Dequeue();
        proj.transform.SetPositionAndRotation(position, rotation);
        proj.gameObject.SetActive(true);
        return proj;
    }

    public void Return(Projectile projectile, GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out var pool))
        {
            pool = new Pool();
            pools[prefab] = pool;
        }

        projectile.gameObject.SetActive(false);
        pool.queue.Enqueue(projectile);
    }
}
