using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Intersection))]
public class IntersectionEditor : Editor
{
    /// <summary>
    /// Gets the coordinates of the intersection point of two lines.
    /// </summary>
    /// <param name="A1">A point on the first line.</param>
    /// <param name="A2">Another point on the first line.</param>
    /// <param name="B1">A point on the second line.</param>
    /// <param name="B2">Another point on the second line.</param>
    /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
    /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
    public Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
    {
        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

        if (tmp == 0)
        {
            Vector2 v1 = (A1 - A2).normalized;
            Vector2 v2 = (B1 - B2).normalized;
            if(v1==v2 || v1 == -v2)
            {
                found = true;
                return (A1 + B1) / 2;
            }
            // No solution!
            found = false;

            return Vector2.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

        found = true;

        return new Vector2(
            B1.x + (B2.x - B1.x) * mu,
            B1.y + (B2.y - B1.y) * mu
        );
    }
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        Intersection intersection = (Intersection)target;
        if (GUILayout.Button("Generate"))
        {
            //Clear Previous trajectories
            Intersection.Nodes[] nodes = intersection.trajectoriesToGenerate;
            Transform trajectorySave = intersection.transform.GetChild(0);
            intersection.trajectories.Clear();
            int childs = trajectorySave.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(trajectorySave.GetChild(i).gameObject);
            }
            int childs2 = intersection.transform.GetChild(1).childCount;
            for (int i = childs2 - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(intersection.transform.GetChild(1).GetChild(i).gameObject);
            }

            foreach (Intersection.Nodes node in nodes)
            {
                Transform P1 = node.start.transform;
                Transform P4 = node.end.transform;
                string name = P1.name + "-" + P4.name;

                Vector2 A1 = P1.position;
                Vector2 A2 = P1.GetChild(0).position;
                Vector2 B1 = P4.position;
                Vector2 B2 = P4.GetChild(0).position;
                bool found;

                Vector3 P2;
                Vector3 P3;
                
                Vector2 vec = GetIntersectionPointCoordinates(A1, A2, B1, B2,out found);
                if (found)
                {
                    P2 = vec;
                    P3 = vec;
                    if (node.split == 0)
                    {
                        generateTrajectory(intersection, P1, P2, P3, P4, name);
                    }
                    else
                    {
                        Vector2 a = P1.position + (P2 - P1.position) * node.split;
                        Vector2 b = P2 + (P3 - P2) * node.split;
                        Vector2 c = P3 + (P4.position - P3) * node.split;
                        Vector2 d = a + (b - a) * node.split;
                        Vector2 e = b + (c - b) * node.split;
                        Vector2 f = d + (e - d) * node.split;

                        GameObject inBetween = new GameObject();
                        inBetween.transform.parent = intersection.transform.GetChild(1);
                        inBetween.transform.position = f;
                        inBetween.name = name + "_waypoint";
                        Trajectory t1=generateTrajectory(intersection, P1, a, d, inBetween.transform, name + "1");
                        Trajectory t2=generateTrajectory(intersection, inBetween.transform, e, c, P4, name + "2");
                        Trajectory.PossibleTrajectory n=new Trajectory.PossibleTrajectory();
                        n.trajectory = t2;
                        n.chance = 0;
                        t1.nextPaths.Add(n);

                    }
                    
                }
                else
                {
                    Debug.LogWarning("Kkas neiet hmmm...");
                    continue;
                }

                
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(intersection);
            EditorSceneManager.MarkSceneDirty(intersection.gameObject.scene);
        }
    }
    public Trajectory generateTrajectory(Intersection intersection,Transform P1, Vector3 P2, Vector3 P3, Transform P4,string name)
    {
        Transform trajectorySave = intersection.transform.GetChild(0);
        GameObject t = Instantiate(intersection.trajectoryPrefab, trajectorySave);

        t.name = name;


        Trajectory tComponent = t.GetComponent<Trajectory>();

        tComponent.start = P1;
        t.transform.GetChild(0).position = P2;
        t.transform.GetChild(1).position = P3;
        tComponent.end = P4;


        intersection.trajectories.Add(tComponent);
        tComponent.intersection = intersection;


        tComponent.Recalculate();
        return tComponent;
    }
}
