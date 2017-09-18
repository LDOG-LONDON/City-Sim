using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFlocking : MonoBehaviour {

    public Vector3 Goal;
    public Vector3 vel;

    private float cohRadiusSq;
    private float aliRadiusSq;
    private float sepRadiusSq;

    private Vector3 cohesion;
    private Vector3 alignment;
    private Vector3 seperation;
    private Vector3 wandering;
    private Vector3 moveTowards;

    private bool toCloseToGoal=false;

    Rigidbody myBody;

    void Start () {

        cohRadiusSq = GlobalFlockingData.Instance.CohesionRadius *
            GlobalFlockingData.Instance.CohesionRadius;
        aliRadiusSq = GlobalFlockingData.Instance.AlignmentRadius *
            GlobalFlockingData.Instance.AlignmentRadius;
        sepRadiusSq = GlobalFlockingData.Instance.SeperationRadius *
            GlobalFlockingData.Instance.SeperationRadius;

        myBody = GetComponent<Rigidbody>();
        Goal = Utility.Instance.CoordToVec3(Utility.Instance.Width / 2, Utility.Instance.Height / 2);
    }
	
    void Cohesion()
    {
        Vector3 groupCenter = Vector3.zero;
        int groupNumber = 0;
        foreach (Rigidbody body in GlobalFlockingData.Instance.AgentBodies)
        {
            if (body == myBody)
                continue;
            float lengthSq = (body.position - myBody.position).sqrMagnitude;
            if (lengthSq <= cohRadiusSq)
            {
                groupNumber++;
                groupCenter += body.position;
            }
        }
        if (groupNumber <= 0)
        {
            cohesion = Vector3.zero;
            return;
        }

        cohesion = myBody.position - (groupCenter / groupNumber);

        if (cohesion.sqrMagnitude > 1f)
            cohesion.Normalize();
    }

    void Seperation()
    {
        int SepCase = GlobalFlockingData.Instance.SeperationCase;
        Vector3 groupCenter = Vector3.zero;
        Vector3 otherAgentVector = Vector3.zero;
        Vector3 AveVel = Vector3.zero;
        int maxGroupNumber = 10000;
        int groupNumber = 0;

        foreach (Rigidbody body in GlobalFlockingData.Instance.AgentBodies)
        {
            if (body == myBody)
                continue;

                float length = (myBody.position - body.position).magnitude;
                if (length <= GlobalFlockingData.Instance.SeperationRadius)
                {
                    if (groupNumber > maxGroupNumber)
                        break;
                    groupNumber++;

                    switch (SepCase)
                    {
                        case 0:
                            groupCenter += body.position;
                            break;
                        case 1:
                            otherAgentVector += (myBody.position - body.position).normalized*(1f-length/ GlobalFlockingData.Instance.SeperationRadius);
                            break;
                        default:
                            otherAgentVector += (myBody.position - body.position).normalized*(1f - length/ GlobalFlockingData.Instance.SeperationRadius);
                            AveVel += body.velocity.normalized * -1f;
                            break;
                    }
                }
        }

        if (groupNumber <= 0)
        {
            seperation = Vector3.zero;
            return;
        }
            
        switch (SepCase)
        {
            case 0:
                seperation =  myBody.position - (groupCenter / groupNumber);
                break;
            case 1:
                seperation = (otherAgentVector / groupNumber);
                break;
            default:
                seperation = ((otherAgentVector + AveVel) / groupNumber);
                break;
        }

        //if (GlobalFlockingData.Instance.UseNormalizedVectors == true)
        //if (seperation.sqrMagnitude > 1f)
        //    seperation.Normalize();
    }

    void Alignment()
    {
        Vector3 AveVel = Vector3.zero;
        int groupNumber = 0;
        foreach (Rigidbody body in GlobalFlockingData.Instance.AgentBodies)
        {
            if (body == myBody)
                continue;

            float length = (myBody.position - body.position).magnitude;
            if (length <= GlobalFlockingData.Instance.AlignmentRadius)
            {  
                groupNumber++;
                AveVel += body.velocity;
            }
        }

        if (groupNumber <= 0)
        {
            alignment = Vector3.zero;
            return;
        }
        alignment = AveVel / groupNumber;

        //if (GlobalFlockingData.Instance.UseNormalizedVectors == true)
        //if (alignment.sqrMagnitude > 1f)
        //    alignment.Normalize();
    }

    void Wandering()
    {
        Vector3 normalVel = myBody.velocity.normalized;
        wandering = normalVel;
        float x = wandering.x;
        wandering.x = -wandering.y;
        wandering.y = x;
        float randVecLength = Random.Range(-0.1f, 0.1f);
        wandering = wandering * randVecLength + normalVel;

        if (GlobalFlockingData.Instance.UseNormalizedVectors == true)
            wandering.Normalize();

    }

    void SetWaypoint(Vector3 goal)
    {
        Goal = goal;
    }

    void TowardsGoal()
    {
        float distFromGoal = (Goal - transform.position).magnitude;

        if (distFromGoal < 0.2f)
            return;

        //if (GlobalFlockingData.Instance.UseNormalizedVectors == true)
        //if (moveTowards.sqrMagnitude > 1f)
        
        moveTowards = (Goal - transform.position).normalized;
            
    }

	void Update () {
        if (!MovementManager.Instance.UseSimpleFlocking)
            return;

        Cohesion();
        Seperation();
        Alignment();
        TowardsGoal();
        Wandering();

        vel = moveTowards +
            alignment * GlobalFlockingData.Instance.AlignmentInfluence +
            cohesion * GlobalFlockingData.Instance.CohesionInfuence +
            seperation * GlobalFlockingData.Instance.SeperationInfuence +
            wandering * GlobalFlockingData.Instance.WanderingInfuence;


        myBody.velocity += vel.normalized * 0.1f;
        if (myBody.velocity.magnitude > GlobalFlockingData.Instance.Speed)
        {
            myBody.velocity = myBody.velocity.normalized * GlobalFlockingData.Instance.Speed;
        }
        vel = myBody.velocity;
	}
}
