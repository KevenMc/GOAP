using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MovementHandler))]

[RequireComponent(typeof(InventoryHandler))]
[RequireComponent(typeof(ItemHandler))]
[RequireComponent(typeof(StationHandler))]
[RequireComponent(typeof(InteractionHandler))]

[RequireComponent(typeof(ActionPlanner))]
[RequireComponent(typeof(ActionPlanHandler))]
[RequireComponent(typeof(ActionHandler))]

[RequireComponent(typeof(StatHandler))]
[RequireComponent(typeof(Memory))]

public class Agent : MonoBehaviour
{

    [Header("Universals")]
    public InventoryData allItems;


    [Header("Agent Info")]
    public string agentName;
    public InventoryHandler inventoryHandler;
    public ItemHandler itemHandler;
    public StationHandler stationHandler;
    public InteractionHandler recipeHandler;


    [Header("AI")]
    public NavMeshAgent navMeshAgent;
    public MovementHandler movementHandler;
    public ActionPlanner actionPlanner;
    public ActionPlanHandler actionPlanHandler;
    public ActionHandler actionHandler;
    public StatHandler statHandler;
    public Memory memory;



    // Start is called before the first frame update
    void OnEnable()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        movementHandler = GetComponent<MovementHandler>();

        inventoryHandler = GetComponent<InventoryHandler>();
        itemHandler = GetComponent<ItemHandler>();
        stationHandler = GetComponent<StationHandler>();
        recipeHandler = GetComponent<InteractionHandler>();

        actionPlanner = GetComponent<ActionPlanner>();
        actionPlanHandler = GetComponent<ActionPlanHandler>();
        actionHandler = GetComponent<ActionHandler>();
        statHandler = GetComponent<StatHandler>();
        memory = GetComponent<Memory>();
    }


    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (movementHandler != null) movementHandler.Init(this);
        if (inventoryHandler != null) inventoryHandler.Init(this);
        if (itemHandler != null) itemHandler.Init(this);
        if (stationHandler != null) stationHandler.Init(this);
        if (recipeHandler != null) recipeHandler.Init(this);

        if (actionPlanner != null) actionPlanner.Init(this);
        if (actionPlanHandler != null) actionPlanHandler.Init(this);
        if (actionHandler != null) actionHandler.Init(this);
        if (statHandler != null) statHandler.Init(this);
    }


    public void MoveTo(Vector3 position)
    {
        navMeshAgent.SetDestination(position);
    }


    public void PickUpItem(Item item)
    {
        if (memory.knownItems.Contains(item))
        {
            memory.knownItems.Remove(item);
        }
        inventoryHandler.PickUpItem(item);
    }

}
