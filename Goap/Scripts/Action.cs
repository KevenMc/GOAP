using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public Goal goal;
    public string actionName;
    public ActionType actionType = ActionType.NULL;
    public float actionCost = 1;
    public float additionalCost;
    public List<Plan> SubPlans = new List<Plan>();
    public Plan parentPlan;
    public Plan topPlan;

    public Interaction interaction;
    public Station interactionStation;

    public ItemData useItem;
    public Item pickUpItem;

    public Transform targetTransform;

    public bool canBePerformed = false;


public void AddSubPlan(Plan plan)
{
    SubPlans.Add(plan);
    plan.hasParentAction = true;
    plan.parentAction = this;
    plan.topPlan = topPlan;
}

    public Action()
    { 
        goal = new Goal(this, GoalType.NULL);
    }

    public Action(Transform targetTransform, Interaction interaction, float dist, GoalType goalType)
    {
        this.interaction = interaction;
        actionCost = 1;
        actionName = "Move to location " + targetTransform;
        this.targetTransform = targetTransform;

        switch (goalType)
        {
            case GoalType.MOVE_TO_ITEM:
                actionType = ActionType.MOVE_TO_PICKUP;
                goal = new Goal(interaction, GoalType.SATISFY_MOVE_TO_ITEM);
                canBePerformed = true;
                actionCost = dist / 10;
                break;

            case GoalType.MOVE_TO_STATION:
                actionType = ActionType.MOVE_TO_STATION;
                goal = new Goal(interaction, GoalType.SATISFY_MOVE_TO_STATION);
                canBePerformed = true;
                actionCost = dist / 10;
                break;

            default:
                Debug.Log("NOT ACCOUNTED FOR CASE : " + goalType);
                goal = new Goal(interaction, goalType);
                break;
        }
    }


    public Action(Interaction interaction, GoalType goalType)
    {
        this.interaction = interaction;
        actionCost = 1;

        switch (goalType)
        {
            case GoalType.CHECK_INTERACTION:
                goal = new Goal(interaction, goalType);
                break;

            case GoalType.CHECK_STATIONS_FOR_INTERACTION:
                actionCost = interaction.interactionCost;
                goal = new Goal(interaction, goalType);
                break;

            case GoalType.USE_ITEM_FROM_INVENTORY:
                break;

            case GoalType.USE_RECIPE_FROM_INVENTORY:
                actionCost = interaction.interactionCost;
                goal = new Goal(interaction, goalType);
                break;

            case GoalType.CHECK_ITEMS_FOR_INTERACTION:
                goal = new Goal(interaction, goalType);
                break;

            case GoalType.COMPLETE_RECIPE_FROM_INVENTORY:
                actionCost = interaction.interactionCost;
                actionType = ActionType.USE_RECIPE_FROM_INVENTORY;
                canBePerformed = true;
                break;

            case GoalType.FIND_ITEM:
                goal = new Goal(interaction.storage.itemData, goalType);
                break;

            default:
                Debug.Log("NOW SUCH CASE FOR GOALTYPE : " + goalType);
                break;
        }
    }


    public Action(Interaction interaction, Station station, GoalType goalType)
    {
        this.interaction = interaction;
        actionCost = 1;
        actionName = "Complete recipe for " + interaction.recipeName + " at " + station;
        interactionStation = station;
        goal = new Goal(interaction, interactionStation, goalType);

        switch (goalType)
        {
            case GoalType.CHECK_ITEMS_FOR_INTERACTION:
                break;

            case GoalType.USE_STATION_FOR_INTERACTION:
                break;

            case GoalType.MOVE_TO_STATION:
                actionType = ActionType.MOVE_TO_STATION;
                targetTransform = station.transform;
                canBePerformed = true;
                break;

            case GoalType.USE_RECIPE_AT_STATION:
                actionType = ActionType.USE_RECIPE_AT_STATION;
                canBePerformed = true;
                break;

            case GoalType.STORE_ITEMS_AT_STATION:
                actionType = ActionType.STORE_ITEMS_AT_STATION;
                interactionStation = station;
                canBePerformed = true;
                break;

            case GoalType.COMPLETE_RECIPE_AT_STATION:
                break;

            case GoalType.RETRIEVE_ITEM_FROM_STORAGE:
                actionType = ActionType.RETRIEVE_ITEM_FROM_STORAGE;
                canBePerformed = true;
                break;

            case GoalType.CHECK_STATIONS_FOR_INTERACTION:
                break;

            default:
                Debug.Log("CASE : " + goalType + " NOT ACCOUTNED FOR");
                break;
        }
    }


    public Action(ItemData itemData, GoalType goalType)
    {
        useItem = itemData;
        goal = new Goal(itemData, goalType);
        actionCost = 1;

        switch (goalType)
        {
            case GoalType.FIND_ITEM:
                actionName = "CHECK INVENTORY FOR ITEM " + itemData;
                break;

            case GoalType.CHECK_ITEMS_FOR_INTERACTION:
                actionName = "THIS IS FOR STORAGE OF  : " + itemData;
                break;

            case GoalType.USE_ITEM_FROM_INVENTORY:
                actionCost = itemData.useCost;
                actionType = ActionType.USE_ITEM_FROM_INVENTORY;
                actionName = "USE THIS ITEM FROM INVENTORY  : " + itemData;
                Debug.Log(actionName);
                break;

            case GoalType.SATISFY_USE_ITEM_FROM_INVENTORY:
                canBePerformed = true;
                actionType = ActionType.USE_ITEM_FROM_INVENTORY;
                actionName = "SATISFY THIS ONE : USE THIS ITEM FROM INVENTORY  : " + itemData;
                break;

            default:
                Debug.Log("case : " + goalType + " not accounted for");
                break;
        }
    }


    public Action(Item item, GoalType goalType, float dist)
    {
        pickUpItem = item;
        actionName = "Pick Up " + pickUpItem;
        goal = new Goal(item, goalType);

        switch (goalType)
        {
            case GoalType.COMPLETE_PICK_UP_ITEM:
                actionType = ActionType.PICK_UP_ITEM;
                actionCost = dist/10;
                canBePerformed = true;
                actionName = "Pick up " + goal.pickUpItem;
                break;

            case GoalType.MOVE_TO_ITEM:
                actionType = ActionType.MOVE_TO_PICKUP;
                targetTransform = item.transform;
                actionCost = dist / 10;
                canBePerformed = true;
                actionName = "1 : Move to " + goal.pickUpItem;
                break;
        }
    }


    public Action(Item item, GoalType goalType)
    {
        actionCost = 1;
        pickUpItem = item;
        actionName = "Pick up " + item.name;
        goal = new Goal(interaction, item, goalType);

        switch (goalType)
        {
            case GoalType.PICK_UP_ITEM:
                actionType = ActionType.PICK_UP_ITEM;
                break;

            case GoalType.MOVE_TO_ITEM:
                actionType = ActionType.MOVE_TO_PICKUP;
                actionName = "2 Move to " + item.name;
                canBePerformed = true;
                break;
        }
    }
}
