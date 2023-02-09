using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal
{
public GoalType goalType = GoalType.NULL;

    public MotivationStatName motivationStatName;
    public MotivationStat motivationStat;
  //  public bool motivationStatIncrease;

 //   public ItemType itemType = ItemType.NULL;
    public ItemData itemData;
    public Item pickUpItem;
    public Station station;
    public Station recipeStation;
    public Interaction interaction;
    public Transform targetTransform;


    // public Goal()
    // {}


    public Goal(MotivationStat motivationStat)
    {
        this.motivationStat = motivationStat;
        goalType = GoalType.MOTIVATION;
        motivationStatName = motivationStat.motivationStatName;
     //   motivationStatIncrease = motivationStat.increase;
    }


    public Goal(ItemData i, GoalType goalType)
    {
        this.goalType = goalType;
        itemData = i;
    }


    public Goal(Item i, GoalType goalType)
    {
        this.goalType = goalType;
        pickUpItem = i;
        targetTransform = i.transform;
    }


    public Goal(Action action, GoalType goalType)
    {
        this.goalType = goalType;
        if (action.targetTransform != null)
        {
            itemData = action.useItem;
           // itemType = action.useItemType;
            pickUpItem = action.pickUpItem;
            station = action.interactionStation;
            interaction = action.interaction;
        }
        else
        {
            targetTransform = action.targetTransform;
        }
    }


    public Goal(Transform transform, GoalType goalType)
    {
        targetTransform = transform;
        this.goalType = goalType;
    }


    public Goal(Interaction interaction, GoalType goalType)
    {
        this.goalType = goalType;
        this.interaction = interaction;
    }

    public Goal(Interaction interaction, Item item, GoalType goalType)
    {
        this.goalType = goalType;
        this.interaction = interaction;
        pickUpItem = item;
    }

    public Goal(Interaction recipe, Station station, GoalType goalType)
    {
        this.goalType = goalType;
        this.interaction = recipe;
        recipeStation = station;
        this.station = station;
    }

    // public Goal(Recipe recipe, Station station)
    // {
    //     targetTransform = station.transform;
    //     recipeStation = station;
    // }


    // public Goal(Goal goal)
    // {
    //     this.motivationStatName = goal.motivationStatName;
    //     this.itemType = goal.itemType;
    //     this.itemData = goal.itemData;
    // }
}
