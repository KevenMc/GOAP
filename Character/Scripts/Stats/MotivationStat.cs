using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotivationStat : MonoBehaviour
{
    [Header("Motivation")]
    public MotivationStatName motivationStatName;
    public bool increase;

    public string statNameText;

    public float currentVal;
    public float triggerVal;

    public float incrementVal;
    public float priority;
    public float currentPriority;

    [Header("Inventory Management")]
    public bool isStorage;
    public bool hasInventory;
    public InventoryData inventoryData;
    public ItemData itemData;
    public float quantity;

}
