using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public Transform TilePrefab = null;

    public float TileWidth = 1f;
    public float TileHeight = 1f;

    public uint Width = 8;
    public uint Height = 8;

    List<Tile> TileList = null;
    List<Transform> ObjectList;
    public Tile[,] Grid;

    public void ResetMapColor()
    {
        foreach(Transform obj in ObjectList)
        {
            obj.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    void Awake () {
        Utility.Instance.Height = Height;
        Utility.Instance.Width = Width;
        Utility.Instance.TileHeight = TileHeight;
        Utility.Instance.TileWidth = TileWidth;
        Utility.Instance.MapPosition = transform.position;
        
        Grid = new Tile[Width, Height];
        ObjectList = new List<Transform>();

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
                Tile newTile = new Tile(x, y,tilePos, obj);
                ObjectList.Add(obj);
                TileList.Add(newTile);
                Grid[x, y] = newTile;
            }

        AStar.Instance.grid = Grid;
    }

    void Start()
    {

    }
	
	void Update () {
		
	}
}
