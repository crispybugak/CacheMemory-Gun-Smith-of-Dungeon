using System;
using System.Collections.Generic;
using KBG.Item;
using UnityEngine;

namespace KBG.Inventory
{
    public class FillDefaultItem : MonoBehaviour
    {
        [SerializeField]List<IItem> items = new List<IItem>();

        private void Start()
        {
            foreach (var item in items)
            {
                Inventory.Instance.AddItem(Instantiate(item));
            }
        }
    }
}