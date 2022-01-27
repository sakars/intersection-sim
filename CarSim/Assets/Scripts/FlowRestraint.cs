using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowRestraint : MonoBehaviour
{
    public int priority=0;
    public bool lowerPriorityOverrideWhenTrue = true;
    public virtual bool DoesRestrictFlow()
    {
        return false;
    }
}
