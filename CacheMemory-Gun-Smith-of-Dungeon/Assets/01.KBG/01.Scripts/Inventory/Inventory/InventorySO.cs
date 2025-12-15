using System.Collections.Generic;
using KBG.Item;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Inventory")]
public class InventorySO : ScriptableObject
{
    public List<IItem>  items = new List<IItem>();
    
    
}
