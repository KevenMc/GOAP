using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlanHandler : MonoBehaviour
{
    [Header("Agent")]
    public Agent agent;
    public ActionPlanner actionPlanner;
    public ActionHandler actionHandler;

    public Plan actionPlan;


private float counterUpdate = 1;
private float counterTime = 0;

    public void Init(Agent agent)
    {
        this.agent = agent;
        actionPlanner = agent.actionPlanner;
        actionHandler = agent.actionHandler;
    }

    private void Start()
    {

    }


    private void FixedUpdate()
    {
      //  Counter(Time.fixedDeltaTime);
    }

    public void Counter(float delta)
    {
        counterTime += delta;
        if (counterTime > counterUpdate)
        {
            counterTime = 0;
            actionPlanner.StartPlan();
        }
    }
}
