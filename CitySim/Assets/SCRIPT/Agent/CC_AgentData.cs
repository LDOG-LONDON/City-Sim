using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_AgentData : MonoBehaviour{

    public Vector2 Goal;
    public Vector2 Velocity;
    public float Radius = 1f; // for density

    public float DensityFn(Vector3 pos)
    {
        float mag = Mathf.Abs((pos - transform.position).magnitude);
        if (mag > Radius)
            return 0f;

        // (x-1)^2

        mag /= Radius;
        mag = mag - 1;
        return mag * mag;
    }
}
