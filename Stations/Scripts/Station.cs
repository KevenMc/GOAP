using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public Transform stand;
    public float stationCost;
    public List<StationType> stationType;
    public bool inUse;
    public Agent currentAgent;
    public List<StationAction> stationActions;
    public InventoryData inventoryData;

    public bool hasCountDown;
    public bool canUse = true;
    public float currentUseTime = 0;
    public float useTimout = 240;



    private void OnTriggerEnter(Collider other)
    {}

    private void FixedUpdate()
    {
        if(hasCountDown) Counter(Time.deltaTime);
    }

    private void Counter(float delta)
    {
        if (currentUseTime <= 0 ) return;
        currentUseTime -= delta;
        if(currentUseTime <= 0) canUse = true;
        else canUse = false;
    }

    public void UseStation(Agent agent, StationAction stationAction)
    {

        if(Vector3.Distance(agent.transform.position, stand.position) < 2)
        {
            agent.navMeshAgent.ResetPath();
            agent.transform.position = stand.position;
        }
    }

    public bool HasActionItem(ItemData itemData)
    {
        if (stationActions == null) return false;
        return (stationActions.Find(x => x.returnItems.Contains(itemData)));
    }


    public void Interact(Interaction interaction)
    {
        if (!canUse) return;
        if (hasCountDown)
        {
            currentUseTime = useTimout;
            canUse = false;
        }

    }

}
