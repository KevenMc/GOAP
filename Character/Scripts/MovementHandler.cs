using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class MovementHandler : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;


    public void Init(Agent agent)
    {
        navMeshAgent = agent.navMeshAgent;
    }

    public void MoveTo(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
    }

}
