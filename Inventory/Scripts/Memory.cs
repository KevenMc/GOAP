using UnityEngine;
using System.Collections.Generic;



public class Memory : MonoBehaviour
{
    public InventoryData allItems;
    public InventoryData inventoryData;
    public List<Item> knownItems;


    public List<Interaction> knownInteractions;
    public List<Station> knownStations;


    private void FixedUpdate()
    {
        knownItems.RemoveAll(x => x == null);
    }


    #region INTERACTIONS
    public List<Interaction> FindInteractions(ItemData itemData)
    {
        return new List<Interaction>(knownInteractions.FindAll(x => x.product == itemData));
    }

    public bool CanPerformRecipeFromInventory(Interaction recipe)
    {
        foreach (Interaction.RecipeItem recipeItem in recipe.recipe)
        {
            if(!HasItem(recipeItem.itemData)) return false;
        }

        return true;
    }


    public List<Interaction> FindRecipesByMotivation(MotivationStatName motivationStatName)
    {
        List<Interaction> recipes = new List<Interaction>(knownInteractions.FindAll(x => x.interactionType == InteractionType.RECIPE));
        List<Interaction> recipeList =  new List<Interaction>(recipes.FindAll(x => x.product.motivationStatName == motivationStatName));

        List<Interaction> storages = new List<Interaction>(knownInteractions.FindAll(x => x.interactionType == InteractionType.STORE_ITEM_AT_STATION));

        List<Interaction> storageList = new List<Interaction>(storages.FindAll(x => x.storage.motivationStatName == motivationStatName));

        storageList.AddRange(recipeList);
        return storageList;
    }
    #endregion

    #region ITEMS
    public bool HasItem(ItemData itemData, List<Item> checkItems = null)
    {
        if (checkItems == null) checkItems = knownItems;
        return checkItems.Find(x => x.itemData == itemData) != null;
    }


    public List<Item> FindItemsByItemData(ItemData itemData, List<Item> checkItems = null)
    {
        if (checkItems == null) checkItems = knownItems;

        return new List<Item>(checkItems.FindAll(x => x.itemData == itemData));
    }


    public Item FindItemByItemData(ItemData itemData, List<Item> checkItems = null)
    {
        if (checkItems == null) checkItems = knownItems;

        return checkItems.Find(x => x.itemData == itemData);
    }


    public List<Item> FindItemsByItemType(ItemType itemType, List<Item> checkItems = null)
    {
        if (checkItems == null) checkItems = knownItems;

        return new List<Item>(checkItems.FindAll(x => x.itemData.itemType == itemType));
    }


    public List<Item> FindItemsByItemMotivationStatName(MotivationStatName motivationStatName, List<Item> checkItems = null)
    {
        if (checkItems == null) checkItems = knownItems;

        return new List<Item>(checkItems.FindAll(x => x.itemData.motivationStatName == motivationStatName));
    }


    public List<Item> FindItemsByTransmuteItemData(ItemData itemData, List<Item> checkItems = null)
    {
        if (checkItems == null) checkItems = knownItems;

        List<Item> transmutable = new List<Item>(checkItems.FindAll(x => x.itemData.transmutes));
        return new List<Item>(transmutable.FindAll(x => x.itemData.transmutesTo == itemData));
    }


    public List<Item> FindItemsByTransmuteItemType(ItemType itemType, List<Item> checkItems = null)
    {
        if (checkItems == null) checkItems = knownItems;

        List<Item> transmutable = new List<Item>(checkItems.FindAll(x => x.itemData.transmutes));
        return new List<Item>(transmutable.FindAll(x => x.itemData.transmutesTo.itemType == itemType));
    }


    public List<Item> FindItemsByTransmuteItemMotivationStatName(MotivationStatName motivationStatName, List<Item> checkItems = null)
    {
        if (checkItems == null) checkItems = knownItems;

        List<Item> transmutable = new List<Item>(checkItems.FindAll(x => x.itemData.transmutes));
        return new List<Item>(transmutable.FindAll(x => x.itemData.transmutesTo.motivationStatName == motivationStatName));
    }
    #endregion


    #region ALLITEMS
    public List<ItemData> AllItemsByItemType(ItemType itemType, List<ItemData> checkItems = null)
    {
        if (checkItems == null) checkItems = allItems.items;

        return new List<ItemData>(checkItems.FindAll(x => x.itemType == itemType));
    }


    public List<ItemData> AllItemsByItemMotivationStatName(MotivationStatName motivationStatName, List<ItemData> checkItems = null)
    {
        if (checkItems == null) checkItems = allItems.items;

        return new List<ItemData>(checkItems.FindAll(x => x.motivationStatName == motivationStatName));
    }


    public List<ItemData> AllItemsByTransmuteItemData(ItemData itemData, List<ItemData> checkItems = null)
    {
        if (checkItems == null) checkItems = allItems.items;

        List<ItemData> transmutable = new List<ItemData>(checkItems.FindAll(x => x.transmutes));
        return new List<ItemData>(transmutable.FindAll(x => x.transmutesTo == itemData));
    }


    public List<ItemData> AllItemsByTransmuteItemType(ItemType itemType, List<ItemData> checkItems = null)
    {
        if (checkItems == null) checkItems = allItems.items;

        List<ItemData> transmutable = new List<ItemData>(checkItems.FindAll(x => x.transmutes));
        return new List<ItemData>(transmutable.FindAll(x => x.transmutesTo.itemType == itemType));
    }


    public List<ItemData> AllItemsByTransmuteItemMotivationStatName(MotivationStatName motivationStatName, List<ItemData> checkItems = null)
    {
        if (checkItems == null) checkItems = allItems.items;

        List<ItemData> transmutable = new List<ItemData>(checkItems.FindAll(x => x.transmutes));
        return new List<ItemData>(transmutable.FindAll(x => x.transmutesTo.motivationStatName == motivationStatName));
    }
    #endregion


    #region STATIONS



    public List<Station> GetStationByActionItem(ItemData itemData)
    {
        List<Station> stations = new List<Station>();

        foreach (Station station in knownStations)
        {
            if (station.HasActionItem(itemData)) stations.Add(station);
        }

        return stations;
    }


    public Station GetStationByItem(ItemData itemData)
    {
        List<Station> stations = knownStations.FindAll(x => x.stationActions.Find(y => y == itemData));
        return stations.Find(x => x.inUse == false);
    }


    public List<Station> GetStationsByItemData(ItemData itemData)
    {
        List<Station> stations = knownStations.FindAll(x => x.stationActions.Find(y => y == itemData));
        return stations.FindAll(x => x.inUse == false);
    }


    public StationAction GetStationActionByItem(ItemData itemData, Station station)
    {
        return station.stationActions.Find(x => x == itemData);
    }


    public List<Station> GetStationsByType(StationType stationType)
    {
        return new List<Station>(knownStations.FindAll(x => x.stationType.Contains(stationType)));
    }


    public Station GetStationByStat(MotivationStatName motivationStatName)
    {
        List<Station> stations = knownStations.FindAll(x => x.stationActions.Find(y => y.motivationStatName == motivationStatName));
        return stations.Find(x => x.inUse == false);
    }


    public StationAction GetStationActionByStat(MotivationStatName motivationStatName, Station station)
    {
        return station.stationActions.Find(x => x.motivationStatName == motivationStatName);
    }
    #endregion
}