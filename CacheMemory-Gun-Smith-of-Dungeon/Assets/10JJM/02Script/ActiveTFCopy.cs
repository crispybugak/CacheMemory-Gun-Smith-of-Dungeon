using KBG.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class ActiveTFCopy : MonoBehaviour
{
    private Image _image;

    private void Awake()
    {
        _image = transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        bool act = Inventory.Instance.Active;
        _image.raycastTarget = act;
        _image.enabled = act;
    }
}
