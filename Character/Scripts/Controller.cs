using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    private OverviewInputActions overviewInputActions;
    private InputAction lClick;
    private InputAction rClick;
    [SerializeField] Transform target;


    public Vector2 mousePos;
    public Agent agent;
    public Agent newAgent;

    static Agent universalAgent;

    public bool shift;



    ///

    ///

    private void Awake()
    {
        overviewInputActions = new OverviewInputActions();

    }


    private void OnEnable()
    {
        lClick = overviewInputActions.Actions.Select;
        lClick.started += LClicked;
        lClick.Enable();


        rClick = overviewInputActions.Actions.RClick;
        rClick.started += RClicked;
        rClick.Enable();

        overviewInputActions.Actions.Shift.started += ToggleShift;
        overviewInputActions.Actions.Shift.canceled += ToggleShift;

        overviewInputActions.Actions.Q.performed += StartPlan; 

        overviewInputActions.Actions.W.performed += PerformCurrentAction;

        overviewInputActions.Actions.E.performed += EndCurrentAction;

        overviewInputActions.Actions.R.performed += CalculatStats;




        overviewInputActions.Actions.Enable();
    }

    public void StartPlan(InputAction.CallbackContext obj)
    {
        agent.actionPlanner.StartPlan();
    }

    public void PerformCurrentAction(InputAction.CallbackContext obj)
    {
        agent.actionPlanner.PlanActions();
    }
public void EndCurrentAction(InputAction.CallbackContext obj)
    {
        agent.actionHandler.PerformCurrentAction();
    }

    public void CalculatStats(InputAction.CallbackContext obj)
    {
        agent.statHandler.CalculatePriority();
    }


    private void RClicked(InputAction.CallbackContext obj)
    {
        if (shift)
        {
        }
        else
        {
            agent = null;
        }
    }


    private void ToggleShift(InputAction.CallbackContext obj)
    {
        shift = !shift;
    }

    private void LClicked(InputAction.CallbackContext obj)
    {
        Ray ray = Camera.main.ScreenPointToRay(overviewInputActions.Actions.Mouse.ReadValue<Vector2>());
        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit);

        //if click hits something
        if (hasHit)
        {
            newAgent = hit.transform.gameObject.GetComponentInParent<Agent>();

            if (shift)
            {
            }
            else
            {
                if (newAgent != null)
                {
                    agent = newAgent;
                }
                else if (agent != null)
                {
                    agent.navMeshAgent.destination = hit.point;
                }
            }
        }

    }

}
