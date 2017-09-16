using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_Grid : Singleton<CC_Grid> {

    protected CC_Grid() {}

    int Width;
    int Height;
    public bool begin = false;
    private bool debugOn = true;
    public bool DensityText = true;

    private GameObject holder;

    public float AverageVelocity;
    public float Lambda = 1f;

    private float avgVel;

    List<CC_GlobalCell> GlobalCellList;
    CC_GlobalCell[,] GlobalGrid;
    List<Transform> AgentList;
    List<CC_AgentData> AgentData;

    List<Transform> Db_TileList;
    List<TextMesh> Db_TextList;
    List<MeshRenderer> Db_MeshList;

    TextMesh[,] Db_TextGrid;
    MeshRenderer[,] Db_MeshGrid;


    void InitDebug()
    {
        Db_TileList = new List<Transform>();
        Db_TextList = new List<TextMesh>();
        Db_MeshList = new List<MeshRenderer>();

        Db_TextGrid = new TextMesh[Width, Height];
        Db_MeshGrid = new MeshRenderer[Width, Height];

        holder = new GameObject("DebugHolder");
    }

    void SetUpDebug(int x, int y, Vector3 WorldPos)
    {
        WorldPos.z += 0.25f;
        GameObject dbTile = (GameObject)Instantiate(
            Resources.Load("DebugTile"),
            WorldPos,
            Quaternion.identity);

        dbTile.transform.localScale = 
            new Vector3(Utility.Instance.TileWidth,
            Utility.Instance.TileHeight,
            0.13f);

        //transform
        Db_TileList.Add(dbTile.transform);

        // debug text
        TextMesh text = dbTile.transform.GetChild(0).GetComponent<TextMesh>();
        text.text = new Vector2((float)x, (float)y).ToString();
        Db_TextList.Add(text);
        Db_TextGrid[x, y] = text;

        // debug mesh (for coloring)
        MeshRenderer mesh = dbTile.GetComponent<MeshRenderer>();
        Db_MeshList.Add(mesh);
        Db_MeshGrid[x, y] = mesh;

        dbTile.transform.parent = holder.transform;

    }

    void Debug()
    {
        if (Utility.Instance.DebugInfo == false && debugOn == true)
        {
            foreach(MeshRenderer obj in Db_MeshList)
            {
                obj.enabled = false;
            }
            foreach(TextMesh obj in Db_TextList)
            {
                obj.transform.GetComponent<MeshRenderer>().enabled = false;
            }
            debugOn = false;
        }
        
        if (Utility.Instance.DebugInfo == true && debugOn == false)
        {
            foreach (MeshRenderer obj in Db_MeshList)
            {
                obj.enabled = true;
            }
            foreach (TextMesh obj in Db_TextList)
            {
                obj.transform.GetComponent<MeshRenderer>().enabled = true;
            }
            debugOn = true;
        }

        if (Utility.Instance.DebugInfo == true)
        {
            if (DensityText == true)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Db_TextGrid[x, y].text = new string(("Den: " + GlobalGrid[x, y].Density).ToCharArray());
                    }
                }
            }
        }
    }

    void Start()
    {
        Width = (int)Utility.Instance.Width;
        Height = (int)Utility.Instance.Height;
        GlobalGrid = new CC_GlobalCell[Width, Height];
        GlobalCellList = new List<CC_GlobalCell>();

        InitDebug();

        for(int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector2 coord = new Vector2((float)x, (float)y);
                Vector3 WorldPos = Utility.Instance.CoordToVec3(x, y);
                CC_GlobalCell newGC = new CC_GlobalCell(WorldPos,coord);
                GlobalGrid[x, y] = newGC;
                GlobalCellList.Add(newGC);
                SetUpDebug(x, y, WorldPos);
            }
        }

        List<GameObject> units = new List<GameObject>(
            GameObject.FindGameObjectsWithTag("SelectableUnit"));
        AgentList = new List<Transform>();
        AgentData = new List<CC_AgentData>();
        foreach(GameObject obj in units)
        {
            AgentData.Add(obj.GetComponent<CC_AgentData>());
            AgentList.Add(obj.transform);
        }

    }

    void DensityConversion()
    {
        foreach(CC_AgentData agent in AgentData)
        {
            // not sure if its talking about cell coord or world coords

            // Grid Setup
            //  D | C
            // -------
            //  A | B

            Vector3 agentWorld = agent.transform.position;
            Vector3 coord = Utility.Instance.Vec3ToCoord(agentWorld);
            int Cx = (int)coord.x;
            int Cy = (int)coord.y;

            int Dx = Cx;
            int Dy = Cy;
            int Bx = Cx;
            int By = Cy;
            int Ax = Cx;
            int Ay = Cy;
            bool dontUseDA = false;
            bool dontUseBA = false;

            // check if 0 on x axis
            if (Cx <= 0)
            {
                Dx = 0;
                Ax = 0;
                dontUseDA = true;
            }
            else
            {
                Dx--;
                Ax--;
            }
            //check if 0 on y axis
            if (Cy <= 0)
            {
                By = 0;
                Ay = 0;
                dontUseBA = true;
            }
            else
            {
                By--;
                Ay--;
            }

            float invX;
            float invY;

            float densityC;
            float densityA;
            float densityB;
            float densityD;

            Vector3 cWorld = GlobalGrid[Cx, Cy].WorldPos;
            Vector3 dWorld = GlobalGrid[Dx, Dy].WorldPos;
            Vector3 bWorld = GlobalGrid[Bx, By].WorldPos;
            Vector3 aWorld = GlobalGrid[Ax, Ay].WorldPos;

            Vector3 minCellWorld = GlobalGrid[Ax, Ax].WorldPos;
            float deltaX = agentWorld.x - minCellWorld.x;
            float deltaY = agentWorld.y - minCellWorld.y;


            // coord [0,0] on grid
            if (dontUseBA && dontUseDA)
            {
                // case if we only use C cell
                //deltaX = cWorld.x - agentWorld.x;
                //deltaY = cWorld.y - agentWorld.y;

                // case if we use coord [-1,-1] for "min" cell
                Vector3 negMinCell = Utility.Instance.CoordToVec3(-1, -1);
                deltaX = agentWorld.x - negMinCell.x;
                deltaY = agentWorld.y - negMinCell.y;

                densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                GlobalGrid[Cx, Cy].Density += densityC;
                continue;
            }

            // case #1: agent is below (in both x & y) it current cell's center
            if (agentWorld.x < cWorld.x && agentWorld.y < cWorld.y)
            {
                // coord [0,y] on grid
                if (dontUseDA)
                {
                    // case if we use coord [-1,y-1] for "min" cell
                    Vector3 negMinCell = Utility.Instance.CoordToVec3(-1, By);
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                    densityB = Mathf.Pow(Mathf.Min(deltaX,deltaY),Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC;
                    GlobalGrid[Bx, By].Density += densityB;
                    continue;
                }
                // coord [x,0] on grid
                else if (dontUseBA)
                {
                    Vector3 negMinCell = Utility.Instance.CoordToVec3(Dx, -1);
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda); 
                    densityD = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC;
                    GlobalGrid[Dx, Dy].Density += densityD;
                    continue;
                }
                // coord [x,y] on grid (from the example)
                else
                {
                    Vector3 negMinCell = aWorld;
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda); 
                    densityB = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda); // fix
                    densityA = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda); // fix
                    densityD = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC;
                    GlobalGrid[Bx, By].Density += densityB;
                    GlobalGrid[Dx, Dy].Density += densityD;
                    GlobalGrid[Ax, Ay].Density += densityA;
                    continue;
                }
            }

            // case #2: agent is above cell center in x-axis but lower on the y
            else if (agentWorld.x > cWorld.x && agentWorld.y < cWorld.y)
            {
                // coord [x, 0] on gid
                if (dontUseBA)
                {
                    Vector3 negMinCell = Utility.Instance.CoordToVec3(Cx, -1);
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);

                    GlobalGrid[Cx, Cy].Density += densityC;
                    continue;
                }
                // coord [x,y] on grid
                else
                {
                    Vector3 negMinCell = bWorld;
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                    densityB = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC; 
                    GlobalGrid[Bx, By].Density += densityB; // fix
                    continue;
                }
            }

            // case #3: both x & y of agent are above its current cells center
            else if (agentWorld.x > cWorld.x && agentWorld.y > cWorld.y)
            {
                Vector3 negMinCell = cWorld;
                deltaX = agentWorld.x - negMinCell.x;
                deltaY = agentWorld.y - negMinCell.y;
                densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);

                GlobalGrid[Cx, Cy].Density += densityC;
                continue;
            }

            // case #4: agents x is less than its currents cell and its y is greater
            else
            {
                // coord [0, y] on grid
                if (dontUseDA)
                {
                    Vector3 negMinCell = Utility.Instance.CoordToVec3(-1, Cx);
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);

                    GlobalGrid[Cx, Cy].Density += densityC;
                    continue;
                }
                // coord [x,y] on grid
                else
                {
                    Vector3 negMinCell = dWorld;
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                    densityD = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC;
                    GlobalGrid[Dx, Dy].Density += densityD;
                    continue;
                }
            }
        }
    }

    void Update()
    {
        Debug();
        DensityConversion();
    }
}
