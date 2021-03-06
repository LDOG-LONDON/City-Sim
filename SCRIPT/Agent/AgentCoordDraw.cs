﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCoordDraw : MonoBehaviour {

    Map map;
    Color prev = Color.white;
    public Color cur = Color.black;
    TrailRenderer trail;
    Tile prevTile;
    Tile nextTile;
    Vector3 coord;
    Rigidbody body;

    public void ChangeColor(Tile tile, Color color)
    {
        if (tile != null && tile.Prefab != null)
            tile.Prefab.GetComponent<MeshRenderer>().material.color = color;
    }

    public Color GetColor(Tile tile)
    {
        if (tile != null && tile.Prefab != null)
            return tile.Prefab.GetComponent<MeshRenderer>().material.color;
        return Color.black;
    }

    void Start () {
        map = GameObject.Find("Map").GetComponent<Map>();
        body = GetComponent<Rigidbody>();
        coord = Utility.Instance.Vec3ToCoord(transform.position);
        prevTile = map.Grid[(int)coord.x, (int)coord.y];
        trail = GetComponent<TrailRenderer>();
        //prev = GetColor(prevTile);
        
	}
	
	void LateUpdate () {
        if (Utility.Instance.DebugInfo == false)
        {
            if (trail.enabled == true)
            {
                ChangeColor(prevTile, Color.white);
                trail.enabled = false;
            }
            return;
        }
           

        if (trail && trail.enabled == false)
            trail.enabled = true;

		if (map != null)
        {
            if (body.velocity.sqrMagnitude <= 0.01)
                return;

            coord = Utility.Instance.Vec3ToCoord(transform.position);
            nextTile = map.Grid[(int)coord.x, (int)coord.y];
            if (!nextTile.Wall)
                ChangeColor(nextTile, cur);
        }
	}
}
