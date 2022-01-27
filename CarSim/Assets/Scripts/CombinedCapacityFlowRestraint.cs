using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinedCapacityFlowRestraint : FlowRestraint
{
    public bool restricts;
    public List<Trajectory> trajectories;
    public int limit = 0;
    public override bool DoesRestrictFlow()
    {
        int c = 0;
        for(int i = 0; i < trajectories.Count; i++)
        {
            c += trajectories[i].vehicles.Count;
        }
        restricts = c >= limit;
        return restricts;
    }
    private void Start()
    {
        for (int i = 0; i < trajectories.Count; i++)
        {
            trajectories[i].flowRestraints.Add(this);
        }
    }
}
