using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : Singleton<AgentManager> {

    protected AgentManager() { }

    public List<Rigidbody> agentBodys;
    public List<Transform> agentTransforms;
    public float agentHeightOffset = 1f;
    public int agentCount = 0;

    GameObject agentHolder;

    public void CirclePreset()
    {
        float circleStep = 360f / agentCount;
        float currentAngle = 0f;

        Vector3 mapMiddle = Utility.Instance.CoordToVec3(
            (Utility.Instance.Width-1) / 2,
            (Utility.Instance.Height-1) / 2);
        Vector3 mapEdge = Utility.Instance.CoordToVec3(
            Utility.Instance.Width-1,
            (Utility.Instance.Height-1) / 2);

        Vector3 midToMapEdge = mapEdge - mapMiddle;

        foreach(Rigidbody body in agentBodys)
        {
            Quaternion rot = Quaternion.AngleAxis(currentAngle, Vector3.forward);
            Vector3 pos = rot * midToMapEdge + mapMiddle;
            Vector3 goal = -1 * (rot * midToMapEdge) + mapMiddle;
            pos.z += agentHeightOffset;
            goal.z += agentHeightOffset;

            body.transform.position = pos;
            body.transform.SendMessage("SetWaypoint", goal);
            currentAngle += circleStep;
        }
    }

    public void SideToSidePreset()
    {
        Vector3 mapBottomRight = Utility.Instance.CoordToVec3(
            0,
            0);

        Vector3 mapTopRight = Utility.Instance.CoordToVec3(
            0,
            (Utility.Instance.Height - 1));

        float DistanceTopToBot = (mapTopRight - mapBottomRight).magnitude;
        float step = DistanceTopToBot / (agentCount / 2f);
        float currentStep = 0f;

        Vector3 mapMiddle = Utility.Instance.CoordToVec3(
            (Utility.Instance.Width - 1) / 2,
            0);
        

        Vector3 midToMapEdge = mapBottomRight - mapMiddle;
        bool agentDivider = true;
        foreach (Rigidbody body in agentBodys)
        {
            Vector3 currentMiddle = mapMiddle + Vector3.up * currentStep;
            if (agentDivider == true)
            {
                Vector3 pos = (currentMiddle + midToMapEdge);
                Vector3 goal = currentMiddle - midToMapEdge;
                pos.z += agentHeightOffset;
                goal.z += agentHeightOffset;
                body.transform.position = pos;
                body.transform.SendMessage("SetWaypoint", goal);
                currentStep += step;
                agentDivider = false;
            }
            else
            {
                Vector3 pos = (currentMiddle - midToMapEdge);
                Vector3 goal = currentMiddle + midToMapEdge;
                pos.z += agentHeightOffset;
                goal.z += agentHeightOffset;
                body.transform.position = pos;
                body.transform.SendMessage("SetWaypoint", goal);
                agentDivider = true;
            }
        }
    }

    public void AddAgent(Vector3 pos)
    {
        GameObject agent = (GameObject)Instantiate(
            Resources.Load("Agent"),
            pos,
            Quaternion.identity);
        agent.transform.parent = agentHolder.transform;
        agentBodys.Add(agent.GetComponent<Rigidbody>());
        agentTransforms.Add(agent.transform);
        agentCount++;
    }

    public void RemoveAgent(GameObject agent)
    {
        if (agent.tag != "SelectableUnit")
        {
            Debug.Log("tried to delete non-agent");
            return;
        }

        agentBodys.Remove(agent.GetComponent<Rigidbody>());
        agentTransforms.Remove(agent.transform);
        Destroy(agent);
        agentCount--;
    }

    void Awake()
    {
        agentHolder = new GameObject("Agent Holder");
        List<GameObject> units = new List<GameObject>(
            GameObject.FindGameObjectsWithTag("SelectableUnit"));
        agentTransforms = new List<Transform>();
        agentBodys = new List<Rigidbody>();

        foreach (GameObject obj in units)
        {
            agentBodys.Add(obj.GetComponent<Rigidbody>());
            agentTransforms.Add(obj.transform);
            obj.transform.parent = agentHolder.transform;
        }
        agentCount = units.Count;
    }

    public void Load(List<GameObject> AgentObj/*, List<Vector3> pos, List<Vector3> goals*/)
    {
        Destroy(agentHolder);
        agentHolder = new GameObject("Agent Holder");
        agentBodys.Clear();
        agentTransforms.Clear();
        agentCount = 0;

        foreach(GameObject obj in AgentObj)
        {
            agentBodys.Add(obj.GetComponent<Rigidbody>());
            agentTransforms.Add(obj.transform);
            obj.transform.parent = agentHolder.transform;
           // obj.transform.position = pos[agentCount];
            //obj.GetComponent<SimpleFlocking>().Goal = goals[agentCount];
           // obj.GetComponent<CC_AgentData>().Goal = goals[agentCount];
           // obj.GetComponent<SimpleNavigation>().SetWaypoint(goals[agentCount]);
            agentCount++;
        }
    }
}
