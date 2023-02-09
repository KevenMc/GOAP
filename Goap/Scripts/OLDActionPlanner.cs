// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;
// using System.Linq;
// using UnityEngine.AI;



// [RequireComponent(typeof(Agent))]
// [RequireComponent(typeof(ActionPlanHandler))]
// [RequireComponent(typeof(ActionHandler))]
// [RequireComponent(typeof(StatHandler))]
// public class OLDActionPlanner : MonoBehaviour
// {

//     [Header("Agent")]
//     public Agent agent;
//     public ActionPlanHandler actionPlanHandler;
//     public ActionHandler actionHandler;
//     public StatHandler statHandler;

//     [Header("Action/Goal")]
//     public Goal endGoal;
//     public Plan madePlan;
//     private MotivationStat motivationStat;
//     public List<Plan> currentPlans = new List<Plan>();

//     [Header("Pick up item")]
//     public float pickUpDistance = 1.5f;

//     public void Init(Agent agent)
//     {
//         this.agent = agent;
//         actionPlanHandler = agent.actionPlanHandler;
//         actionHandler = agent.actionHandler;
//         statHandler = agent.statHandler;
//     }

//     #region startion
//     public void StartPlan()
//     {
//         if (statHandler == null)
//         {
//             Init(agent);
//         }
//         if (SetGoal())
//         {
//             List<Item> planKnownItems = new List<Item>(agent.memory.knownItems);
//             List<ItemData> planInventory = new List<ItemData>(agent.inventoryHandler.inventory.items);
//             madePlan = new Plan();
//             GenPlan(new List<Plan>(), endGoal, out madePlan, planKnownItems, planInventory);
//         }
//         else
//         {
//             madePlan = new Plan();
//         }
//     }


//     public Action GetCurrentAction()
//     {
//         StartPlan();

//         List<Plan> terminalNodes = GetTerminalNodes(madePlan);
//         List<Action> actions = GetTerminalActions(terminalNodes);
//         foreach (Action action in actions) LogAction(action);
//         return actions[0];
//     }


//     private static int SortPlansByCost(Plan x, Plan y)
//     {
//         if (x.planCost < y.planCost) return -1;
//         if (x.planCost > y.planCost) return 1;
//         return 0;
//     }


//     private static int SortActionByCost(Action x, Action y)
//     {
//         if (x.actionCost < y.actionCost) return -1;
//         if (x.actionCost > y.actionCost) return 1;
//         return 0;
//     }


//     private List<Plan> GetTerminalNodes(Plan plan)
//     {
//         List<Plan> returnPlans = new List<Plan>();

//         if (plan.actions.Last().plans.Count == 0) returnPlans.Add(plan);

//         foreach (Plan p in plan.actions.Last().plans) returnPlans.AddRange(GetTerminalNodes(p));

//         return returnPlans;
//     }


//     private List<Action> GetTerminalActions(List<Plan> plans)
//     {
//         List<Action> actions = new List<Action>();
//         List<Action> getItemActions = new List<Action>();
//         foreach (Plan plan in plans)
//         {
//             switch (plan.actions.Last().actionType)
//             {
//                 case ActionType.MOVE_TO_PICKUP:
//                     getItemActions.Add(plan.actions.Last());
//                     break;

//                 case ActionType.PICK_UP_ITEM:
//                     getItemActions.Add(plan.actions.Last());
//                     break;

//                 default:
//                     actions.Add(plan.actions.Last());
//                     break;
//             }

//         }
//         getItemActions.Sort(SortActionByCost);
//         actions.Sort(SortActionByCost);
//         getItemActions.AddRange(actions);

//         return getItemActions;
//     }


//     public bool SetGoal()
//     {
//         statHandler.CalculatePriority();
//         motivationStat = statHandler.motivationStats[0];
//         if (motivationStat.currentPriority <= 0)
//         {
//             endGoal = new Goal(null);
//             return false;
//         }

//         endGoal = new Goal(motivationStat);

//         return true;
//     }
//     #endregion


//     #region LOGGING
//     public void LogGoal(Goal goal)
//     {
//         string logStr = "************************** | ";
//         if (goal.pickUpItem != null) logStr += " Item : " + goal.pickUpItem;
//         if (goal.targetTransform != null) logStr += " | ItemPosition : " + goal.targetTransform;
//         if (goal.itemType != ItemType.NULL) logStr += " | ItemType :  " + goal.itemType;
//         if (goal.itemData) logStr += " | ItemData : " + goal.itemData;
//         if (goal.motivationStatName != MotivationStatName.NULL) logStr += " | Motivation : " + goal.motivationStatName;
//         if (goal.recipe != null) logStr += " | Recipe : " + goal.recipe.recipeName;
//         if (goal.station != null) logStr += " | Station : " + goal.station;
//         logStr += " | GoalType = " + goal.goalType;
//         logStr += " |  *************************";
//         //         Debug.Log(logStr);
//     }


//     public string LogPlan(Plan plan, int level = 0, string s = " >>> ", string t = "--------------")
//     {
//         string r = s + level + "  >> can be performed : " + plan.canBePerformed + " | cost : " + plan.planCost + "\n";
//         foreach (Action action in plan.actions)
//         {
//             r += t + action.goal.goalType + " : " + action.actionType + " | " + action.actionName + " | Action cost : " + action.actionCost + " | Can perform : " + action.canBePerformed + " \n";
//             foreach (Plan p in action.plans)
//             {
//                 r += "^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n";
//                 r += LogPlan(p, level + 1, s + level + s, t + t);
//             }
//         }

//         return r;
//     }

//     public void LogAction(Action action)
//     {
//         string logStr = "\n";

//         logStr += action.actionName + " : " + action.actionCost + " : " + action.actionType + " : " + action.goal.goalType + " : " + action.canBePerformed + " \n";

//         Debug.Log(logStr);
//     }


//     public void LogActions(Plan plan)
//     {
//         string logStr = "************************** | \n";

//         List<Action> actions = GetTerminalActions(GetTerminalNodes(plan));
//         actions.Sort(SortActionByCost);
//         foreach (Action action in actions)
//         {
//             logStr += action.actionName + " : " + action.actionCost + " \n";
//         }
//         //        Debug.Log(logStr);
//     }

//     #endregion


//     public bool GenPlan(List<Plan> plans, Goal goal, out Plan madePlan, List<Item> knownItems, List<ItemData> inventory)
//     {
//         foreach (Plan p in plans) p.Calculate();

//         Plan plan = new Plan(goal, knownItems, inventory);
//         madePlan = plan;
//         if (plans.Count > 0) Debug.Log(LogPlan(plans[0]));

//         switch (goal.goalType)
//         {
//             case GoalType.CHECK_INGREDIENTS_FOR_RECIPE:
//                 {
//                     plans[0].canBePerformed = true;
//                     Action thisAction = plans[0].actions.Last();
//                     bool canMakeRecipe = true;

//                     if (goal.recipe.isRecipe)
//                     {
//                         foreach (ItemData tool in goal.recipe.tools)
//                         {
//                             if (agent.inventoryHandler.HasItem(tool, plans[0].inventory)) continue;

//                             Goal toolGoal = new Goal(tool, GoalType.CHECK_INVENTORY);
//                             List<Plan> newPlanList = new List<Plan>();
//                             Plan newPlan = new Plan(toolGoal);

//                             if (GenerateAction(newPlanList, toolGoal, out newPlan, plans[0].knownItems, plans[0].inventory))
//                             {
//                                 newPlan.UpdateKnowledge();
//                                 newPlan.Calculate();
//                                 thisAction.plans.Add(newPlan);
//                             }
//                             else
//                             {
//                                 canMakeRecipe = false;
//                                 break;
//                             }
//                         }

//                         foreach (Recipe.RecipeItem recipe in goal.recipe.recipe)
//                         {
//                             if (agent.inventoryHandler.HasItem(recipe.itemData, plans[0].inventory)) continue;
//                             Goal recipeGoal = new Goal(recipe.itemData, GoalType.CHECK_INVENTORY);

//                             List<Plan> newPlanList = new List<Plan>();
//                             Plan newPlan = new Plan(recipeGoal);

//                             if (GenerateAction(newPlanList, recipeGoal, out newPlan, plans[0].knownItems, plans[0].inventory))
//                             {
//                                 newPlan.UpdateKnowledge();
//                                 newPlan.Calculate();
//                                 thisAction.plans.Add(newPlan);
//                             }
//                             else
//                             {
//                                 canMakeRecipe = false;
//                                 break;
//                             }
//                         }
//                     }

//                     if (goal.recipe.isStorage)
//                     {                       
//                             if (!agent.inventoryHandler.HasItem(goal.recipe.storage.itemData, plans[0].inventory, goal.recipe.storage.quantity ))
//                             {
//                             Goal storageGoal = new Goal(goal.recipe.storage.itemData, GoalType.CHECK_INVENTORY);
//                             List<Plan> newPlanList = new List<Plan>();
//                             Plan newPlan = new Plan(storageGoal);

//                             if (GenerateAction(newPlanList, storageGoal, out newPlan, plans[0].knownItems, plans[0].inventory))
//                             {
//                                 Debug.Log("at least this can be achieved ---------------------------------------------");
//                                 newPlan.UpdateKnowledge();
//                                 newPlan.Calculate();
//                                 thisAction.plans.Add(newPlan);
//                             }
//                             else
//                             {
//                                 Debug.Log("BUTTHIS ONE CAN'NT F SOME READON BE");
//                                 canMakeRecipe = false;
                                
//                             }

//                         }


//                     }

//                     thisAction.canBePerformed = canMakeRecipe;
//                     plans[0].canBePerformed = canMakeRecipe;
//                     madePlan = plans[0];
//                     plans.Sort(SortPlansByCost);
//                     madePlan = plans[0];

//                     if (madePlan.canBePerformed)
//                     {
//                         Debug.Log("I CAN DO THE THING THO");
//                         return true;
//                     }
//                     plans[0].SetCurrentGoal();
//                     Goal nextGoal = plans[0].currentGoal;

//                     return GenerateAction(plans, nextGoal, out madePlan, knownItems, inventory);
//                 }

//             default:
//                 {
//                     if (plans.Count > 0)
//                     {
//                         madePlan = plans[0];
//                         return GenerateAction(plans, goal, out madePlan, plans[0].knownItems, plans[0].inventory);
//                     }

//                     return GenerateAction(plans, goal, out madePlan, knownItems, inventory);
//                 }
//         }
//     }


//     public bool GenerateAction(List<Plan> plans, Goal goal, out Plan madePlan, List<Item> knownItems, List<ItemData> inventory)
//     {
//         Plan plan = new Plan(goal, knownItems, inventory);
//         madePlan = plan;
//         plans.Sort(SortPlansByCost);

//         if (plans.Count > 0)
//         {
//             plan = plans[0];
//             plans.RemoveAt(0);
//         }
//         else plans.Add(plan);

//         SatisfyGoal(plans, plan, goal);

//         if (plans.Count() > 0 && plans[0].canBePerformed)
//         {
//             madePlan = plans[0];
//             return true;
//         }
//         else
//         {
//             currentPlans = plans;

//             if (plans.Count > 0)
//             {
//                 plans[0].SetCurrentGoal();
//                 Goal nextGoal = plans[0].currentGoal;
//                 RemoveGoalItems(goal, plans[0].knownItems, plans[0].inventory);

//                 return GenPlan(plans, nextGoal, out madePlan, knownItems, inventory);
//             }
//         }

//         return false;
//     }


//     private void SatisfyGoal(List<Plan> plans, Plan plan, Goal goal)
//     {
//         switch (goal.goalType)
//         {
//             case GoalType.NULL:
//                 break;

//             case GoalType.MOTIVATION:
//                 plans.AddRange(SatisfyGoalFromMotivation(new Plan(plan), goal)); // GOAL => CHECK INVENTORY
//                 plans.AddRange(SatisfyGoalFromRecipe(new Plan(plan), goal)); // GOAL FOREACH ITEM => CHECK ALL ITEMS || GOAL STATION => USE RECIPE FROM STATION

//                 break;

//             case GoalType.CHECK_INVENTORY:
//                 plans.AddRange(SatisfyGoalFromInventory(new Plan(plan), goal)); //GOAL => USE FROM INVENTORY
//                 break;

//             case GoalType.CHECK_ALL_KNOWLEDGE:
//             Debug.Log("check all knowledge");
//                 plans.AddRange(SatisfyGoalFromRecipe(new Plan(plan), goal)); // GOAL FOREACH ITEM => CHECK ALL ITEMS || GOAL STATION => USE RECIPE FROM STATION
//                 plans.AddRange(SatisfyGoalFromItemMemory(new Plan(plan), goal)); // GOAL => CHECK MEMORY
//                 plans.AddRange(SatisfyGoalFromAllItemTransmute(new Plan(plan), goal)); // GOAL => CHECK ALL ITEMS
//                 break;

//             case GoalType.CHECK_MEMORY:
//                 plans.AddRange(SatisfyGoalFromPickUp(new Plan(plan), goal)); // GOAL => PICK UP ITEM
//                 break;

//             case GoalType.CHECK_TRANSMUTE_FROM_ALL_ITEMS:
//                 plans.AddRange(SatisfyGoalFromAllItemTransmute(new Plan(plan), goal));
//                 break;

//             case GoalType.USE_ITEM_FROM_INVENTORY:
//                 plans.AddRange(SatisfyGoalFromInventory(new Plan(plan), goal)); //GOAL => USE FROM INVENTORY
//                 break;

//             case GoalType.PICK_UP_ITEM:
//                 plans.AddRange(SatisfyGoalFromPickUp(new Plan(plan), goal)); // GOAL => MOVETO ITEM
//                 break;

//             case GoalType.MOVE_TO_ITEM:
//                 plans.AddRange(SatisfyMoveToTransform(new Plan(plan), goal));
//                 break;

//             case GoalType.CHECK_RECIPES_FOR_ITEM:
//                 plans.AddRange(SatisfyGoalFromRecipe(new Plan(plan), goal)); // GOAL FOREACH ITEM => CHECK ALL ITEMS || GOAL STATION => USE RECIPE FROM STATION
//                 break;

//             case GoalType.CHECK_RECIPE:
//                 plans.AddRange(SatisfyGoalFromRecipe(new Plan(plan), goal)); // GOAL FOREACH ITEM => CHECK ALL ITEMS || GOAL STATION => USE RECIPE FROM STATION
//                 break;

//             case GoalType.USE_RECIPE_FROM_INVENTORY:
//                 plans.AddRange(SatisfyGoalFromRecipe(new Plan(plan), goal)); // GOAL FOREACH ITEM => CHECK ALL ITEMS || GOAL STATION => USE RECIPE FROM STATION
//                 break;

//             case GoalType.USE_RECIPE_AT_STATION:
//                 plans.AddRange(SatisfyGoalFromRecipe(new Plan(plan), goal)); // GOAL FOREACH ITEM => CHECK ALL ITEMS || GOAL STATION => USE RECIPE FROM STATION
//                 break;

//             case GoalType.CHECK_STATIONS_FOR_RECIPE:
//                 plans.AddRange(SatisfyGoalFromStationMemory(new Plan(plan), goal)); // GOAL => USE_STATION_FOR_RECIPE
//                 break;

//             case GoalType.USE_STATION_FOR_RECIPE:

//                 plans.AddRange(SatisfyGoalFromRecipe(new Plan(plan), goal)); // GOAL FOREACH ITEM => CHECK ALL ITEMS || GOAL STATION => USE RECIPE FROM STATION
//                 break;

//             case GoalType.MOVE_TO_STATION:
//                 plans.AddRange(SatisfyMoveToTransform(new Plan(plan), goal));
//                 break;
//         }

//         foreach (Plan p in plans)
//         {
//             p.Calculate();
//             RemoveGoalItems(goal, p.knownItems, p.inventory);
//         }
//     }


//     public void RemoveGoalItems(Goal goal, List<Item> planKnownItems, List<ItemData> planInventory)
//     {
//         if (goal.itemData != null) planInventory.Remove(goal.itemData);
//         if (goal.pickUpItem != null) planKnownItems.Remove(goal.pickUpItem);
//     }



//     #region ITEMS

//     public List<Plan> SatisfyGoalFromMotivation(Plan thisPlan, Goal goal) // GOAL => CHECK ALL ITEMS
//     {
//         List<Plan> newPlans = new List<Plan>();
//         List<ItemData> knownItems = agent.memory.AllItemsByItemMotivationStatName(goal.motivationStatName);

//         foreach (ItemData itemData in knownItems)
//         {
//             Plan newPlan = new Plan(thisPlan);
//             Action newAction = new Action(itemData, false, GoalType.CHECK_INVENTORY);
//             newPlan.AddAction(newAction, GoalType.CHECK_INVENTORY);
//             newPlans.Add(newPlan);
//         }

//         return newPlans;
//     }


//     public List<Plan> SatisfyGoalFromInventory(Plan thisPlan, Goal goal) //GOAL => USE FROM INVENTORY
//     {
//         List<Plan> newPlans = new List<Plan>();
//         List<ItemData> itemDatas = new List<ItemData>();
//         Plan newPlan = new Plan(thisPlan);
//         Action newAction = new Action();

//         bool addAction = false;
//         switch (goal.goalType)
//         {
//             case GoalType.CHECK_INVENTORY:
//                 {
//                     ItemData itemData = agent.inventoryHandler.GetItemByItemData(goal.itemData, thisPlan.inventory);
//                     if (itemData != null)
//                     {
//                         newAction = new Action(itemData, false, GoalType.USE_ITEM_FROM_INVENTORY);
//                         addAction = true;
//                     }
//                     else
//                     {
//                         newAction = new Action(goal.itemData, false, GoalType.CHECK_ALL_KNOWLEDGE);
//                         addAction = true;
//                     }
//                 }
//                 break;

//             case GoalType.USE_ITEM_FROM_INVENTORY:
//                 {
//                     newAction = new Action(goal.itemData, true, GoalType.SATISFIED);
//                     newPlan.canBePerformed = true;
//                     addAction = true;
//                 }
//                 break;
//         }

//         if (addAction)
//         {
//             newPlan.AddAction(newAction, newAction.goal.goalType);
//             newPlans.Add(newPlan);
//         }

//         return newPlans;
//     }




//     public List<Plan> SatisfyGoalFromItemMemory(Plan thisPlan, Goal goal) // GOAL =>PICK UP ITEM
//     {
//         List<Plan> newPlans = new List<Plan>();
//         List<Item> knownItems = agent.memory.FindItemsByItemData(goal.itemData, thisPlan.knownItems);
//         foreach (Item item in knownItems)
//         {
//             Plan newPlan = new Plan(thisPlan);
//             Action newAction = new Action(item, GoalType.CHECK_MEMORY, false);
//             newPlan.AddAction(newAction, GoalType.CHECK_MEMORY);
//             newPlans.Add(newPlan);
//             newPlan.currentGoal.pickUpItem = item;
//         }

//         return newPlans;
//     }


//     public List<Plan> SatisfyGoalFromPickUp(Plan thisPlan, Goal goal) // GOAL => MOVETO ITEM
//     {
//         List<Plan> newPlans = new List<Plan>();
//         Plan newPlan = new Plan(thisPlan);
//         Action newAction = new Action();

//         switch (goal.goalType)
//         {
//             case GoalType.PICK_UP_ITEM:
//                 {
//                     NavMeshPath path = new NavMeshPath();
//                     agent.navMeshAgent.CalculatePath(goal.pickUpItem.transform.position, path);

//                     if (path.status == NavMeshPathStatus.PathComplete && CalculatePathDistance(path.corners) < pickUpDistance) newAction = new Action(goal.pickUpItem, GoalType.SATISFIED, true);
//                     else newAction = new Action(goal.pickUpItem, GoalType.MOVE_TO_ITEM, false);
//                 }
//                 break;

//             case GoalType.CHECK_MEMORY:
//                 newAction = new Action(goal.pickUpItem, GoalType.PICK_UP_ITEM, false);
//                 break;
//         }

//         newPlan.AddAction(newAction, newAction.goal.goalType);
//         newPlans.Add(newPlan);

//         return newPlans;
//     }


//     public List<Plan> SatisfyGoalFromAllItemTransmute(Plan thisPlan, Goal goal)// GOAL => CHECK ALL ITEMS
//     {
//         List<Plan> newPlans = new List<Plan>();
//         List<ItemData> knownItems = agent.memory.AllItemsByTransmuteItemData(goal.itemData);

//         foreach (ItemData itemData in knownItems)
//         {
//             Plan newPlan = new Plan(thisPlan);
//             Action newAction = new Action(itemData, false, GoalType.CHECK_TRANSMUTE_FROM_ALL_ITEMS);
//             newPlan.AddAction(newAction, GoalType.CHECK_ALL_KNOWLEDGE);
//             newPlans.Add(newPlan);
//         }

//         return newPlans;
//     }
//     #endregion


//     #region MOVETO
//     public List<Plan> SatisfyMoveToTransform(Plan thisPlan, Goal goal)
//     {
//         List<Plan> newPlans = new List<Plan>();
//         List<Item> knownItems = new List<Item>();
//         Transform targetTransform = agent.transform;

//         switch (goal.goalType)
//         {
//             case GoalType.MOVE_TO_STATION:
//                 {
//                     if(goal.recipe.isRecipe)
//                     {
//                         GoalType goalType = GoalType.SATISFIED;
//                         bool satisfied = true;
//                         foreach (Recipe.RecipeItem recipeItem in goal.recipe.recipe)
//                         {
//                             if (agent.inventoryHandler.HasItem(recipeItem.itemData, thisPlan.inventory)) continue;
//                             goalType = GoalType.CHECK_RECIPE;
//                             satisfied = false;
//                         }

//                         NavMeshPath path = new NavMeshPath();
//                         agent.navMeshAgent.CalculatePath(goal.station.transform.position, path);

//                         if (path.status == NavMeshPathStatus.PathComplete)
//                         {
//                             float dist = CalculatePathDistance(path.corners);
//                             Plan newPlan = new Plan(thisPlan);

//                             Action newAction = new Action(goal.station, goal.recipe, dist, goalType, satisfied);
//                             newPlan.AddAction(newAction, newAction.goal.goalType);
//                             newPlans.Add(newPlan);
//                         }
//                     }

//                     if (goal.recipe.isStorage)
//                     {
//                         GoalType goalType = GoalType.SATISFIED;
//                         bool satisfied = true;


//                             goalType = GoalType.CHECK_ALL_KNOWLEDGE;
//                             satisfied = false;
                            

//                         NavMeshPath path = new NavMeshPath();
//                         agent.navMeshAgent.CalculatePath(goal.station.transform.position, path);

//                         if (path.status == NavMeshPathStatus.PathComplete)
//                         {
//                             float dist = CalculatePathDistance(path.corners);
//                             Plan newPlan = new Plan(thisPlan);

//                             Action newAction = new Action(goal.station, goal.recipe, dist, goalType, satisfied);
//                             newPlan.AddAction(newAction, newAction.goal.goalType);
//                             newPlans.Add(newPlan);
//                         }
//                     }
//                 }
//                 break;

//             case GoalType.MOVE_TO_ITEM:
//                 {
//                     Debug.Log("i am now moving to tomato");
//                     NavMeshPath path = new NavMeshPath();
//                     agent.navMeshAgent.CalculatePath(goal.pickUpItem.transform.position, path);

//                     if (path.status == NavMeshPathStatus.PathComplete)
//                     {
//                         float dist = CalculatePathDistance(path.corners);
//                         Plan newPlan = new Plan(thisPlan);
//                         Action newAction = new Action(goal.pickUpItem, dist, GoalType.SATISFIED);
//                         newAction.canBePerformed = true;
//                         newPlan.AddAction(newAction, newAction.goal.goalType);
//                         newPlans.Add(newPlan);
//                         Debug.Log("SUCCESFLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLFUL");
//                     }
//                 }
//                 break;
//         }

//         return newPlans;
//     }


//     public float CalculatePathDistance(Vector3[] corners)
//     {
//         float dist = 0;
//         Vector3 currentPos = agent.transform.position;
//         foreach (Vector3 corner in corners)
//         {
//             dist += Vector3.Distance(currentPos, corner);
//             currentPos = corner;
//         }

//         return dist;
//     }
//     #endregion


//     #region RECIPES
//     public List<Plan> SatisfyGoalFromRecipe(Plan thisPlan, Goal goal)  // GOAL => CHECK RECIPE >>>  FOREACH ITEM => CHECK ALL ITEMS
//     {
//         List<Plan> newPlans = new List<Plan>();
//         List<Recipe> recipes = new List<Recipe>();
//         Plan newPlan = new Plan(thisPlan);
//         Action newAction = new Action();

//         switch (goal.goalType)
//         {
//             case GoalType.CHECK_ALL_KNOWLEDGE:
//                 {
//                     Debug.Log("check recipes for tomatos");
//                     recipes = agent.memory.FindRecipes(goal.itemData);

//                     foreach (Recipe recipe in recipes)
//                     {
//                         newAction = new Action(recipe, GoalType.CHECK_RECIPE);
//                         newPlan.AddAction(newAction, newAction.goal.goalType);
//                         newPlans.Add(newPlan);
//                     }
//                 }
//                 break;

//             case GoalType.CHECK_RECIPES_FOR_ITEM:
//                 {
//                     recipes = agent.memory.FindRecipes(goal.itemData);
//                     foreach (Recipe recipe in recipes)
//                     {
//                         newAction = new Action(recipe, GoalType.CHECK_RECIPE);
//                         newPlan.AddAction(newAction, newAction.goal.goalType);
//                         newPlans.Add(newPlan);
//                     }
//                 }
//                 break;

//             case GoalType.CHECK_RECIPE:
//                 {
//                     if (goal.recipe.isStorage)
//                     {
//                         Debug.Log("check recipe for storgage");
//                         newAction = new Action(goal.recipe, GoalType.CHECK_STATIONS_FOR_RECIPE);
//                     }
//                     if(goal.recipe.isRecipe)
//                     {
//                         if (goal.recipe.requiresStation) newAction = new Action(goal.recipe, GoalType.CHECK_STATIONS_FOR_RECIPE);
//                         else newAction = new Action(goal.recipe, GoalType.USE_RECIPE_FROM_INVENTORY);

//                     }

//                     newPlan.AddAction(newAction, newAction.goal.goalType);
//                     newPlans.Add(newPlan);
//                 }

//                 break;

//             case GoalType.USE_RECIPE_FROM_INVENTORY:
//                 {
//                     newAction = new Action(goal.recipe, GoalType.CHECK_INGREDIENTS_FOR_RECIPE);
//                     newPlan.AddAction(newAction, newAction.goal.goalType);
//                     newPlans.Add(newPlan);
//                 }

//                 break;

//             case GoalType.USE_RECIPE_AT_STATION:
//                 {
//                     newAction = new Action(goal.recipe, GoalType.CHECK_INGREDIENTS_FOR_RECIPE);
//                     newPlan.AddAction(newAction, newAction.goal.goalType);
//                     newPlans.Add(newPlan);
//                 }
//                 break;

//             case GoalType.USE_STATION_FOR_RECIPE:
//                 {
//                     NavMeshPath path = new NavMeshPath();
//                     agent.navMeshAgent.CalculatePath(goal.station.transform.position, path);

//                     if (path.status == NavMeshPathStatus.PathComplete && CalculatePathDistance(path.corners) < pickUpDistance)
//                     {
//                         newAction = new Action(goal.recipe, goal.station, GoalType.SATISFIED, true);

//                         if(goal.recipe.isRecipe)
//                         {
//                             foreach (Recipe.RecipeItem recipeItem in goal.recipe.recipe) if (!agent.inventoryHandler.HasItem(recipeItem.itemData, thisPlan.inventory)) return newPlans;
//                         }


//                     }
//                     else
                    

//                         if (goal.recipe.isStorage)
//                         {
//                             Debug.Log("use station for storage recipe");
//                             if (!agent.inventoryHandler.HasItem(goal.recipe.storage.itemData, thisPlan.inventory, goal.recipe.storage.quantity))
//                             {
//                                 newAction = new Action(goal.recipe, GoalType.CHECK_INGREDIENTS_FOR_RECIPE);
//                             }
//                         }
//                     else newAction = new Action(goal.recipe, goal.station, GoalType.MOVE_TO_STATION, false);


//                     newPlan.AddAction(newAction, newAction.goal.goalType);
//                     newPlans.Add(newPlan);
//                 }
//                 break;

//             case GoalType.MOTIVATION:
//                 {
//                     if (goal.motivationStat.isStorage)
//                     {
//                         Debug.Log("looking for storage");
//                         recipes = agent.memory.FindRecipesByMotivation(goal.motivationStatName);
//                         foreach (Recipe recipe in recipes)
//                         {
//                             newAction = new Action(recipe, GoalType.CHECK_RECIPE);
//                             newPlan.AddAction(newAction, newAction.goal.goalType);
//                             newPlans.Add(newPlan);
//                         }
//                     }
//                 }
//                 break;


//         }

//         return newPlans;
//     }
//     #endregion



//     #region STATIONS
//     public List<Plan> SatisfyGoalFromStationMemory(Plan thisPlan, Goal goal)
//     {
//         List<Plan> newPlans = new List<Plan>();
//         List<Station> knownStations = new List<Station>();

//         switch (goal.goalType)
//         {
//             case GoalType.CHECK_STATIONS_FOR_RECIPE:
//                 {
//                     Debug.Log("check station for storage");
//                     knownStations = new List<Station>(agent.memory.GetStationsByType(goal.recipe.stationType));
//                     knownStations = new List<Station>(knownStations.FindAll(x => x.canUse));

//                     foreach (Station station in knownStations)
//                     {
//                         Debug.Log("this is a storage unti for what i am looking for");
//                         Plan newPlan = new Plan(thisPlan);
//                         Action newAction = new Action(goal.recipe, station, GoalType.USE_STATION_FOR_RECIPE, false);
//                         newPlan.AddAction(newAction, newAction.goal.goalType);
//                         newPlans.Add(newPlan);
//                     }
//                 }
//                 break;
//         }

//         return newPlans;
//     }
//     #endregion
// }


