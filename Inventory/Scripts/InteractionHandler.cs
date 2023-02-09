using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class InteractionHandler : MonoBehaviour
{
    public Agent agent;
    public InventoryHandler inventoryHandler;


    public void Init(Agent agent)
    {
        this.agent = agent;
        inventoryHandler = agent.inventoryHandler;
    }


    public void MakeRecipe(Interaction recipe)
    {
        foreach (Interaction.RecipeItem recipeItem in recipe.recipe) if (!inventoryHandler.HasItem(recipeItem.itemData)) return;
        
        foreach (Interaction.RecipeItem recipeItem in recipe.recipe) inventoryHandler.DestroyItem(recipeItem.itemData);

        foreach (Interaction.ReturnItem returnItem in recipe.returnItems)
        {
            for (int i = 0; i < returnItem.quantity; i++) inventoryHandler.AddItem(returnItem.returnItem);
        }
    }


    public void StoreItemsAtStation(Interaction interaction, Station station)
    {
        if (!station.canUse) return;

        station.Interact(interaction);
        if (station.inventoryData == interaction.storage.inventory)
        {
            for (int i = 0; i < interaction.storage.quantity; i++)
            {
                inventoryHandler.DestroyItem(interaction.storage.itemData);
                station.inventoryData.items.Add(interaction.storage.itemData);
            }
        }
    }


    public void RetrieveItems(Interaction interaction, Station station)
    {
        if (!station.canUse) return;

        station.Interact(interaction);
        if (inventoryHandler.HasItem(interaction.product, station.inventoryData.items))
        {
            inventoryHandler.DestroyItem(interaction.product, station.inventoryData.items);
            inventoryHandler.AddItem(interaction.product);
        }
    }

    
    public void MakeRecipeAtStation(Interaction interaction, Station station)
    {
        if (!station.canUse) return;

        station.Interact(interaction);
        foreach (Interaction.RecipeItem recipeItem in interaction.recipe)
        {
            if (!inventoryHandler.HasItem(recipeItem.itemData)) return;
            inventoryHandler.DestroyItem(recipeItem.itemData);
        }

        foreach (Interaction.ReturnItem returnItem in interaction.returnItems)
        {
            for (int i = 0; i < returnItem.quantity; i++) inventoryHandler.AddItem(returnItem.returnItem);
        }
    }
}
