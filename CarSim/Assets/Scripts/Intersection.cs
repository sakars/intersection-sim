using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dweiss;
using System;
using System.IO;
using UnityEngine.SceneManagement;
public class Intersection : MonoBehaviour
{
    public Settings settings;
    public float timeScale = 1;
    public float timeLeft=90;
    public float startTime;
    public Trajectory[] printables;
    public struct Nodes
    {
        public GameObject start;
        public GameObject end;
        public float split;
    }
    public Nodes[] trajectoriesToGenerate;
    public GameObject trajectoryPrefab;
    //public GameObject carTemplate;
    public List<Trajectory> trajectories;
    [System.Serializable]
    public struct IntersectionPhase
    {
        public float time;
        public List<Trajectory> activated;

    }
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        if (startTime < 10) PlayerPrefs.DeleteAll();
        int t=PlayerPrefs.GetInt("timesLeftToSim", settings.simTimes);
        PlayerPrefs.SetInt("timesLeftToSim", t-1);
        Debug.Log(t);
        if (t == 0)
        {
            settings.output = PlayerPrefs.GetString("out", "");
            settings.SaveToFile();
            Debug.Log(settings.output);
            string path = "Assets/Resources/test.txt";

            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(PlayerPrefs.GetString("out", ""));
            writer.Close();
            Debug.Log("STOP!!!");
            Application.Quit();
        }
        IntersectionPhase ph = phases[1];
        ph.time = 20 - settings.stopL;
        phases[1]=ph;
        ph = phases[2];
        ph.time = settings.stopL;
        phases[2] = ph;
        timeLeft = settings.simLength;
        timeScale = settings.simSpeed;
        Time.timeScale = timeScale;
        //Debug.Log(timeScale);
        //Debug.Log(Time.timeScale);
        Application.runInBackground = true;
    }
    //public float period = 30;
    //float timer = 0;
    // Update is called once per frame
    float stageTimer = 0;
    public List<IntersectionPhase> phases;
    int phase = 0;
    public float interphase=3;
    float interphaseTimer = 0;
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            if (Time.timeScale != 0)
            {
                string o = "";
                int max = 0;
                for(int i = 0; i < printables.Length; i++)
                {
                    if (printables[i].stoppingTimes.Count > max) max = printables[i].stoppingTimes.Count;
                    //o += printables[i].transform.name+",";
                }
                //o += "\n";
                for(int i = 0; i < max; i++)
                {
                    for(int i2 = 0; i2 < printables.Length; i2++)
                    {
                        if (i < printables[i2].stoppingTimes.Count)
                        {
                            o += (printables[i2].endTimes[i]-startTime)+",";
                            o += printables[i2].stoppingTimes[i];
                        }
                        else { 
                            o += ","; 
                        }
                        if(i2<printables.Length-1)o += ",";
                    }
                    o += "\n";
                }
                Debug.Log(o);
                string pr=PlayerPrefs.GetString("out", "");
                pr +="\n" + o;
                PlayerPrefs.SetString("out", pr);
                Debug.Log(PlayerPrefs.GetString("out"));
                //Time.timeScale = 0;
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
            return;
        }
        stageTimer -= Time.deltaTime;
        if (stageTimer <= 0)
        {
            IntersectionPhase currentPhase = phases[phase];
            for(int i = 0; i < currentPhase.activated.Count; i++)
            {
                currentPhase.activated[i].flowing = false;
            }
            interphaseTimer -= Time.deltaTime;
            if (interphaseTimer <= 0)
            {
                phase = (phase + 1) % phases.Count;
                currentPhase = phases[phase];
                for (int i = 0; i < currentPhase.activated.Count; i++)
                {
                    currentPhase.activated[i].flowing = true;
                }
                stageTimer = currentPhase.time;
                interphaseTimer = interphase;
            }
            
        }
        
        /*if (timer <= 0)
        {
            timer = period;
            Vehicle car = carTemplate.GetComponent<Vehicle>();
            car.intersection = this;
            car.progress = 0;
            car.trajectory = trajectories[0];
            car.pos = 0;
        }
        
        timer -= Time.deltaTime;
        */
    }
    
}
