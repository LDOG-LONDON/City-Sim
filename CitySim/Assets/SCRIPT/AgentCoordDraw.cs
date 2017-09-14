using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCoordDraw : MonoBehaviour {

    Map map;
    Color prev = Color.white;
    public Color cur = Color.black;
    Tile prevTile;
    Tile nextTile;
    Vector3 coord;
    Rigidbody body;

    public void ChangeColor(Tile tile, Color color)
    {
        if (tile.Prefab != null)
            tile.Prefab.GetComponent<MeshRenderer>().material.color = color;
    }

    public Color GetColor(Tile tile)
    {
        if (tile.Prefab != null)
            return tile.Prefab.GetComponent<MeshRenderer>().material.color;
        return Color.black;
    }

    void Start () {
        map = GameObject.Find("Map").GetComponent<Map>();
        body = GetComponent<Rigidbody>();
        coord = Utility.Instance.Vec3ToCoord(transform.position);
        prevTile = map.Grid[(int)coord.x, (int)coord.y];
        //prev = GetColor(prevTile);
        
	}
	
	void Update () {
        //if (Utility.Instance.DebugInfo == false)
            return;

		if (map != null)
        {
            if (body.velocity.sqrMagnitude <= 0.01)
                return;

            coord = Utility.Instance.Vec3ToCoord(transform.position);
            nextTile = map.Grid[(int)coord.x, (int)coord.y];
            if (prevTile == nextTile)
                return;
            else
            {
                ChangeColor(prevTile, prev);
                prevTile = nextTile;
                //prev = GetColor(nextTile);
                ChangeColor(nextTile, cur);
            }
        }
	}
}
