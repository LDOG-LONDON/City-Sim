using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNavigation : MonoBehaviour {

    Rigidbody body;
    Vector3 waypoint;
    float speed = 2f;
    bool goTowards = false;
    bool useAStart = false;
    List<Tile> path;
    public bool doneFindingPath = false;

    public void SetWaypoint(Vector3 Waypoint)
    {
        goTowards = true;
        waypoint = Waypoint;
    }

    void Start()
    {
        body = transform.GetComponent<Rigidbody>();
    }
	
	void Update () {

        if (Utility.Instance.DebugInfo == true)
            Debug.DrawRay(transform.position, body.velocity,Color.black);

        if (useAStart)
        {
            if (goTowards)
            {
                Vector3 origin = Utility.Instance.Vec3ToCoord(transform.position);
                Vector3 goal = Utility.Instance.Vec3ToCoord(waypoint);
                Vector4 go = new Vector4((int)goal.x, (int)goal.y, (int)origin.x, (int)origin.y);
                AStar.Instance.NewRequest(go, this);
                goTowards = false;
            }
            return;
        }

		if (goTowards)
        {
            Vector3 movDir = waypoint - transform.position;
            float mag = movDir.magnitude;
            if (mag < 0.05f)
                goTowards = false;
            body.velocity = movDir * speed;
        }
	}
}
