using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.AI;


[RequireComponent(typeof(Agent))]
[RequireComponent(typeof(ActionPlanHandler))]
[RequireComponent(typeof(ActionHandler))]
[RequireComponent(typeof(StatHandler))]
public class ActionPlanner : MonoBehaviour
{

    [Header("Agent")]
    public Agent agent;
    public ActionPlanHandler actionPlanHandler;
    public ActionHandler actionHandler;
    public StatHandler statHandler;

    [Header("Action/Goal")]
    public List<Plan> planList = new List<Plan>();
    public Goal endGoal;
    public Plan madePlan;
    private MotivationStat motivationStat;
    public List<Plan> currentPlans = new List<Plan>();
    public Action currentAction;

    [Header("Pick up item")]
    public float interactDistance = 1.5f;

    public void Init(Agent agent)
    {
        this.agent = agent;
        actionPlanHandler = agent.actionPlanHandler;
        actionHandler = agent.actionHandler;
        statHandler = agent.statHandler;
    }


    #region startion
    public void StartPlan()
    {
        if (statHandler == null)
        {
            Init(agent);
        }
        if (SetGoal())
        {
            List<Item> planKnownItems = new List<Item>(agent.memory.knownItems);
            List<ItemData> planInventory = new List<ItemData>(agent.inventoryHandler.inventory.items);
           // madePlan = new Plan();

            Plan startPlan = new Plan(endGoal, planKnownItems, planInventory);
            planList.Clear();
            planList.Add(startPlan);

            //GenPlan(planList, out madePlan);
            //  madePlan.Calculate();
        }
        else madePlan = new Plan();
    }



    public List<Action> GetCurrentAction()
    {
       // StartPlan();

        if (!madePlan.canBePerformed) return null;
        List<Plan> terminalNodes = GetTerminalNodes(madePlan);
        List<Action> actions = GetTerminalActions(terminalNodes);
        foreach (Action action in actions) LogAction(action);
        return actions;
    }


    private static int SortPlansByCost(Plan x, Plan y)
    {
        if (x.planCost < y.planCost) return -1;
        if (x.planCost > y.planCost) return 1;
        return 0;
    }


    private static int SortActionByCost(Action x, Action y)
    {
        if (x.actionCost < y.actionCost) return -1;
        if (x.actionCost > y.actionCost) return 1;
        return 0;
    }


    private List<Plan> GetTerminalNodes(Plan plan)
    {
        List<Plan> returnPlans = new List<Plan>();

        if (plan.actions.Last().SubPlans.Count == 0) returnPlans.Add(plan);

        foreach (Plan p in plan.actions.Last().SubPlans) returnPlans.AddRange(GetTerminalNodes(p));

        return returnPlans;
    }


    private List<Action> GetTerminalActions(List<Plan> plans)
    {
        List<Action> actions = new List<Action>();
        List<Action> moveToActions = new List<Action>();
        List<Action> pickUpActions = new List<Action>();
        foreach (Plan plan in plans)
        {
            switch (plan.actions.Last().actionType)
            {
                case ActionType.MOVE_TO_PICKUP:
                    moveToActions.Add(plan.actions.Last());
                    break;

                case ActionType.PICK_UP_ITEM:
                    pickUpActions.Add(plan.actions.Last());
                    break;

                default:
                    actions.Add(plan.actions.Last());
                    break;
            }
        }

        pickUpActions.Sort(SortActionByCost);
        moveToActions.Sort(SortActionByCost);
        actions.Sort(SortActionByCost);

        if (pickUpActions.Count > 0) return pickUpActions;
        if (moveToActions.Count > 0) return moveToActions;
        return actions;
    }


    public bool SetGoal()
    {
        statHandler.CalculatePriority();
        motivationStat = statHandler.motivationStats[0];
        if (motivationStat.currentPriority <= 0)
        {
            endGoal = new Goal(null);
            return false;
        }

        endGoal = new Goal(motivationStat);

        return true;
    }
    #endregion


    #region LOGGING
    public void LogGoal(Goal goal)
    {
        string logStr = "************************** | ";
        if (goal.pickUpItem != null) logStr += " Item : " + goal.pickUpItem;
        if (goal.targetTransform != null) logStr += " | ItemPosition : " + goal.targetTransform;
        if (goal.itemData) logStr += " | ItemData : " + goal.itemData;
        if (goal.motivationStatName != MotivationStatName.NULL) logStr += " | Motivation : " + goal.motivationStatName;
        if (goal.interaction != null) logStr += " | Recipe : " + goal.interaction.recipeName;
        if (goal.station != null) logStr += " | Station : " + goal.station;
        logStr += " | GoalType = " + goal.goalType;
        logStr += " |  *************************";
        Debug.Log(logStr);
    }


    public string LogPlan(Plan plan, int level = 0, string s = " >>> ", string t = "--------------")
    {
        string r = s + level + "  >> can be performed : " + plan.canBePerformed + " | Plan cost : " + plan.planCost + "\n";
        foreach (Action action in plan.actions)
        {
            r += t + action.goal.goalType + " : " + action.actionType + " | " + action.actionName + " | Action cost : " + action.actionCost + " | Can perform : " + action.canBePerformed + " \n";
            foreach (Plan p in action.SubPlans)
            {
                r += "^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n";
                r += LogPlan(p, level + 1, s + level + s, t + t);
            }
        }

        return r;
    }


    public void LogAction(Action action)
    {
        string logStr = "\n";
        logStr += action.actionName + " : " + action.actionCost + " : " + action.actionType + " : " + action.goal.goalType + " : " + action.canBePerformed + " \n";
        Debug.Log(logStr);
    }
    #endregion


    #region ACTION PLANNING

    public void PlanActions()
    {

        if (planList.Count == 0) StartPlan();

        Debug.Log(planList.Count);
        if (GenPlan(planList, out madePlan))
        {
            currentAction = GetCurrentAction()[0];
            planList.Clear();
        }

    }



    public bool GenPlan(List<Plan> plans, out Plan madePlan)
    {
        foreach (Plan p in plans) p.Calculate();
        plans.Sort(SortPlansByCost);
        Plan plan = GetBottomPlan(plans[0]);
        Debug.Log(plan == plans[0]);
        madePlan = plan;
        LogGoal(plan.currentGoal);
       Debug.Log(LogPlan(plan));

        switch (plan.currentGoal.goalType)
        {
            
            case GoalType.CHECK_ITEMS_FOR_INTERACTION:
                {
                    Goal currentGoal = plan.actions.Last().goal;
                    plan.canBePerformed = true;
                    Action thisAction = plan.actions.Last();
                    bool canMakeRecipe = true;
                    switch (currentGoal.interaction.interactionType)
                    {
                        case InteractionType.RECIPE:
                            {
                                Debug.Log(" >>>>>>>>>>>>>>>>>> CHECK INTERACTION RECIPE " + currentGoal.interaction.recipeName);
                                foreach (ItemData tool in currentGoal.interaction.tools)
                                {
                                    if (agent.inventoryHandler.HasItem(tool, plan.inventory)) continue;

                                    Goal toolGoal = new Goal(tool, GoalType.FIND_ITEM);
                                    List<Plan> newPlanList = new List<Plan>();
                                    Plan newPlan = new Plan(toolGoal, plan.knownItems, plan.inventory, plan);
                                    newPlanList.Add(newPlan);

                                    if (GenerateAction(newPlanList, out newPlan))
                                    {
                                        AddBranch(newPlan, thisAction);
                                        plan.Calculate();
                                    }
                                    else
                                    {
                                        AddBranch(newPlan, thisAction);

                                        canMakeRecipe = false;
                                        break;
                                    }
                                }

                                foreach (Interaction.RecipeItem recipe in currentGoal.interaction.recipe)
                                {
                                    if (agent.inventoryHandler.HasItem(recipe.itemData, plan.inventory)) continue;
                                    Goal recipeGoal = new Goal(recipe.itemData, GoalType.FIND_ITEM);

                                    List<Plan> newPlanList = new List<Plan>();
                                    Plan newPlan = new Plan(recipeGoal, plan.knownItems, plan.inventory, plan);
                                    newPlanList.Add(newPlan);

                                    if (GenerateAction(newPlanList, out newPlan))
                                    {
                                        AddBranch(newPlan, thisAction);
                                        plan.Calculate();
                                    }
                                    else
                                    {
                                        AddBranch(newPlan, thisAction);
                                        canMakeRecipe = false;
                                        break;
                                    }
                                }
                            }
                            break;

                        case InteractionType.STORE_ITEM_AT_STATION:
                            {
                                if (agent.inventoryHandler.HasItem(currentGoal.interaction.storage.itemData, plan.inventory, currentGoal.interaction.storage.quantity)) break;

                                Goal storageGoal = new Goal(plan.currentGoal.interaction.storage.itemData, GoalType.FIND_ITEM);
                                List<Plan> newPlanList = new List<Plan>();
                                Plan newPlan = new Plan(storageGoal, plan.knownItems, plan.inventory, plan);
                                newPlanList.Add(newPlan);

                                if (GenerateAction(newPlanList, out newPlan)) AddBranch(newPlan, thisAction);
                                else
                                {
                                    AddBranch(newPlan, thisAction);
                                    canMakeRecipe = false;
                                }
                            }
                            break;
                    }

                    thisAction.canBePerformed = canMakeRecipe;
                    plan.canBePerformed = canMakeRecipe;
                    plan.Calculate();

                    plans.Sort(SortPlansByCost);
                    madePlan = plan;

                    if (madePlan.canBePerformed) return true;

                    plan.SetCurrentGoal();
                    Goal nextGoal = plan.currentGoal;
//return false;
                    return GenerateAction(plans, out madePlan);
                }

            default:
                {
                    plans.Sort(SortPlansByCost);

                    if (plans.Count > 0)
                    {
                        plans[0].SetCurrentGoal();
                        madePlan = plans[0];

                        if (madePlan.canBePerformed) return true;
                        return GenerateAction(plans, out madePlan);
                    }
                    return GenerateAction(plans, out madePlan);
                }
        }
    }


    public bool GenerateAction(List<Plan> plans, out Plan madePlan)
    {
        Plan plan = plans[0];
        madePlan = plan;


        if (plans.Count > 0)
        {
            plan = GetBottomPlan(plans[0]);

        }

        SatisfyGoal(plans);


        if (plans.Count() > 0 && plans[0].canBePerformed)
        {
            Debug.Log("PLAN HAS BEEN ACHIEVED");
            madePlan = plans[0];
            madePlan.Calculate();
            return true;
        }
        else
        {
            currentPlans = plans;

            if (plans.Count > 0)
            {
                if (plans[0] == plan) Debug.Log("I SHOULD NOT BE 8***************************************");
                plans[0].SetCurrentGoal();
                Goal nextGoal = plans[0].currentGoal;

                
                Debug.Log("no plan yet");
                return false;
               // return GenPlan(plans, out madePlan);
            }
        }

        return false;
    }


    private void SatisfyGoal(List<Plan> plans)
    {
        Plan plan = GetBottomPlan(plans[0]);

        plans.RemoveAt(0);
        Goal goal = plan.currentGoal;
        switch (goal.goalType)
        {
            case GoalType.NULL:
                break;

            case GoalType.MOTIVATION:
                Debug.Log("case GoalType.MOTIVATION: " + goal.motivationStat.motivationStatName);
                SatisfyGoalForMotivation(plans, new Plan(plan), goal); // >> USE_ITEM_FROM_INVENTORY >> CHECK_INTERACTION >> PICK_UP_ITEM
                break;

            case GoalType.USE_ITEM_FROM_INVENTORY:
                Debug.Log("case GoalType.USE_ITEM_FROM_INVENTORY:" + goal.itemData);
                SatisfyGoalFromInventory(plans, new Plan(plan), goal); // >> SATISFY_USE_ITEM_FROM_INVENTORY
                break;

            case GoalType.CHECK_INTERACTION:
                Debug.Log("CHECK_INTERACTION : " + goal.interaction);
                CheckInteraction(plans, new Plan(plan), goal); // >>CHECK_STATIONS_FOR_RECIPE >>USE_RECIPE_FROM_INVENTORY
                break;

            case GoalType.CHECK_STATIONS_FOR_INTERACTION:
                Debug.Log("case GoalType.CHECK_STATIONS_FOR_RECIPE:" + goal.interaction);
                CheckStationsForRecipe(plans, new Plan(plan), goal);  // >> USE_STATION_FOR_RECIPE >> CHECK_INGREDIENTS_FOR_RECIPE
                break;

            case GoalType.USE_STATION_FOR_INTERACTION:
                Debug.Log("case GoalType.USE_STATION_FOR_RECIPE:" + goal.interaction + " >> " + goal.station);
                CheckInteractionWithStation(plans, new Plan(plan), goal);  // >> MOVE_TO_STATION >> USE_RECIPE_AT_STATION
                break;

            case GoalType.MOVE_TO_STATION:
                Debug.Log("case GoalType.MOVE_TO_STATION:" + goal.station);
                SatisfyMoveToTransform(plans, new Plan(plan), goal); // >> SATISFY_MOVE_TO_STATION
                break;

            case GoalType.USE_RECIPE_AT_STATION:
                Debug.Log("case GoalType.USE_RECIPE_AT_STATION:" + goal.interaction + " >> " + goal.station);
                CompleteRecipeAtStation(plans, new Plan(plan), goal); //COMPLETE_RECIPE_AT_STATION
                break;

            case GoalType.PICK_UP_ITEM:
                Debug.Log("case GoalType.PICK_UP_ITEM:" + goal.pickUpItem);
                PickUpItem(plans, new Plan(plan), goal); // >> MOVE_TO_ITEM >> COMPLETE_PICK_UP_ITEM
                break;

            case GoalType.MOVE_TO_ITEM:
                Debug.Log("case GoalType.MOVE_TO_ITEM:" + goal.pickUpItem);
                SatisfyMoveToTransform(plans, new Plan(plan), goal); // >> SATISFY_MOVE_TO_ITEM
                break;

            case GoalType.FIND_ITEM:
                Debug.Log("case GoalType.FIND_ITEM:" + goal.itemData);
                FindItemData(plans, new Plan(plan), goal); //>>CHECK_RECIPE >> PICK_UP_ITEM
                break;

            case GoalType.USE_RECIPE_FROM_INVENTORY:
                Debug.Log("case GoalType.USE_RECIPE_FROM_INVENTORY:");
                CheckRecipeFromInventory(plans, new Plan(plan), goal); // >> COMPLETE_RECIPE_FROM_INVENTORY >> CHECK_INGREDIENTS_FOR_RECIPE
                break;
        }

        foreach (Plan p in plans)
        {
            p.Calculate();
            RemoveGoalItems(goal, p.knownItems, p.inventory);
        }
        plans.Sort(SortPlansByCost);

    }
    #endregion


    #region HELPER FUNCTIONS
    public Plan GetBottomPlan(Plan plan)
    {
        Plan returnPlan = plan;
        if(plan.actions.Count > 0)
        {
            foreach(Action action in plan.actions)
            {
                foreach (Plan p in plan.actions.Last().SubPlans)
                {
                    Debug.Log("/////////////////////////////////////////////////////// sub plans are real");
                    if (!p.canBePerformed) returnPlan = GetBottomPlan(p);
                }
            }
        }
 
        return returnPlan;
    }

    public void RemoveGoalItems(Goal goal, List<Item> planKnownItems, List<ItemData> planInventory)
    {
        if (goal.itemData != null) planInventory.Remove(goal.itemData);
        if (goal.pickUpItem != null) planKnownItems.Remove(goal.pickUpItem);
    }


    public void AddActionToPlans(List<Plan> plans, Plan plan, Action newAction)
    {
        Plan newPlan = new Plan(plan);
        newPlan.AddAction(newAction);
        plans.Add(newPlan);
    }


    public void AddBranch(Plan plan, Action action)
    {
        plan.UpdateKnowledge();
        plan.Calculate();
        action.AddSubPlan(plan);
    }
    #endregion


    #region ITEMS
    public void SatisfyGoalForMotivation(List<Plan> plans, Plan plan, Goal goal)
    {
        if (goal.motivationStatName == MotivationStatName.NULL) return;

        #region Inventory Items >> USE_ITEM_FROM_INVENTORY
        IEnumerable<ItemData> inventoryItems = agent.inventoryHandler.GetItemsByMotivationStatName(goal.motivationStatName, plan.inventory).Distinct();
        
        foreach (ItemData itemData in inventoryItems) AddActionToPlans(plans, plan, new Action(itemData, GoalType.USE_ITEM_FROM_INVENTORY));
        #endregion

        #region Recipes >> CHECK_INTERACTION
        List<Interaction> interactions = agent.memory.FindRecipesByMotivation(goal.motivationStatName);

        foreach (Interaction interaction in interactions) AddActionToPlans(plans, plan, new Action(interaction, GoalType.CHECK_INTERACTION));
        #endregion

        #region KnownItems >> PICK_UP_ITEM
        List<Item> knownItems = agent.memory.FindItemsByItemMotivationStatName(goal.motivationStatName, plan.knownItems);

        foreach (Item item in knownItems) AddActionToPlans(plans, plan, new Action(item, GoalType.PICK_UP_ITEM));
        #endregion
    }


    public void SatisfyGoalFromInventory(List<Plan> plans, Plan plan, Goal goal) //>>SATISFY_USE_ITEM_FROM_INVENTORY
    {
        AddActionToPlans(plans, plan, new Action(goal.itemData, GoalType.SATISFY_USE_ITEM_FROM_INVENTORY));
    }


    public void FindItemData(List<Plan> plans, Plan plan, Goal goal) //>>CHECK_RECIPE >> PICK_UP_ITEM  >> USE_STATION_FOR_INTERACTION  //>> USE_ITEM_FROM_INVENTORY
    {

        #region Interactions >> CHECK_INTERACTION
        {
            List<Interaction> interactions = agent.memory.FindInteractions(goal.itemData);//>>CHECK_RECIPE

            foreach (Interaction interaction in interactions)
            {
                Debug.Log("INTERACTIONS FOR " + goal.itemData + " >> " + interaction.recipeName);
                AddActionToPlans(plans, plan, new Action(interaction, GoalType.CHECK_INTERACTION));
            }
        }
        #endregion

        #region KnownItems >> PICK_UP_ITEM
        {
            List<Item> knownItems = agent.memory.FindItemsByItemData(goal.itemData, plan.knownItems); //>> PICK_UP_ITEM

            foreach (Item item in knownItems) AddActionToPlans(plans, plan, new Action(item, GoalType.PICK_UP_ITEM));
        }
        #endregion

        #region Transmute
        {
            List<ItemData> transmuteItems = agent.inventoryHandler.FindItemsByTransmuteItemData(goal.itemData, plan.inventory); //>> USE_ITEM_FROM_INVENTORY

            foreach (ItemData itemData in transmuteItems) AddActionToPlans(plans, plan, new Action(itemData, GoalType.USE_ITEM_FROM_INVENTORY));
        }
        #endregion
    }


    public void PickUpItem(List<Plan> plans, Plan plan, Goal goal) // >> MOVE_TO_ITEM >> COMPLETE_PICK_UP_ITEM
    {
        Action newAction = new Action();

        switch (goal.goalType)
        {
            case GoalType.PICK_UP_ITEM: // >> MOVE_TO_ITEM >> COMPLETE_PICK_UP_ITEM
                {
                    NavMeshPath path = new NavMeshPath();
                    agent.navMeshAgent.CalculatePath(goal.pickUpItem.transform.position, path);
                    float dist = CalculatePathDistance(path.corners);

                    if (path.status == NavMeshPathStatus.PathComplete && dist < interactDistance) newAction = new Action(goal.pickUpItem, GoalType.COMPLETE_PICK_UP_ITEM, dist);
                    else newAction = new Action(goal.pickUpItem, GoalType.MOVE_TO_ITEM, dist);
                }
                break;
        }

        AddActionToPlans(plans, plan, newAction);
    }
    #endregion


    #region MOVETO
    public void SatisfyMoveToTransform(List<Plan> plans, Plan plan, Goal goal)// >> SATISFY_MOVE_TO_STATION >> SATISFY_MOVE_TO_ITEM
    {
        Transform targetTransform = transform;
        GoalType goalType = GoalType.NULL;

        switch (goal.goalType)
        {
            case GoalType.MOVE_TO_STATION:// >> SATISFY_MOVE_TO_STATION
                {
                    targetTransform = goal.station.transform;
                    goalType = GoalType.SATISFY_MOVE_TO_STATION;
                }
                break;

            case GoalType.MOVE_TO_ITEM:// >> SATISFY_MOVE_TO_ITEM
                {
                    targetTransform = goal.pickUpItem.transform;
                    goalType = GoalType.SATISFY_MOVE_TO_ITEM;
                }
                break;
        }

        NavMeshPath path = new NavMeshPath();
        agent.navMeshAgent.CalculatePath(targetTransform.position, path);
        float dist = CalculatePathDistance(path.corners);

        if (path.status == NavMeshPathStatus.PathComplete && dist > interactDistance)
        {
            AddActionToPlans(plans, plan, new Action(targetTransform, goal.interaction, dist, goalType));
        }
    }


    public float CalculatePathDistance(Vector3[] corners)
    {
        float dist = 0;
        Vector3 currentPos = agent.transform.position;
        foreach (Vector3 corner in corners)
        {
            dist += Vector3.Distance(currentPos, corner);
            currentPos = corner;
        }

        return dist;
    }
    #endregion


    #region INTERACTIONS
    public void CheckInteraction(List<Plan> plans, Plan plan, Goal goal)// >>CHECK_STATIONS_FOR_RECIPE >>USE_RECIPE_FROM_INVENTORY
    {
        Action newAction = new Action();

        switch (goal.goalType)
        {
            case GoalType.CHECK_INTERACTION: // >>CHECK_STATIONS_FOR_RECIPE >>USE_RECIPE_FROM_INVENTORY
                {
                    switch (goal.interaction.interactionType)
                    {
                        case InteractionType.RECIPE:
                            if (goal.interaction.requiresStation) newAction = new Action(goal.interaction, GoalType.CHECK_STATIONS_FOR_INTERACTION);
                            else
                            {
                                if (agent.inventoryHandler.CanSatisfyRecipe(goal.interaction.recipe, plan.inventory)) newAction = new Action(goal.interaction, GoalType.COMPLETE_RECIPE_FROM_INVENTORY);
                                else newAction = new Action(goal.interaction, GoalType.USE_RECIPE_FROM_INVENTORY);
                            }
                            break;

                        case InteractionType.STORE_ITEM_AT_STATION:
                            if (agent.inventoryHandler.HasItem(goal.interaction.storage.itemData, plan.inventory, goal.interaction.storage.quantity)) newAction = new Action(goal.interaction, GoalType.CHECK_STATIONS_FOR_INTERACTION);
                            else newAction = new Action(goal.interaction, GoalType.FIND_ITEM);
                            break;

                        case InteractionType.RETRIEVE_ITEM_FROM_STORAGE:
                            newAction = new Action(goal.interaction, goal.station, GoalType.CHECK_STATIONS_FOR_INTERACTION);
                            break;
                    }

                    AddActionToPlans(plans, plan, newAction);
                }
                break;
        }
    }


    public void CompleteRecipeAtStation(List<Plan> plans, Plan plan, Goal goal)
    {
        AddActionToPlans(plans, plan, new Action(goal.interaction, goal.station, GoalType.COMPLETE_RECIPE_AT_STATION));
    }


    public void CheckInteractionWithStation(List<Plan> plans, Plan plan, Goal goal)// >> MOVE_TO_STATION >> USE_RECIPE_AT_STATION
    {
        Action newAction = new Action();
        NavMeshPath path = new NavMeshPath();

        agent.navMeshAgent.CalculatePath(goal.station.transform.position, path);
        float dist = CalculatePathDistance(path.corners);

        if (!(path.status == NavMeshPathStatus.PathComplete)) return;

        switch (goal.interaction.interactionType)
        {
            case InteractionType.RECIPE:
                if (dist > interactDistance) newAction = new Action(goal.interaction, goal.station, GoalType.MOVE_TO_STATION); // >> MOVE_TO_STATION
                else newAction = new Action(goal.interaction, goal.station, GoalType.USE_RECIPE_AT_STATION); //>> USE_RECIPE_AT_STATION
                break;

            case InteractionType.STORE_ITEM_AT_STATION:
                if (dist > interactDistance) newAction = new Action(goal.interaction, goal.station, GoalType.MOVE_TO_STATION); // >> MOVE_TO_STATION
                else newAction = new Action(goal.interaction, goal.station, GoalType.STORE_ITEMS_AT_STATION); //>> USE_RECIPE_AT_STATION        
                break;

            case InteractionType.RETRIEVE_ITEM_FROM_STORAGE:
                if (dist > interactDistance) newAction = new Action(goal.interaction, goal.station, GoalType.MOVE_TO_STATION); // >> MOVE_TO_STATION
                else newAction = new Action(goal.interaction, goal.station, GoalType.RETRIEVE_ITEM_FROM_STORAGE); //>> USE_RECIPE_AT_STATION
                break;
        }

        AddActionToPlans(plans, plan, newAction);
    }


    public void CheckRecipeFromInventory(List<Plan> plans, Plan plan, Goal goal) // >> COMPLETE_RECIPE_FROM_INVENTORY >> CHECK_INGREDIENTS_FOR_RECIPE
    {
        GoalType goalType = GoalType.COMPLETE_RECIPE_FROM_INVENTORY;
        if (!agent.inventoryHandler.CanSatisfyRecipe(goal.interaction.recipe, plan.inventory)) goalType = GoalType.CHECK_ITEMS_FOR_INTERACTION;

        switch (goal.goalType)
        {
            case GoalType.USE_RECIPE_FROM_INVENTORY:
                {
                    AddActionToPlans(plans, plan, new Action(goal.interaction, goalType));
                }
                break;
        }
    }
    #endregion


    #region STATIONS
    public void CheckStationsForRecipe(List<Plan> plans, Plan plan, Goal goal) // >> USE_STATION_FOR_RECIPE >>CHECK_INGREDIENTS_FOR_RECIPE 
    {
        List<Station> knownStations = new List<Station>();

        knownStations = new List<Station>(agent.memory.GetStationsByType(goal.interaction.stationType));
        knownStations = new List<Station>(knownStations.FindAll(x => x.canUse));

        GoalType goalType = GoalType.USE_STATION_FOR_INTERACTION;
        switch (goal.interaction.interactionType)
        {
            case InteractionType.RECIPE:
                if (!agent.inventoryHandler.CanSatisfyRecipe(goal.interaction.recipe, plan.inventory)) goalType = GoalType.CHECK_ITEMS_FOR_INTERACTION;
                break;

            case InteractionType.RETRIEVE_ITEM_FROM_STORAGE:
                List<Station> newStations = new List<Station>();
                knownStations = new List<Station>(knownStations.FindAll(x => x.inventoryData != null));

                foreach (Station station in knownStations)
                {
                    if (agent.inventoryHandler.HasItem(goal.interaction.product, station.inventoryData.items)) newStations.Add(station);
                }

                knownStations = newStations;
                break;
        }

        foreach (Station station in knownStations)
        {
            NavMeshPath path = new NavMeshPath();
            agent.navMeshAgent.CalculatePath(station.transform.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                AddActionToPlans(plans, plan, new Action(goal.interaction, station, goalType));
            }
        }
    }
    #endregion
}


