using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : Singleton<AStar> {

    protected AStar() {}

    private float timer;
    int Height;
    int Width;
    int searchIteration = -1;
    private Tile goal;
    private Tile origin;
    public bool useDiagonals = true;
    public bool singleStep;
    bool pathEvent = false;
    List<GameObject> agents;

    List<Tile> open;
    List<Tile> finalPath;
    List<Vector3> finalPathVec3;
    public Tile[,] grid;


    public void Start()
    {
        Width = (int)Utility.Instance.Width;
        Height = (int)Utility.Instance.Height;

        open = new List<Tile>();
        finalPath = new List<Tile>();
        finalPathVec3 = new List<Vector3>();
        agents = new List<GameObject>();
    }

    void Update()
    {
        if (pathEvent)
        {
            timer = 0;
            while(timer < 0.1f)
            {
                timer += Time.fixedDeltaTime;
                bool done = FindPath();
                if (done)
                {
                    GivePath();
                    pathEvent = false;
                    return;
                }
            }
        }
    }

    void GivePath()
    {
        foreach(GameObject unit in agents)
        {
            unit.SendMessage("SetWaypointList",
                finalPathVec3,
                SendMessageOptions.DontRequireReceiver);
        }
        
        List<Vector3> buff = finalPathVec3;
    }

    public void NewRequest(Vector3 Start, Vector3 Goal, GameObject Agent)
    {
        pathEvent = true;
        
        open.Clear();
        finalPath.Clear();
        finalPathVec3.Clear();
        agents.Clear();

        agents.Add(Agent);
        Start = Utility.Instance.Vec3ToCoord(Start);
        Goal = Utility.Instance.Vec3ToCoord(Goal);

        Goal.x = Mathf.Clamp(Goal.x, 0, Width - 1);
        Start.x = Mathf.Clamp(Start.x, 0, Width - 1);
        Goal.y = Mathf.Clamp(Goal.y, 0, Height - 1);
        Start.y = Mathf.Clamp(Start.y, 0, Height - 1);

        goal = grid[(int)Goal.x,(int)Goal.y];
        origin = grid[(int)Start.x,(int)Start.y];

        goal.Parent = null;
        origin.Parent = null;

        origin.Cost = 0f;
        origin.Given = 0f;

        open.Add(origin);

        searchIteration++;

        //GameObject.Find("Map").GetComponent<Map>().SendMessage("ResetMapColor", SendMessageOptions.DontRequireReceiver);
    }

    public void NewRequest(Vector3 Goal, List<GameObject> Agents)
    {
        pathEvent = true;

        open.Clear();
        finalPath.Clear();
        finalPathVec3.Clear();

        if (agents != Agents)
        {
            agents.Clear();
            agents = Agents;
        }
        
        Vector3 Start = new Vector3();

        foreach(GameObject unit in Agents)
        {
            Start += unit.transform.position;
        }
        Start /= Agents.Count;
        Start = Utility.Instance.Vec3ToCoord(Start);

        Goal.x = Mathf.Clamp(Goal.x, 0, Width - 1);
        Start.x = Mathf.Clamp(Start.x, 0, Width - 1);
        Goal.y = Mathf.Clamp(Goal.y, 0, Height - 1);
        Start.y = Mathf.Clamp(Start.y, 0, Height - 1);

        goal = grid[(int)Goal.x, (int)Goal.y];
        origin = grid[(int)Start.x, (int)Start.y];

        goal.Parent = null;
        origin.Parent = null;

        origin.Cost = 0f;
        origin.Given = 0f;

        open.Add(origin);

        searchIteration++;
    }

    public bool FindPath()
    {
        Tile cur;
        while (open.Count > 0)
        {
            cur = GetBestOnOpenList();

            if (cur == null)
                return true; // no path

            if (cur.X == goal.X && cur.Y == goal.Y)
            {
                ChangeColor(cur, Color.red);

                ConstructPath(cur);
                return true;// found path
            }

            List<Tile> neighbors = GetNeighbors(cur);
            for (int i = 0; i < neighbors.Count; i++)
            {
                Tile n = neighbors[i];


                if (n.searchIteration != searchIteration || n.type == Tile.searchType.NONE)
                {
                    float neighborWeight = 1f;
                    if (cur.X != n.X && cur.Y != n.Y)
                        neighborWeight = 1.414f;

                    float newGiven = cur.Given + neighborWeight;
                    float newCost = newGiven + Heuristic(n, goal);

                    n.type = Tile.searchType.OPEN;
                    n.searchIteration = searchIteration;
                    n.Given = newGiven;
                    n.Cost = newCost;
                    n.Parent = cur;
                    open.Add(n);

                    ChangeColor(n, Color.yellow);
                }
                else if (n.type != Tile.searchType.NONE)
                {
                    float neighborWeight = 1f;
                    if (cur.X != n.X && cur.Y != n.Y)
                        neighborWeight = 1.414f;
                    float newGiven = cur.Given + neighborWeight;

                    if (newGiven < n.Given)
                    {
                        float newCost = newGiven + Heuristic(n, goal);
                        n.Cost = newCost;
                        n.Given = newGiven;
                        n.Parent = cur;

                        if (n.type == Tile.searchType.CLOSED)
                        {
                            n.type = Tile.searchType.OPEN;
                            open.Add(n);
                            ChangeColor(n, Color.yellow);
                        }
                    }
                }
            }
            cur.type = Tile.searchType.CLOSED;
            ChangeColor(cur, Color.blue);

            if (singleStep)
                return false;
        }
        return true;
    }

    public void PrintFinalPath()
    {
        if (DebugManager.Instance.DebugAStar != DB_AStar.Color)
            return;

        for (int i = 0; i < finalPath.Count; i++)
        {
            ChangeColor(finalPath[i], Color.green);
        }
    }

    public void ChangeColor(Tile tile, Color color)
    {
        tile.Prefab.GetComponent<MeshRenderer>().material.color = color;
    }

    public bool isOpen(Tile start, Tile end)
    {
        int minX = Mathf.Min(start.X, end.X);
        int maxX = Mathf.Max(start.X, end.X);
        int minY = Mathf.Min(start.Y, end.Y);
        int maxY = Mathf.Max(start.Y, end.Y);

        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                if (grid[x, y].Wall)
                    return false;
            }
        }
        return true;
    }

    public void ConstructPath(Tile tile)
    {
        finalPath.Add(tile);
        finalPathVec3.Add(Utility.Instance.CoordToVec3(tile.X, tile.Y));
        Tile walker = tile.Parent;
        while (walker != origin)
        {
            finalPath.Insert(0, walker);
            finalPathVec3.Insert(0,Utility.Instance.CoordToVec3(walker.X, walker.Y));
            walker = walker.Parent;
        }

        finalPathVec3.Insert(0, Utility.Instance.CoordToVec3(origin.X, origin.Y));
        finalPath.Insert(0, origin);
    }

    public Tile GetBestOnOpenList()
    {
        int best = -1;
        float bestCost = Mathf.Infinity;

        for (int i = 0; i < open.Count; i++)
        {
            if (open[i].Cost < bestCost)
            {
                bestCost = open[i].Cost;
                best = i;
            }
        }

        if (best == -1)
            return null;
        Tile bestTile = open[best];
        open.RemoveAt(best);
        return bestTile;
    }

    public float Heuristic(Tile start, Tile end)
    {
        int XDiff = Mathf.Abs(start.X - end.X);
        int YDiff = Mathf.Abs(start.Y - end.Y);
        int min = Mathf.Min(XDiff, YDiff);

        return min * 1.414231f + Mathf.Max(XDiff, YDiff) - min;
    }

    public List<Tile> GetNeighbors(Tile tile)
    {
        Tile S = GetS(tile.X, tile.Y);
        Tile N = GetN(tile.X, tile.Y);
        Tile W = GetW(tile.X, tile.Y);
        Tile E = GetE(tile.X, tile.Y);

        List<Tile> tileList = new List<Tile>();

        if (S != null && !S.Wall)
        {
            //S.neighborGiven = 1f;
            tileList.Add(S);
        }
        if (W != null && !W.Wall)
        {
            //W.neighborGiven = 1f;
            tileList.Add(W);
        }
        if (E != null && !E.Wall)
        {
            // E.neighborGiven = 1f;
            tileList.Add(E);
        }
        if (N != null && !N.Wall)
        {
            //N.neighborGiven = 1f;
            tileList.Add(N);
        }

        if (!useDiagonals)
            return tileList;

        Tile NW = GetNW(tile.X, tile.Y);
        Tile NE = GetNE(tile.X, tile.Y);
        Tile SW = GetSW(tile.X, tile.Y);
        Tile SE = GetSE(tile.X, tile.Y);

        if (NW != null && !NW.Wall)
        {
            if ((N != null && !N.Wall) && (W != null && !W.Wall))
            {
                // NW.neighborGiven = 1.414231f;
                tileList.Add(NW);
            }
        }
        if (NE != null && !NE.Wall)
        {
            if ((N != null && !N.Wall) && (E != null && !E.Wall))
            {
                // NE.neighborGiven = 1.414231f;
                tileList.Add(NE);
            }

        }
        if (SW != null && !SW.Wall)
        {
            if ((S != null && !S.Wall) && (W != null && !W.Wall))
            {
                //SW.neighborGiven = 1.414231f;
                tileList.Add(SW);
            }
        }
        if (SE != null && !SE.Wall)
        {
            if ((S != null && !S.Wall) && (E != null && !E.Wall))
            {
                // SE.neighborGiven = 1.414231f;
                tileList.Add(SE);
            }
        }

        return tileList;
    }

    public Tile GetS(int _x, int _y)
    {
        int y = _y - 1;

        if (y < 0)
            return null;
        return grid[_x, y];
    }

    public Tile GetSW(int _x, int _y)
    {
        int y = _y - 1;
        int x = _x - 1;
        if (y < 0 || x < 0)
            return null;

        return grid[x, y];
    }

    public Tile GetSE(int _x, int _y)
    {
        int y = _y - 1;
        int x = _x + 1;
        if (y < 0 || x >= Width-1)
            return null;
        return grid[x, y];
    }

    public Tile GetW(int _x, int _y)
    {
        int x = _x - 1;
        if (x < 0)
            return null;
        return grid[x, _y];
    }

    public Tile GetE(int _x, int _y)
    {
        int x = _x + 1;
        if (x >= Width-1)
            return null;
        return grid[x, _y];
    }

    public Tile GetNE(int _x, int _y)
    {
        int y = _y + 1;
        int x = _x + 1;
        if (y >= Height-1 || x >= Width-1)
            return null;
        return grid[x, y];
    }

    public Tile GetNW(int x, int y)
    {
        int Y = y + 1;
        int X = x - 1;
        if (y >= Height-1 || X < 0)
            return null;
        return grid[X, Y];
    }

    public Tile GetN(int x, int y)
    {
        int Y = y + 1;
        if (Y >= Height-1)
            return null;
        return grid[x, Y];
    }
}
