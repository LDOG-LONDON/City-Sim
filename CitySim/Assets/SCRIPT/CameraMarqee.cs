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

    float invertedY;

    private void OnGUI()
    {
        marqueeRect = new Rect(marqueeOrgin.x, marqueeOrgin.y,
                                marqueeSize.x, marqueeSize.y);
        GUI.color = new Color(0, 0, 0, 0.3f);
        GUI.DrawTexture(marqueeRect, marqueeGraphics);
    }

	// Use this for initialization
	void Start () {
        if (marqueeGraphics == null)
            Debug.Log("CameraMarquee Script: Marquee graphic not set!");
        selectedUnits = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(1))
        {
            foreach (GameObject agent in selectedUnits)
                agent.SendMessage("OnUnselect", SendMessageOptions.DontRequireReceiver);
            selectedUnits.Clear();
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

                //if (!marqueeRect.Contains(screenPoint) || !backUpRect.Contains(screenPoint))
                //{
                //    agent.SendMessage("OnUnselect", SendMessageOptions.DontRequireReceiver);
                //}
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
