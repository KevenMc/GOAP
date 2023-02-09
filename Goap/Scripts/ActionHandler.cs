using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
[RequireComponent(typeof(MovementHandler))]
[RequireComponent(typeof(ItemHandler))]
[RequireComponent(typeof(StationHandler))]
[RequireComponent(typeof(ActionPlanner))]
[RequireComponent(typeof(StatHandler))]
[RequireComponent(typeof(InteractionHandler))]

public class ActionHandler : MonoBehaviour
{
    public Agent agent;
    public MovementHandler movementHandler;
    public ItemHandler itemHandler;
    public StationHandler stationHandler;
    public InteractionHandler interactionHandler;
    public ActionPlanner actionPlanner;
    public StatHandler statHandler;


    public int frameCount = 5;
    private int currentFrame = 0;

    private Action currentAction;
    private Action lastAction;
    private List<Action> actionList;
    
    public bool startPlannig = false;


    public void Init(Agent agent)
    {
        this.agent = agent;
        movementHandler = agent.movementHandler;
        itemHandler = agent.itemHandler;
        stationHandler = agent.stationHandler;
        interactionHandler = agent.recipeHandler;
        actionPlanner = agent.actionPlanner;
        statHandler = agent.statHandler;
    }


    private void FixedUpdate()
    {
        if(startPlannig) CountFrames();
        else{
            currentFrame += 1;
            if (currentFrame > 100) startPlannig = true;
        }
    }



    private void CountFrames()
    {
        // currentFrame += 1;
        // if (currentFrame >= frameCount)
        // {
            actionPlanner.PlanActions();
            actionList = actionPlanner.GetCurrentAction();
            if(actionList != null && actionList.Count > 0)
            {
                currentAction = actionList[0];
                Debug.Log(currentAction.actionName);
                if (currentAction != lastAction)
                {
                    PerformAction(currentAction);

                    lastAction = currentAction;
                }
                else
                {
                    actionList.RemoveAt(0);
                }

            }

        //     currentFrame = 0;
        // }
    }


    public void PerformCurrentAction()
    {
        PerformAction(actionPlanner.currentAction);
    }


    private void PerformAction(Action action)
    {
        if (action == null) return;

        switch (action.actionType)
        {
            case ActionType.MOVE_TO_PICKUP:
                movementHandler.MoveTo(action.targetTransform.position);
                break;
            case ActionType.MOVE_TO_STATION:
                movementHandler.MoveTo(action.targetTransform.position);
                break;

            case ActionType.PICK_UP_ITEM:
                itemHandler.PickUpItem(action.pickUpItem);
                break;

            case ActionType.USE_RECIPE_AT_STATION:
                interactionHandler.MakeRecipeAtStation(action.interaction, action.interactionStation);
                break;

            case ActionType.STORE_ITEMS_AT_STATION:
                interactionHandler.StoreItemsAtStation(action.interaction, action.interactionStation);
            break;

            case ActionType.USE_RECIPE_FROM_INVENTORY:
                interactionHandler.MakeRecipe(action.interaction);
                break;

            case ActionType.USE_ITEM_FROM_INVENTORY:
                itemHandler.UseItem(statHandler, action.useItem);
                break;

            // case ActionType.CHECK_ALL_ITEMS:
            //     break;

            case ActionType.RETRIEVE_ITEM_FROM_STORAGE:
                interactionHandler.RetrieveItems(action.interaction, action.interactionStation);

                break;

                default:
                Debug.Log("WARNING : CASE " + action.actionType + "NOT ACCOUNTED FOR");
                break;
                
        }

        // if (!CanPerform(action)) return;
        // if (action.targetTransform != null) 
        // if (action.useItem != null) 
        // if (action.useStation != null) stationHandler.Interact(action.useStation, action.stationAction);
        // if (action.recipe != null && !action.recipe.requiresStation) recipeHandler.MakeRecipe(action.recipe);
        // if (action.recipe != null && action.recipe.requiresStation && Vector3.Distance(agent.transform.position, action.recipeStation.transform.position) < 3) recipeHandler.MakeRecipeAtStation(action.recipe, action.recipeStation);
    }


    private bool CanPerform(Action action)
    {
        return action.canBePerformed;
    }
}
