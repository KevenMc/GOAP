using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Framework/Inventory/Inventory", order = 0)]
public class InventoryData : ScriptableObject {
    public List<ItemData> items;
}