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
    }

    void SetWaypointList(List<Vector3> waypoints)
    {
        WaypointList.Clear();
        waypointSet = false;
        foreach (Vector3 vec in waypoints)
        {
            WaypointList.Add(new Vector3(vec.x,vec.y,vec.z));
        }
    }

    void Update()
    {
        if (MovementManager.Instance.UseAStar == false)
            return;

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
