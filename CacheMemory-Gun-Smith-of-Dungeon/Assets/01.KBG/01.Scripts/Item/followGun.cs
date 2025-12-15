using System;
using KBG.Item;
using UnityEngine;

public class FollowGun : MonoBehaviour
{
    public GameObject player;

    private void LateUpdate()
    {
        GunDataApplier.Instance.gameObject.transform.position = player.transform.position;
    }
}
