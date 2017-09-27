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

    public float CohesionInfuence = 0.5f;
    public float AlignmentInfluence = 0.2f;
    public float SeperationInfuence = 40f;
    public float WanderingInfuence = 0.5f;

    public float Speed = 3f;

   

    void Start () {
        AgentList = AgentManager.Instance.agentTransforms;
        AgentBodies = AgentManager.Instance.agentBodys;
    }
	
}
