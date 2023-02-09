using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;


    public virtual void EquipItem(Agent agent)
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        Agent agent = other.gameObject.GetComponent<Agent>();
        if (agent != null)
        {
            agent.itemHandler.AddPickupItem(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        Agent agent = other.gameObject.GetComponent<Agent>();
        if (agent != null)
        {
            agent.itemHandler.RemovePickupItem(this);
        }
    }
}
