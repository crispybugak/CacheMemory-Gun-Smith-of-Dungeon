using KBG.Inventory;
using UnityEngine;

public class ActiveSortLayer : MonoBehaviour
{
    [SerializeField] private Canvas _canva;
    [SerializeField] private int _min = -1;
    [SerializeField] private int _max = 1;

    private void Update()
    {
        _canva.sortingOrder = Inventory.Instance.Active ? _min : _max;
    }
}
