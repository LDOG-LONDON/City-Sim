using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public Transform Cam = null;
    public Transform Focus = null;
    float Distance = 10f;
    float Speed = 0.5f;
    public int touches = 0;
    public Vector3 NEW;
    public Vector4 MapBounds;


	void Start () {
        MapBounds = new Vector4();
        Vector3 min = Utility.Instance.CoordToVec3(0, 0);
        int w = (int)Utility.Instance.Width - 1;
        int h = (int)Utility.Instance.Height - 1;
        Vector3 max = Utility.Instance.CoordToVec3(w, h);

        MapBounds.x = min.x - 1; // Min X 
        MapBounds.y = min.y - 1; // Min Y
        MapBounds.z = max.x + 1; // Max X
        MapBounds.w = max.y + 1; // Max Y
        
        Cam = GameObject.Find("Main Camera").transform;
        if (Cam == null)
        {
            Debug.Log("Couldn't find Camera");
            return;
        }

        Focus = GameObject.Find("Map").transform;
        if (Focus == null)
        {
            Debug.Log("No Focus object for Camera");
            return;
        }


        Cam.transform.position = Focus.transform.position + Vector3.back * -Distance;
        Cam.LookAt(Focus);
    }


    void Update()
    {
        Vector3 newPos = Cam.transform.position;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPos -= Speed * Cam.right;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPos -= Speed * Cam.up;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPos += Speed * Cam.right;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPos += Speed * Cam.up;
        }

        if (Input.accelerationEventCount > 0)
        {
            //Debug.Log(Input.mouseScrollDelta);
            newPos += Speed * Cam.transform.forward * Input.mouseScrollDelta.y * 2;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            newPos -= Speed * Cam.transform.forward;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            newPos += Speed * Cam.transform.forward;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Cam.transform.Rotate(Vector3.forward, Speed * 3);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Cam.transform.Rotate(Vector3.forward, -Speed * 3);
        }

        newPos.z = Mathf.Clamp(newPos.z, 1.5f, 50f);
        newPos.x = Mathf.Clamp(newPos.x, MapBounds.x, MapBounds.z);
        newPos.y = Mathf.Clamp(newPos.y, MapBounds.y, MapBounds.w);
        //Distance = newPos.z;
        Cam.transform.position = newPos;
        //NEW = newPos;
        //Cam.LookAt(new Vector3(newPos.x, newPos.y, 1));
    }
        
        
}
