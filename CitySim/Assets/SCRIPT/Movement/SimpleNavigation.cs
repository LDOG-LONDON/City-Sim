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
    public bool isOn = false;

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

        MV_Type type = MovementManager.Instance.MoveType;
        if (type != MV_Type.SimpleNav)
            return;

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
