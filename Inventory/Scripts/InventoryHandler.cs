using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class InventoryHandler : MonoBehaviour
{
    public InventoryData inventory;
    public Agent agent;


    public InventoryHandler(InventoryData inv)
    {
        inventory = inv;
    }


    public void Init(Agent agent)
    {
        this.agent = agent;
    }


    #region Interactions
    public void DestroyItem(ItemData itemData, List<ItemData> checkInv = null)
    {
        if (checkInv == null) checkInv = inventory.items;

        checkInv.Remove(itemData);
    }


    public void AddItem(ItemData itemData, List<ItemData> checkInv = null)
    {
        if (checkInv == null) checkInv = inventory.items;

        checkInv.Add(itemData);
    }


    public void PickUpItem(Item item)
    {
        inventory.items.Add(item.itemData);
        Destroy(item.gameObject);
    }
    #endregion


    public bool HasItem(ItemData itemData, List<ItemData> checkInv = null, float quantity = 1)
    {
        if (checkInv == null) checkInv = inventory.items;

        return (checkInv.FindAll(x => x == itemData).Count >= quantity);
    }


    public bool CanSatisfyRecipe(List<Interaction.RecipeItem> recipe, List<ItemData> checkInv = null)
    {
        if (checkInv == null) checkInv = inventory.items;
        bool satisfies = true;

        foreach(Interaction.RecipeItem recipeItem in recipe)
        {
            if (!HasItem(recipeItem.itemData, checkInv)) satisfies = false;
        }

        return satisfies;
    }


    public bool CanSatisfyStorage(Interaction.Storage storage, List<ItemData> checkInv = null)
    {
        if (checkInv == null) checkInv = inventory.items;
        return HasItem(storage.itemData, checkInv);

    }


    public List<ItemData> GetItemsByMotivationStatName(MotivationStatName stat, List<ItemData> checkInv = null)
    {
        if (checkInv == null) checkInv = inventory.items;

        return checkInv.FindAll(x => x.motivationStatName == stat);
    }


    public List<ItemData> FindItemsByTransmuteItemData(ItemData itemData, List<ItemData> checkInv = null)
    {
        if (checkInv == null) checkInv = inventory.items;

        List<ItemData> transmutable = checkInv.FindAll(x => x.transmutes == true);
        return transmutable.FindAll(x => x.transmutesTo == itemData);
    }
}
