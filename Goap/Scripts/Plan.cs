using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plan
{
    public Goal endGoal;
    public Goal currentGoal;
    public List<Action> actions = new List<Action>();
    public Action parentAction;
    public Plan topPlan;
    public bool hasParentAction = false;
    public float planCost;

    public bool satisfiesGoal = false;
    public bool canBePerformed = false;

    public List<Item> knownItems = new List<Item>();
    public List<ItemData> inventory = new List<ItemData>();
    public List<Item> updateKnownItems;
    public List<ItemData> updateInventory;
    public bool hasKnowledge = false;


    public void AddAction(Action action)
    {
        this.actions.Add(action);
        this.satisfiesGoal = true;
        this.canBePerformed = action.canBePerformed;
        this.currentGoal = action.goal;//new Goal(action, goalType);
        action.parentPlan = this;
        action.topPlan = topPlan;

        Calculate();
        SetCurrentGoal();
    }


    public void UpdateKnowledge()
    {
        updateInventory.Clear();
        updateInventory.AddRange(inventory);
        updateKnownItems.Clear();
        updateKnownItems.AddRange(knownItems);
    }


    public Plan()
    { 
        topPlan = this;
    }


    public Plan(List<Item> items, List<ItemData> inv)
    {
        knownItems = items;
        inventory = inv;
    }


    public Plan(Goal goal, List<Item> items, List<ItemData> inv, Plan plan = null)
    {
        endGoal = goal;
        currentGoal = goal;
        knownItems = new List<Item>(items);
        inventory = new List<ItemData>(inv);
        updateKnownItems = items;
        updateInventory = inv;
        hasKnowledge = true;
        if(plan == null) topPlan = this;
        else topPlan = plan.topPlan;
    }


    public Plan(Goal goal, Plan plan = null)
    {
        endGoal = goal;
        currentGoal = endGoal;
        if(plan == null) topPlan = this;
        else topPlan = plan;
    }


    public Plan(Plan oldPlan)
    {
        endGoal = oldPlan.endGoal;
        actions = new List<Action>(oldPlan.actions);

        if (actions.Count > 0) currentGoal = new Goal(oldPlan.actions.Last(), oldPlan.actions.Last().goal.goalType);
        else currentGoal = endGoal;

        planCost = oldPlan.planCost;
        updateKnownItems = oldPlan.updateKnownItems;
        knownItems = oldPlan.knownItems;
        updateInventory = oldPlan.updateInventory;
        inventory = oldPlan.inventory;
    }


    public void Calculate()
    {
        planCost = CalculateCost(this);
    }




    public float CalculateCost(Plan plan, float cost = 0)
    {
        List<Action> revActions = new List<Action>(plan.actions);
        revActions.Reverse();
        foreach (Action a in revActions)
        {

            foreach (Plan p in a.SubPlans)
            {
                a.actionCost = CalculateCost(p, cost);
            }
            cost += a.actionCost;
            plan.planCost = cost;
        }
        return cost;
    }


    public void SetCurrentGoal()
    {
        if(actions.Count > 0) this.currentGoal = actions.Last().goal;
        else this.currentGoal = endGoal;
    }
}
