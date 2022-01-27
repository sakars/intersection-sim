using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleLeave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            Vehicle me = transform.parent.GetComponent<Vehicle>();
            Vehicle vehicleC = collision.transform.parent.GetComponent<Vehicle>();
            for (int i = 0; i < me.trajectory.yieldPaths.Length; i++)
            {
                if (me.trajectory.yieldPaths[i] == vehicleC.trajectory)
                {
                    if (me.waitingList.Contains(vehicleC))
                    {
                        me.waitingList.Remove(vehicleC);
                        me.yieldAmount[i]--;
                        
                    }
                    
                }
            }
        }
    }*/
}
