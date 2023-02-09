using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
[RequireComponent(typeof(InventoryHandler))]
public class ItemHandler : MonoBehaviour
{
    public Agent agent;
    public InventoryHandler inventoryHandler;
    public List<Item> pickupItems = new List<Item>();

    public void Init(Agent agent)
    {
        this.agent = agent;
        inventoryHandler = agent.inventoryHandler;
    }


    public void UseItem(StatHandler stats, ItemData itemData)
    {
        if(!inventoryHandler.HasItem(itemData)) return;
        UpdateMotivationStats(stats.motivationStats, itemData);
        Transmute(itemData);
        DestroyItem(itemData);
    }

    public void PickUpItem(Item item)
    {
        if(pickupItems.Contains(item)) inventoryHandler.PickUpItem(item);
    }

    public void AddPickupItem(Item item)
    {
        pickupItems.Add(item);
    }

    public void RemovePickupItem(Item item)
    {
        pickupItems.Remove(item);
    }


    public void UpdateMotivationStats(List<MotivationStat> motivationStats, ItemData itemData)
    {
        if (itemData != null && itemData.motivationStatName != MotivationStatName.NULL)
        {
            foreach (MotivationStat mstat in motivationStats)
            {
                if (mstat.motivationStatName == itemData.motivationStatName)
                {
                    mstat.currentVal -= itemData.motivationStatVal;
                }
            }
        }
    }


    public void Transmute(ItemData itemData)
    {
        if (itemData != null && itemData.transmutes)
        {
            List<ItemData> inv = inventoryHandler.inventory.items;
            int idx = inv.FindLastIndex(x => x == itemData);
            inv.RemoveAt(idx);
            inv.Insert(idx, itemData.transmutesTo);
        }
    }


    public void DestroyItem(ItemData itemData)
    {
        if (itemData != null && itemData.destroyOnUse)
        {
            List<ItemData> inv = inventoryHandler.inventory.items;
            int idx = inv.FindIndex(x => x == itemData);
            inv.RemoveAt(idx);
        }
    }
}
