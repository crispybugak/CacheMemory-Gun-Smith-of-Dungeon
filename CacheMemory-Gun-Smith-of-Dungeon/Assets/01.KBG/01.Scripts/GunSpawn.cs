using UnityEngine;

public class GunSpawn : MonoBehaviour
{
    public GameObject prefab;
    void Start()
    {
        Instantiate(prefab);
    }
}
