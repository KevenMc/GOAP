using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    public List<MotivationStat> motivationStats;
    public Agent agent;

    private float incrementTimer;
    public float incrementTimerVal;

    public void Init(Agent agent)
    {
        this.agent = agent;
        motivationStats = new List<MotivationStat>(GetComponents<MotivationStat>());
    }


    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        Counter(delta);
    }

    
    private void Counter(float delta)
    {
        incrementTimer += delta;
        if (incrementTimer >= incrementTimerVal)
        {
            IncrementMotivationStats();
            CalculatePriority();
            incrementTimer = 0;
        }
    }


    private void IncrementMotivationStats()
    {
        foreach (MotivationStat stat in motivationStats) if (!stat.isStorage) stat.currentVal += stat.incrementVal;
    }


    public void CalculatePriority()
    {
        foreach(MotivationStat mstat in motivationStats)
        {
            if(mstat.isStorage) mstat.currentPriority = mstat.priority * (mstat.quantity - mstat.inventoryData.items.FindAll(x => x == mstat.itemData).Count);
            else mstat.currentPriority = (mstat.currentVal - mstat.triggerVal) * mstat.priority;
        }
        motivationStats.Sort(SortByPriority);
    }


    private static int SortByPriority(MotivationStat x, MotivationStat y)
    {
        if (x.currentPriority < y.currentPriority) return 1;
        if (x.currentPriority > y.currentPriority) return -1;
        return 0;
    }
}
