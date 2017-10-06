using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CC_Grid : Singleton<CC_Grid> {

    protected CC_Grid() {}
    // map data
    int Width;
    int Height;

    // debug
    public bool begin = false;
    private bool debugOn = true;
    public bool DensityText = false;


    private GameObject holder;

    // weights for unit costs
    public float DistanceWeight = 1f;
    public float TimeWeight = 1f;
    public float DiscomfortWeight = 1f;

    // for density
    public float Lambda = 0.5f;
    public float AgentRadius = 1f;

    // speed min and max
    public float Fmin = 0f;
    public float Fmax = 10f;
    // slope min and max
    public float SlopeMin = -0.5f;
    public float SlopeMax = 0.5f;
    // density min and max
    public float DensityMin = 0f;
    public float DensityMax = 8.5f;

    // for groups
    public int GroupCount = 1;
    public int currentGroupIndex = 0;
    List<CC_GroupGrid> GroupGridList;



    // data
    List<CC_GlobalCell> GlobalCellList;
    CC_GlobalCell[,] GlobalGrid;
    List<Transform> AgentList;
    List<CC_AgentData> AgentData;

    // debug data
    List<Transform> Db_TileList;
    List<TextMesh> Db_TextList;
    List<MeshRenderer> Db_MeshList;

    List<TextMesh>[,] Db_TextGrid;
    MeshRenderer[,] Db_MeshGrid;
    LineRenderer[,] Db_LineGrid;

    bool SingleStep = true;

    public void Load()
    {
        Width = (int)Utility.Instance.Width;
        Height = (int)Utility.Instance.Height;
        GlobalGrid = new CC_GlobalCell[Width, Height];
        GlobalCellList = new List<CC_GlobalCell>();

        InitDebug();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector2 coord = new Vector2((float)x, (float)y);
                Vector3 WorldPos = Utility.Instance.CoordToVec3(x, y);
                CC_GlobalCell newGC = new CC_GlobalCell(WorldPos, coord);
                GlobalGrid[x, y] = newGC;
                GlobalCellList.Add(newGC);
                SetUpDebug(x, y, WorldPos);
            }
        }


        AgentList = AgentManager.Instance.agentTransforms;
        AgentData = new List<CC_AgentData>();
        foreach (Transform obj in AgentList)
        {
            AgentData.Add(obj.GetComponent<CC_AgentData>());
        }
    }

    void Start()
    {
        // get map width/height
        Width = (int)Utility.Instance.Width;
        Height = (int)Utility.Instance.Height;

        // initialize grid and cell list
        GlobalGrid = new CC_GlobalCell[Width, Height];
        GlobalCellList = new List<CC_GlobalCell>();

        // set density min
        DensityMin = 1f / Mathf.Pow(2f, Lambda);

        InitDebug();

        // put data in global grid
        for(int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector2 coord = new Vector2((float)x, (float)y);
                Vector3 WorldPos = Utility.Instance.CoordToVec3(x, y);
                CC_GlobalCell newGC = new CC_GlobalCell(WorldPos,coord);
                GlobalGrid[x, y] = newGC;
                GlobalGrid[x, y].Height = WorldPos.z;
                GlobalCellList.Add(newGC);
                SetUpDebug(x, y, WorldPos);
            }
        }

        // get agent data that pertains to contuum crowd data
        AgentList = AgentManager.Instance.agentTransforms;
        AgentData = new List<CC_AgentData>();
        foreach(Transform obj in AgentList)
        {
            AgentData.Add(obj.GetComponent<CC_AgentData>());
        }

        // set up group grid(s)
        GroupGridList = new List<CC_GroupGrid>();
        int agentPerGroup = AgentList.Count / GroupCount;
        for (int i = 0; i < GroupCount; i++)
        {
            CC_GroupGrid groupGrid = new CC_GroupGrid();
            groupGrid.Goals = new List<Vector2>();
            groupGrid.heap = new List<CC_GridCell>();
            groupGrid.Grid = new CC_GridCell[Width, Height];
            groupGrid.GroupNumber = i;
            GroupGridList.Add(groupGrid);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Vector2 coord = GlobalGrid[x, y].Coord;
                    Vector3 worldPos = GlobalGrid[x, y].WorldPos;
                    GroupGridList[i].Grid[x, y] = new CC_GridCell(worldPos, coord);
                    GroupGridList[i].Grid[x, y].East = new CellFaceInfo();
                    GroupGridList[i].Grid[x, y].West = new CellFaceInfo();
                    GroupGridList[i].Grid[x, y].North = new CellFaceInfo();
                    GroupGridList[i].Grid[x, y].South = new CellFaceInfo();
                }
            }
        }

        // set up group height stuff
        for (int i = 0; i < GroupCount; i++)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    GroupGridList[i].Grid[x, y].East.DeltaHeight =
                        HeightDifference(x, y, Vector2.right);

                    GroupGridList[i].Grid[x, y].West.DeltaHeight =
                        HeightDifference(x, y, Vector2.left);

                    GroupGridList[i].Grid[x, y].North.DeltaHeight =
                        HeightDifference(x, y, Vector2.up);

                    GroupGridList[i].Grid[x, y].South.DeltaHeight =
                        HeightDifference(x, y, Vector2.down);
                }
            }
        }
    }

    void ResetGrid()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                GlobalGrid[x, y].AverageVelocity = Vector2.zero;
                GlobalGrid[x,y].Density = 0f;
            }
        }
    }

    void Update()
    {
        ResetGrid();
        DensityConversion();
        for (int i = 0; i < GroupGridList.Count; i++)
        {
            UnitCost(i);
            Potential(i);
        }
        Debug();
    }

    float HeightDifference(int x, int y, Vector2 direction)
    {
        Vector2 unitVector = direction.normalized;
        // angle in degrees
        float angle = Mathf.Atan2(unitVector.y, unitVector.x) * 180f / Mathf.PI;

        // east
        if (angle <= 45f && angle >= -45f)
        {
            if (x <= 0)
                return 0f;
            return GlobalGrid[x - 1, y].Height - GlobalGrid[x, y].Height;
        }
        // north
        else if (angle > 45f && angle <= 135f)
        {
            if (y >= Height - 1)
                return 0f;
            return GlobalGrid[x, y + 1].Height - GlobalGrid[x, y].Height;
        }
        // west
        else if (angle > 135f && angle <= -135f)
        {
            if (x >= Width - 1)
                return 0f;
            return GlobalGrid[x + 1, y].Height - GlobalGrid[x, y].Height;
        }
        // south (angle > -135 and angle < -45)
        else
        {
            if (y <= 0)
                return 0f;
            return GlobalGrid[x, y - 1].Height - GlobalGrid[x, y].Height;
        }
        return 0f;
    }

    Vector2 GetTileInDirection(int x, int y, Vector2 direction)
    {
        Vector2 unitVector = direction.normalized;

        // angle in degrees
        float angle = Mathf.Atan2(unitVector.y, unitVector.x) * 180f / Mathf.PI;

        // east
        if (angle <= 45f && angle >= -45f)
        {
            if (x <= 0)
                return new Vector2(-1, -1);
            return new Vector2(x - 1, y);
        }
        // north
        else if (angle > 45f && angle <= 135f)
        {
            if (y >= Height - 1)
                return new Vector2(-1, -1);
            return new Vector2(x, y + 1);
        }
        // west
        else if (angle > 135f && angle <= -135f)
        {
            if (x >= Width - 1)
                return new Vector2(-1, -1);
            return new Vector2(x + 1, y);
        }
        // south (angle > -135 and angle < -45)
        else
        {
            if (y <= 0)
                return new Vector2(-1, -1);
            return new Vector2(x, y - 1);
        }
    }

    Vector2 GetTileInDirection(int x, int y, float direction)
    {
        // angle in degrees
        float angle = direction;

        // east
        if (angle <= 45f && angle >= -45f)
        {
            if (x <= 0)
                return new Vector2(-1, -1);
            return new Vector2(x - 1, y);
        }
        // north
        else if (angle > 45f && angle <= 135f)
        {
            if (y >= Height - 1)
                return new Vector2(-1, -1);
            return new Vector2(x, y + 1);
        }
        // west
        else if (angle > 135f && angle <= -135f)
        {
            if (x >= Width - 1)
                return new Vector2(-1, -1);
            return new Vector2(x + 1, y);
        }
        // south (angle > -135 and angle < -45)
        else
        {
            if (y <= 0)
                return new Vector2(-1, -1);
            return new Vector2(x, y - 1);
        }
    }
}
