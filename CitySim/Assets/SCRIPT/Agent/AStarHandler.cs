using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarHandler : MonoBehaviour {

    public List<Vector3> WaypointList;
    public float acceptableRadius = 0.25f;
    public float radiusSq;
    bool isSelected = false;
    bool waypointSet = false;
    Vector3 currentWaypoint;
    LineRenderer line;

    public void OnSelect()
    {
        isSelected = true;
    }

    public void OnUnselect()
    {
        isSelected = false;
    }

    void Start()
    {
        radiusSq = acceptableRadius * acceptableRadius;
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.2f;
        line.endWidth = 0.1f;
        line.enabled = false;
    }

    void SetWaypointList(List<Vector3> waypoints)
    {
        WaypointList.Clear();
        waypointSet = false;
        foreach (Vector3 vec in waypoints)
        {
            WaypointList.Add(new Vector3(vec.x,vec.y,vec.z));
        }
        line.positionCount = WaypointList.Count;
        for (int i = 0; i < WaypointList.Count; i++)
        {
            line.SetPosition(i, WaypointList[i] + Vector3.forward * 0.4f);
        }
        //line.SetPositions(WaypointList.ToArray());
       
    }

    void Update()
    {
        if (MovementManager.Instance.UseAStar == false)
            return;

        DB_Line dbline = DebugManager.Instance.DebugLine;

        // for debug line drawing
        if (dbline == DB_Line.Astar)
        {
            if (WaypointList.Count > 0)
                line.enabled = true;
            else
                line.enabled = false;
        }
        else if (dbline != DB_Line.AgentGoal)
            line.enabled = false;
            

        // for movement
        if (WaypointList.Count > 0)
        {
            if (waypointSet == false)
            {
                currentWaypoint = WaypointList[0] + Vector3.forward;
                SendMessage("SetWaypoint",
                    currentWaypoint + Vector3.forward,
                    SendMessageOptions.DontRequireReceiver);

                if (WaypointList.Count > 1)
                    WaypointList.RemoveAt(0);
                waypointSet = true;
            }
            else
            {
                Vector3 pos = transform.position;
                float magSq = (pos - currentWaypoint).sqrMagnitude;
                if (magSq < radiusSq)
                {
                    waypointSet = false;
                }
            }
        }
    }
}
