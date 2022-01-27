using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[System.Serializable]
public class Trajectory : MonoBehaviour
{
    public Transform start;
    public Transform end;
    Vector3[] points;
    public Intersection intersection;
    public Trajectory[] yieldPaths;
    public List<int[]> yieldIntersections;
    public const int pathResolution = 1000;
    public Vector3[] path;
    float[] distances;
    public float[] waypoints;
    public List<Vehicle> vehicles;
    public float entryDistance = 20;
    public bool flowing = true;
    public bool hasGenerated = false;

    [System.Serializable]
    public struct PossibleTrajectory
    {
        public Trajectory trajectory;
        public float chance;
    }
    public List<PossibleTrajectory> nextPaths;
    public bool doesGenerateVehicles = false;

    public int vehicleLimit = 0;

    public List<FlowRestraint> flowRestraints;
    public bool restricted;


    
    [System.Serializable]
    public struct AllowedVehicle
    {
        public GameObject template;
        public float generationChance;
    }
    public AllowedVehicle[] allowedVehicles;
    public float spawnInterval;
    public float spawnTimer;
    public bool spawnRandomness=false;
    public int queuedVehicles = 0;

    public float averageStopTime;
    public List<float> stoppingTimes=new List<float>();
    public List<float> endTimes = new List<float>();
    public void RecalculateAverageStopTime()
    {
        averageStopTime = 0;
        for(int i = 0; i < stoppingTimes.Count; i++)
        {
            averageStopTime += stoppingTimes[i];
        }
        averageStopTime /= stoppingTimes.Count;
    }
    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnInterval;
        Recalculate();
        RestraintCheck();
        /*float avgSize = tSize / pathResolution;
        float err = 0;
        int erInd = 0;
        for (int i = 1; i < pathResolution; i++)
        {
            float e = Mathf.Abs(((path[i] - path[i - 1]).magnitude) - avgSize);
            if (e > err)
            {
                erInd = i;
                err = e;
            }
        }
        Debug.Log("Avg:" + avgSize + " Err:" + err);*/
    }
    public void RestraintCheck()
    {
        int topPriority = 0;
        restricted = false;
        for (int i = 0; i < flowRestraints.Count; i++)
        {
            if (flowRestraints[i].priority >= topPriority)
            {
                restricted |= flowRestraints[i].DoesRestrictFlow();
            }
            
        }
    }
    public void Recalculate()
    {

        hasGenerated = true;
        yieldIntersections = new List<int[]>();

        vehicles = new List<Vehicle>();
        path = new Vector3[pathResolution];
        distances = new float[pathResolution - 1];
        waypoints = new float[pathResolution - 1];
        points = new Vector3[4];
        points[3] = start.position;
        points[0] = end.position;
        points[2] = transform.GetChild(0).position;
        points[1] = transform.GetChild(1).position;
        //float tSize = 0;
        path[0] = GetLocationT(0);
        for (int i = 1; i < pathResolution; i++)
        {
            path[i] = GetLocationT((double)(i) / (double)pathResolution);
            if (i != pathResolution - 1)
            {
                distances[i] = (path[i] - path[i - 1]).magnitude;
                //tSize += distances[i - 1];
                waypoints[i] = distances[i - 1] + waypoints[i - 1];
            }
        }
        for(int i = 0; i < yieldPaths.Length; i++)
        {
            yieldIntersections.Add(Trajectory.CrossingPoint(this, yieldPaths[i]));
        }
    }
    public Vector3 GetLocationT(double t)
    {
        t = System.Math.Min(System.Math.Max(0, t), 1);
        Vector3 res = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            res += (float)(((i == 1 || i == 2) ? 3 : 1) * System.Math.Pow(t, (double)(3 - i)) * System.Math.Pow(1.0 - t, (double)i)) * points[i];
        }
        return res;
    }
    public Vector3 DerivativeT(double t)
    {
        t = 1 - System.Math.Min(System.Math.Max(0, t), 1);
        return (float)(3 * System.Math.Pow(1 - t, 2.0)) * (points[1] - points[0]) + (float)(6 * (1 - t) * t) * (points[2] - points[1]) + (float)(3 * System.Math.Pow(t, 2.0)) * (points[3] - points[2]);
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (
            points[3] != start.position ||
            points[0] != end.position ||
            points[2] != transform.GetChild(0).position ||
            points[1] != transform.GetChild(1).position
        )
        {
            Recalculate();
        }
#endif
        RestraintCheck();
        if (!spawnRandomness)
        {
            spawnTimer -= Time.deltaTime;
        }
        else if (Random.value < (Time.deltaTime / spawnInterval))
        {
            queuedVehicles++;
        }
        if ((!spawnRandomness && spawnTimer <= 0) || (spawnRandomness && queuedVehicles>0))
        {
            RestraintCheck();
            if (doesGenerateVehicles)
            {
                if (flowing) //&& !trajectories[i].restricted)
                {
                    if (vehicles.Count < vehicleLimit || vehicleLimit == 0)
                    {
                        if (vehicles.Count > 0)
                        {
                            if (vehicles[vehicles.Count - 1].progress > entryDistance)
                            {
                                //create new vehicle
                                CreateVehicle();
                            }
                        }
                        else
                        {
                            CreateVehicle();
                        }

                    }
                }
            }
        }
        
        
    }
    public void CreateVehicle()
    {
        spawnTimer = spawnInterval;
        queuedVehicles--;
        //if (!trajectory.doesGenerateVehicles) return;
        GameObject carTemplate = allowedVehicles[allowedVehicles.Length - 1].template;
        float key = Random.value;
        float threshold = 0;
        for (int i = 0; i < allowedVehicles.Length - 1; i++)
        {
            threshold += allowedVehicles[i].generationChance;
            if (key > threshold)
            {
                carTemplate = allowedVehicles[i].template;
            }
        }

        Vehicle car = Instantiate(carTemplate).GetComponent<Vehicle>();
        car.intersection = intersection;
        car.progress = 0;
        car.trajectory = this;
        car.pos = 0;
        car.transform.position = path[0];
        car.firstTrajectory = this;
        //car.GetComponent<CircleCollider2D>().enabled = true;
        //for(int i = 0; i < trajectory.yieldPaths.Length; i++)
        //{
        //car.shouldYield.Add(true);
        //car.yieldAmount.Add(0);
        //}
        vehicles.Add(car);
        RestraintCheck();
    }
    public static int[] CrossingPoint(Trajectory a,Trajectory b)
    {
        if (!a.hasGenerated) a.Recalculate();
        if (!b.hasGenerated) b.Recalculate();
        float smallestDistance = float.MaxValue;
        int[] output = new int[2];
        for(int i = 0; i < a.path.Length-1; i++)
        {
            for(int i2 = 0; i2 < b.path.Length-1; i2++)
            {
                float d = (a.path[i] - b.path[i2]).sqrMagnitude;
                if (d < smallestDistance)
                {
                    
                    smallestDistance = d;
                    output =new int[]{i, i2};
                    if (smallestDistance < 0.1f) return output;
                    //if (smallestDistance < 0.01f) return output;
                    //output = b.path[i2] - (a.path[i] - b.path[i2])/2;
                }
            }
        }
        return output;
    }
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        
        DrawSegments(Selection.Contains(transform.GetChild(0).gameObject) || Selection.Contains(transform.GetChild(1).gameObject));

    }
    private void OnDrawGizmosSelected()
    {
        DrawSegments(true);
    }
    void DrawSegments(bool selected)
    {
        Gizmos.color = ((selected) ? Color.cyan : (flowing?Color.green:Color.red));
        if (points==null) Recalculate();
        if (hasGenerated == false || points[3] != start.position || points[0] != end.position || points[2] != transform.GetChild(0).position || points[1] != transform.GetChild(1).position)
        
            Recalculate();
        
        for(int i=1;i< path.Length; i++)
        {
            try
            {
                Gizmos.DrawLine(path[i - 1], path[i]);
            }
            catch(System.Exception e)
            {
                Recalculate();
            }
        }
    }
#endif
}
