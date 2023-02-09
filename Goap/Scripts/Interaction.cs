using UnityEngine;
using System;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "Recipe", menuName = "Framework/Recipes/Recipe", order = 0)]
public class Interaction : ScriptableObject
{
    [Serializable]
    public struct RecipeItem
    {
        public ItemData itemData;
    }

    [Serializable]
    public struct ReturnItem
    {
        public ItemData returnItem;
        public int quantity;
    }

    [Serializable]
    public struct Storage
    {
        public InventoryData inventory;
        public ItemData itemData;
        public float quantity;
        public MotivationStatName motivationStatName;
    }

    [Header("Basic Info")]
    public InteractionType interactionType;
    public string recipeName;
    public float interactionCost;
    public bool requiresStation;
    public StationType stationType;
    public List<ItemData> tools;

    [Header("Storage info")]
    public bool isStorage;
    public Storage storage;

    [Header("Recipe Info")]
    public bool isRecipe;
    public List<RecipeItem> recipe;
    public List<ReturnItem> returnItems;
    public ItemData product;
}