using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForBothFlowRestraint : FlowRestraint
{
    public Trajectory a;
    public Trajectory b;
    public override bool DoesRestrictFlow()
    {
        Vehicle aV = a.vehicles[a.vehicles.Count - 1];
        Vehicle bV = a.vehicles[b.vehicles.Count - 1];
        if(aV.pos==aV.trajectory.waypoints.Length-1 && bV.pos == bV.trajectory.waypoints.Length - 1)
        {
            return false;
        }
        return true;
    }
    public void Start()
    {
        priority = 1;
        lowerPriorityOverrideWhenTrue = false;
    }
}
