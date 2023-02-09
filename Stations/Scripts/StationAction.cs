using UnityEngine;
using System.Collections.Generic;




[CreateAssetMenu(fileName = "StationAction", menuName = "Framework/Stations/StationAction", order = 0)]
public class StationAction : ScriptableObject
{



    [Header("Return Item")]
    public bool returnsItem;
    public List<ItemData> returnItems;

    [Header("Requires Single Item")]
    public bool requiresItem;
    public bool destroyItemOnUse;
    public bool damagesItem;
    public float itemDamageVal;
    public ItemData requiredItem;
    public ItemType neededItemType;

    // [Header("Recipe")]
    // public bool hasRecipe;
    // public Recipe recipe;

    [Header("Stats")]
    public MotivationStatName motivationStatName;
    public float motivationStatVal;
}