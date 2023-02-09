using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Agent))]
public class StationHandler : MonoBehaviour
{
    public Agent agent;

    public void Init(Agent agent)
    {
        this.agent = agent;
    }


    public void Interact(Station station, StationAction stationAction)
    {
        Debug.Log("INTERACTING WITH STATION " + station);
      //  if (!station.Interact(stationAction)) return;
        if (stationAction.requiredItem != null)
        {
            if (agent.inventoryHandler.HasItem(stationAction.requiredItem))
            {
                if (stationAction.destroyItemOnUse)
                {
                    agent.inventoryHandler.DestroyItem(stationAction.requiredItem);
                }
            }
        }
        foreach(ItemData returnItem in  stationAction.returnItems)
        {
            agent.inventoryHandler.AddItem(returnItem);
        }

        if (stationAction.motivationStatName != MotivationStatName.NULL) agent.statHandler.motivationStats.Find(x => x.motivationStatName == stationAction.motivationStatName).currentVal += stationAction.motivationStatVal;
    }
}
