using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMarqee : MonoBehaviour {

    public Texture marqueeGraphics;
    private Vector2 marqueeOrgin;
    private Vector2 marqueeSize;

    public Rect marqueeRect;
    private Rect backUpRect;

    public List<GameObject> selectedUnits;
    List<GameObject> tempUnits;
    private GameObject map;

    float invertedY;

    // projection stuff
    private float d;
    private Vector3 normal;

    // map bounds for agents
    private Vector3 minV3;
    private Vector3 maxV3;

    private void OnGUI()
    {
        marqueeRect = new Rect(marqueeOrgin.x, marqueeOrgin.y,
                                marqueeSize.x, marqueeSize.y);
        GUI.color = new Color(0, 0, 0, 0.3f);
        GUI.DrawTexture(marqueeRect, marqueeGraphics);
    }

	void Start () {
        if (marqueeGraphics == null)
            Debug.Log("CameraMarquee Script: Marquee graphic not set!");
        selectedUnits = new List<GameObject>();

       // this for finding projected points for agents
       Vector3 point = Utility.Instance.MapPosition;
       normal = Vector3.forward;
       d = -1f * (point.z * normal.z);


       // keeps agent within map bounds
       minV3 = Utility.Instance.CoordToVec3(0, 0);
       minV3.x -= Utility.Instance.TileWidth / 2f;
       minV3.y -= Utility.Instance.TileHeight / 2f;

       maxV3 = Utility.Instance.CoordToVec3(Utility.Instance.Width - 1,
                                            Utility.Instance.Height - 1);
       maxV3.x += Utility.Instance.TileWidth / 2f;
       maxV3.y += Utility.Instance.TileHeight / 2f;
    }
	
	void Update () {

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float t = -1f * (Vector3.Dot(ray.origin, normal) + d) 
                            / (Vector3.Dot(ray.direction, normal));

            Vector3 projectedPoint = ray.origin + t * ray.direction;

            
            projectedPoint.x = Mathf.Clamp(projectedPoint.x, minV3.x, maxV3.x);
            projectedPoint.y = Mathf.Clamp(projectedPoint.y, minV3.y, maxV3.y);
            
            if (selectedUnits.Count == 1)
            {
                Vector3 waypoint = projectedPoint;
                if (MovementManager.Instance.UseAStar == true)
                {
                    Vector3 start = selectedUnits[0].transform.position;
                    AStar.Instance.NewRequest(start, waypoint, selectedUnits[0]);
                } 
                else
                {
                    selectedUnits[0].SendMessage("SetWaypoint", waypoint, SendMessageOptions.DontRequireReceiver);
                }
                return;
            }

            if (MovementManager.Instance.UseAStar == true)
            {
                if (selectedUnits.Count <= 0)
                    return;
                Vector3 waypoint = projectedPoint + Vector3.forward;
                AStar.Instance.NewRequest(waypoint, selectedUnits);
            }
            else
            {
                foreach (GameObject agent in selectedUnits)
                {
                    Vector3 randDir = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
                    randDir.Normalize();
                    Vector3 waypoint = projectedPoint + Vector3.forward + randDir * Random.Range(0f, 2f);
                    agent.SendMessage("SetWaypoint", waypoint, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

		if (Input.GetMouseButtonDown(0))
        {
            tempUnits = new List<GameObject>(
                GameObject.FindGameObjectsWithTag("SelectableUnit"));

            invertedY = Screen.height - Input.mousePosition.y;
            marqueeOrgin = new Vector2(Input.mousePosition.x, invertedY);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                tempUnits.Remove(hit.transform.gameObject);

                if (hit.transform.tag == "SelectableUnit")
                    if (!selectedUnits.Contains(hit.transform.gameObject))
                        selectedUnits.Add(hit.transform.gameObject);

                hit.transform.gameObject.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            marqueeSize = Vector2.zero;
            marqueeRect.width = 0;
            marqueeRect.height = 0;
            backUpRect.height = 0;
            backUpRect.width = 0;
            Debug.Log("Agents Selected: " + selectedUnits.Count);
        }
        if (Input.GetMouseButton(0))
        {
            // top down (from screen) mouse position
            invertedY = Screen.height - Input.mousePosition.y;
            marqueeSize = new Vector2(Input.mousePosition.x - marqueeOrgin.x,
                                           (marqueeOrgin.y - invertedY) * -1f);

            if (marqueeRect.width < 0)
            { 
                backUpRect = new Rect(marqueeRect.x - Mathf.Abs(marqueeRect.width), marqueeRect.y,
                                    Mathf.Abs(marqueeRect.width), marqueeRect.height);
            }
            else if (marqueeRect.height < 0)
            {
                backUpRect = new Rect(marqueeRect.x, marqueeRect.y - Mathf.Abs(marqueeRect.height),
                                      marqueeRect.width, Mathf.Abs(marqueeRect.height));
            }
            if (marqueeRect.width < 0 && marqueeRect.height < 0)
            {
                backUpRect = new Rect(marqueeRect.x - Mathf.Abs(marqueeRect.width), marqueeRect.y - Mathf.Abs(marqueeRect.height),
                                    Mathf.Abs(marqueeRect.width), Mathf.Abs(marqueeRect.height));
            }
            

            foreach (GameObject agent in tempUnits)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(agent.transform.position);
                Vector2 screenPoint = new Vector2(screenPos.x, Screen.height - screenPos.y);

                if (!marqueeRect.Contains(screenPoint) || !backUpRect.Contains(screenPoint))
                {
                    agent.SendMessage("OnUnselect", SendMessageOptions.DontRequireReceiver);
                    if (selectedUnits.Contains(agent))
                        selectedUnits.Remove(agent);
                }
                if (marqueeRect.Contains(screenPoint) || backUpRect.Contains(screenPoint))
                {
                    if (!selectedUnits.Contains(agent))
                    {
                        selectedUnits.Add(agent);
                        agent.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }

	}
}
