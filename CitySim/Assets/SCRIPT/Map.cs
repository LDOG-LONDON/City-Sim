using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public Transform TilePrefab = null;

    float TileWidth = 1f;
    float TileHeight = 1f;

    public uint Width = 8;
    public uint Height = 8;

    List<Tile> TileList = null;
    List<Zone> ZoneList = null;

    //public Vector3 CoordToVec3(int x, int y)
    //{
    //    return new Vector3((-(Width - 1) * TileWidth / 2f) + (TileWidth) * x + transform.position.x,
    //                       (-(Height - 1) * TileHeight / 2f) + (TileHeight) * y + transform.position.y,
    //                       transform.position.z - 1f);
    //}
    //
    //public Vector3 Vec3ToCoord(Vector3 pos)
    //{
    //    float X = ((Mathf.Round(pos.x * 100.0f) / 100.0f) + ((Width + 1) * TileWidth / 2f) - transform.position.x) / TileWidth;
    //    float Y = ((Mathf.Round(pos.y * 100.0f) / 100.0f) + ((Height + 1) * TileHeight / 2f) - transform.position.y) / TileHeight;
    //    float Z = transform.position.z + 1f;
    //    return new Vector3(X, Y, Z);
    //}

    void Awake () {
        Utility.Instance.Height = Height;
        Utility.Instance.Width = Width;
        Utility.Instance.TileHeight = TileHeight;
        Utility.Instance.TileWidth = TileWidth;

        if (TilePrefab == null)
        {
            Debug.Log("Awake: Tile Prefab not set.");
            return;
        }

        TileList = new List<Tile>();
		for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                Vector3 tilePos = Utility.Instance.CoordToVec3(x, y);
                Transform obj = Instantiate(TilePrefab, tilePos, Quaternion.identity, transform);
                obj.localScale = new Vector3(TileWidth, TileHeight, 0.5f);
                Tile newTile = new Tile(x, y, obj);

                TileList.Add(newTile);
            }

        
	}

    void Start()
    {

    }
	
	void Update () {
		
	}
}
