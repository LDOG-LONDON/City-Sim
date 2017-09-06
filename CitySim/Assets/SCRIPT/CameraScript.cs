using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public Transform Cam = null;
    public Transform Focus = null;
    float Distance = 10f;
    float Speed = 0.5f;
    public int touches = 0;

    Vector4 MapBounds;


	void Start () {
        MapBounds = new Vector4();

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
        Cam.transform.position = Focus.transform.position + Vector3.back * -10;
        Cam.LookAt(Focus);
    }


    void Update()
    {
       
        Vector3 newPos = Cam.transform.position;
        newPos.z = Distance;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            newPos.x -= Speed;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            newPos.y -= Speed;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            newPos.x += Speed;
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            newPos.y += Speed;
        }
        Input.simulateMouseWithTouches = true;
        if (Input.touchCount == 2)
        {
            Debug.Log("Touch pad");
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 t0prev = touchZero.position - touchZero.deltaPosition;
            Vector2 t1prev = touchOne.position - touchOne.deltaPosition;

            float prevTouchMag = (t0prev - t1prev).magnitude;
            float touchMag = (touchZero.position - touchOne.position).magnitude;

            float touchDiff = prevTouchMag - touchMag;

            newPos.z += touchDiff * Speed;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            newPos.z -= Speed;
        }
        Mathf.Clamp(newPos.z, 1.5f, 50f);
        Distance = newPos.z;
        Cam.transform.position = newPos;
    }
        
        
}
