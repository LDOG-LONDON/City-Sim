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
    List<Vector2> WallList;
    List<Transform> ObjectList;
    public Tile[,] Grid;

    // for map selection stuff
    // projection stuff
    private float d;
    private Vector3 normal;

    // map bounds for agents
    private Vector3 minV3;
    private Vector3 maxV3;

    public void ResetMapColor()
    {
        for(int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Grid[x, y].Wall)
                    continue;
                Grid[x,y].Prefab.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }

    public void LoadMap(int mapWidth, int mapHeight, float tileWidth, float tileHeight, List<Vector2> walls)
    {
        // set data
        Height = (uint)mapHeight;
        Width = (uint)mapWidth;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        WallList.Clear();
        WallList = walls;

        // delete old stuff
        TileList.Clear();
        foreach(Transform trans in ObjectList)
        {
            Destroy(trans.gameObject);
        }
        ObjectList.Clear();
        Grid = null;

        // set the dependencies
        Utility.Instance.Height = Height;
        Utility.Instance.Width = Width;
        Utility.Instance.TileHeight = TileHeight;
        Utility.Instance.TileWidth = TileWidth;
        Utility.Instance.MapPosition = transform.position;

        // set up new Tiles and their data
        Grid = new Tile[Width, Height];
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                Vector3 tilePos = Utility.Instance.CoordToVec3(x, y);
                Transform obj = Instantiate(TilePrefab, tilePos, Quaternion.identity, transform);
                obj.localScale = new Vector3(TileWidth, TileHeight, 0.5f);
                Tile newTile = new Tile(x, y, tilePos, obj);
                ObjectList.Add(obj);
                TileList.Add(newTile);
                Grid[x, y] = newTile;
            }

        foreach(Vector2 wall in WallList)
        {
            int x = (int)wall.x;
            int y = (int)wall.y;
            Grid[x, y].Wall = true;
            Grid[x, y].Prefab.GetComponent<MeshRenderer>().material.color = Color.black;

        }
        WallList = walls;

        AStar.Instance.grid = Grid;
    }

    public void SaveMap()
    {
        GameState.current.SaveMapData((int)Width, (int)Height, TileWidth, TileHeight, WallList);
    }

    void Awake () {
        CC_Grid.Instance.begin = true;
        Utility.Instance.Height = Height;
        Utility.Instance.Width = Width;
        Utility.Instance.TileHeight = TileHeight;
        Utility.Instance.TileWidth = TileWidth;
        Utility.Instance.MapPosition = transform.position;

        
        Grid = new Tile[Width, Height];
        ObjectList = new List<Transform>();
        WallList = new List<Vector2>();

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
        GameState.current = new GameState((int)Width, (int)Height, TileWidth, TileHeight);
    }

    void Start()
    {
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

    void Update()
    {
        ResetMapColor();
    }

    void OnGUI ()
    {
        if (DebugManager.Instance.DB_PlaceWalls)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float t = -1f * (Vector3.Dot(ray.origin, normal) + d)
                                / (Vector3.Dot(ray.direction, normal));

                Vector3 projectedPoint = ray.origin + t * ray.direction;


                projectedPoint.x = Mathf.Clamp(projectedPoint.x, minV3.x, maxV3.x);
                projectedPoint.y = Mathf.Clamp(projectedPoint.y, minV3.y, maxV3.y);

                projectedPoint = Utility.Instance.Vec3ToCoord(projectedPoint);
                Grid[(int)projectedPoint.x, (int)projectedPoint.y].Wall = true;
                WallList.Add(projectedPoint);
                Grid[(int)projectedPoint.x, (int)projectedPoint.y].Prefab.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }
}
