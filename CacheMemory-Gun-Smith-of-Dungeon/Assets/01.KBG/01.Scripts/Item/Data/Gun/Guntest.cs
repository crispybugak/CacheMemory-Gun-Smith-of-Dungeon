using KBG.Item;
using UnityEngine;

public class Guntest : MonoBehaviour
{
    public GunData data;

    public Part part;
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            data.ChangePart(part);
            GunDataApplier.Instance.InitializeRenderer();
        }
    }
}
