using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    
    public Intersection intersection;
    public Trajectory trajectory;
    public float speed=0.01f;
    public float progress=0;
    public int pos=0;
    //public int blockedAmount = 0;
    public bool stopped = true;
    int nextInd = -1;
    public List<string> stopReason = new List<string>();
    public float timeStopped = -1;
    public Trajectory firstTrajectory;
    //public List<Vehicle> waitingList=new List<Vehicle>();
    //public List<int> yieldAmount = new List<int>();
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        stopReason.Clear();
        stopped = false;
        /*distance from intersection check*/
        for(int i = 0; i < trajectory.yieldPaths.Length; i++)
        {
            int crossingIndexSelf = trajectory.yieldIntersections[i][0];
            if (pos < crossingIndexSelf)
            {
                if (trajectory.waypoints[crossingIndexSelf] - progress <2f*transform.localScale.y && trajectory.waypoints[crossingIndexSelf] - progress > transform.localScale.y*0.7f)
                {
                    int crossingIndexOther = trajectory.yieldIntersections[i][1];
                    Trajectory otherT = trajectory.yieldPaths[i];
                    if (otherT.vehicles.Count > 0)
                    {
                        stopped = true;
                        stopReason.Add("The trajectory crossing it is busy.");
                        break;
                    }
                }
            }
            
        }
        //endLoop:
        if (stopCars.Count > 0)
        {
            stopped = true;
            stopReason.Add("The car in front of it is blocking it.");
        }
        //stopped = stopped || (stopCars.Count > 0);
        if (!stopped)
        {
            progress += speed * Time.deltaTime;
            for (int i = pos; i < trajectory.waypoints.Length; i++)
            {
                if (progress > trajectory.waypoints[i])
                {
                    pos = i;
                    transform.position = trajectory.path[i + 1];
                    Vector3 up= -trajectory.DerivativeT((double)(pos + 1) / (double)(Trajectory.pathResolution - 1));
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, up);
                }
            }
            //is at the end of a trajectory
            if (pos + 1 == Trajectory.pathResolution - 1)
            {
                //trajectory.vehicles.Remove(this);
                if (trajectory.nextPaths.Count > 0)
                {

                    if (nextInd == -1)
                    {
                        float v = Random.value;
                        float threshold = 0;
                        nextInd = trajectory.nextPaths.Count - 1;
                        for(int i = 0; i < trajectory.nextPaths.Count - 1; i++)
                        {
                            threshold += trajectory.nextPaths[i].chance;
                            if (threshold > v)
                            {
                                nextInd = i;
                            }
                        }
                    }
                    Trajectory nextPath = trajectory.nextPaths[nextInd].trajectory;
                    bool isLimitless = nextPath.vehicleLimit == 0;
                    bool hasSpace = nextPath.vehicleLimit > nextPath.vehicles.Count;
                    if (nextPath.flowing && (isLimitless || hasSpace) && !nextPath.restricted)
                    {
                        trajectory.vehicles.Remove(this);
                        progress -= trajectory.waypoints[trajectory.waypoints.Length - 1];
                        pos = 0;
                        trajectory = trajectory.nextPaths[nextInd].trajectory;
                        trajectory.vehicles.Add(this);
                        nextInd = -1;
                    }
                    else
                    {
                        if (timeStopped == -1)
                        {
                            timeStopped = Time.time;
                        }
                        progress = trajectory.waypoints[trajectory.waypoints.Length - 1];
                        stopReason.Add("The next trajectory is busy.");
                    }
                }
                else
                {
                    if (timeStopped == -1)
                    {
                        firstTrajectory.stoppingTimes.Add(0);
                    }
                    else
                    {
                        firstTrajectory.stoppingTimes.Add(Time.time - timeStopped);
                    }
                    firstTrajectory.RecalculateAverageStopTime();
                    firstTrajectory.endTimes.Add(Time.time);
                    trajectory.vehicles.Remove(this);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if (timeStopped == -1)
            {
                timeStopped = Time.time;
            }
        }
        /*stopped = false;
        for(int i = 0; i < waitingList.Count; i++)
        {
            if (waitingList[i])
            {
                stopped = stopped || !waitingList[i].stopped;
            }
            else
            {
                waitingList.RemoveAt(i);
            }
        }
        stopped = stopped || blockedAmount != 0;
        if (!stopped)
        {
            progress += speed * Time.deltaTime;
            for (int i = pos; i < trajectory.waypoints.Length; i++)
            {
                if (progress > trajectory.waypoints[i])
                {
                    pos = i;
                    transform.position = trajectory.path[i + 1];
                    transform.up = -trajectory.DerivativeT((double)(pos + 1) / (double)(Trajectory.pathResolution - 1));
                }
            }
            if (pos + 1 == Trajectory.pathResolution - 1)
            {
                trajectory.vehicles.Remove(this);
                Destroy(gameObject);
            }
        }
        */
    }

    public List<Vehicle> stopCars=new List<Vehicle>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vehicle v;
        if( collision.transform.CompareTag("Car") && collision.transform.parent.TryGetComponent<Vehicle>(out v) && transform.GetChild(2).GetComponent<BoxCollider2D>().IsTouching(collision))
        {
            if(!v.stopCars.Contains(this)) stopCars.Add(v);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Vehicle v;
        if (collision.transform.CompareTag("Car") && collision.transform.parent.TryGetComponent<Vehicle>(out v)) //&& transform.GetChild(2).GetComponent<BoxCollider2D>().IsTouching(collision))
        {
            stopCars.Remove(v);
        }
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!transform.GetComponent<PolygonCollider2D>().IsTouching(collision)) return;
        if (collision.gameObject.CompareTag("Car"))
        {
            Vehicle vehicleC = collision.transform.parent.GetComponent<Vehicle>();
            for(int i = 0; i < trajectory.yieldPaths.Length; i++)
            {
                if (trajectory.yieldPaths[i] == vehicleC.trajectory)
                {
                    
                    yieldAmount[i]++;
                    waitingList.Add(vehicleC);
                    
                }
            }
            
        }
    }
    */
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            Vehicle vehicleC = collision.transform.parent.GetComponent<Vehicle>();
            for (int i = 0; i < trajectory.yieldPaths.Length; i++)
            {
                if (trajectory.yieldPaths[i]==vehicleC.trajectory)
                {
                    waitingList.Remove(vehicleC);
                    yieldAmount[i]--;
                    if(yieldAmount[i]==0) shouldYield[i] = false;
                }
            }
        }
    }*/
}
