using _06.SDW._01.Scripts.Item;
using KBG.Inventory;
using UnityEngine;

public class ActSet : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    void Update()
    {
        if (MaterialInventory.Instance != null)
            Inventory.Instance.Active = MaterialInventory.Instance.gameObject.activeSelf;
    }
}
