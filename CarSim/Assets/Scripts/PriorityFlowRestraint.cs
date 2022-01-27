using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityFlowRestraint : FlowRestraint
{
    public List<Trajectory> isPrioritizedBy;
    public int rank=0;
    public override bool DoesRestrictFlow()
    {
        bool output = false;
        for(int i = 0; i < isPrioritizedBy.Count; i++)
        {
            if(isPrioritizedBy[i].vehicles.Count > 0 && isPrioritizedBy[i].flowing)
            {
                if (isPrioritizedBy[i].nextPaths.Count == 0) output = true;
                for(int i2 = 0; i2 < isPrioritizedBy[i].nextPaths.Count; i2++)
                {
                    if (isPrioritizedBy[i].nextPaths[i2].trajectory.flowing)
                    {
                        output = true;
                        break;
                    }
                }
                

            }
            PriorityFlowRestraint pflowRestraint;
            if (isPrioritizedBy[i].TryGetComponent<PriorityFlowRestraint>(out pflowRestraint))
            {
                if (pflowRestraint.rank >= rank)
                {
                    rank = pflowRestraint.rank + 1;
                    if (rank > 200)
                    {
                        rank = 0;
                        output = false;
                        break;
                    }
                }
            }
        }
        
        if (!output) rank = 0;
        return output;
        //return base.DoesRestrictFlow();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
