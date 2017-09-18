using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlockingData : Singleton<GlobalFlockingData> {

    protected GlobalFlockingData() {}

    public int SeperationCase = 1;
    public bool UseNormalizedVectors = true;

    public List<Transform> AgentList;
    public List<Rigidbody> AgentBodies;

    public float CohesionRadius = 1f;
    public float AlignmentRadius = 0.75f;
    public float SeperationRadius = 0.5f;

    public float CohesionInfuence = 0.7f;
    public float AlignmentInfluence = 0.6f;
    public float SeperationInfuence = 0.5f;
    public float WanderingInfuence = 0.2f;

    public float Speed = 3f;

   

    void Start () {
        List<GameObject> units = new List<GameObject>(
            GameObject.FindGameObjectsWithTag("SelectableUnit"));
        AgentList = new List<Transform>();
        AgentBodies = new List<Rigidbody>();

        foreach (GameObject obj in units)
        {
            AgentBodies.Add(obj.GetComponent<Rigidbody>());
            AgentList.Add(obj.transform);
        }
    }
	
}
