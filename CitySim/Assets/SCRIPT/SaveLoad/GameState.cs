using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState {

    public static GameState current;

    public string Name;

    // maps data
    public int MapWidth;
    public int MapHeight;
    public float TileWidth;
    public float TileHeight;

    [SerializeField]
    public List<Vector2> WallCoords;

    // agent data
    public int AgentCount;
    //public List<GameObject> Agents;
    //public List<Vector3> AgentPos;
    //public List<Vector3> AgentGoals;

    // flocking data
    public float CohesionRadius;
    public float AlignmentRadius;
    public float SeperationRadius;
    public float CohesionInfuence;
    public float AlignmentInfluence;
    public float SeperationInfuence;
    public float WanderingInfuence;
    public int SeperationCase;
    public float Speed;

    public GameState(int mapWidth, int mapHeight, float tileWidth, float tileHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Name = new string("New".ToCharArray());
    }

    public void SaveMapData(int mapWidth, int mapHeight, float tileWidth, float tileHeight, List<Vector2> Walls)
    {
        current.MapWidth = mapWidth;
        current.MapHeight = mapHeight;
        current.TileWidth = tileWidth;
        current.TileHeight = tileHeight;
        current.WallCoords = Walls;

        SaveAgentData();
        SaveAgentFlockingData();
    }
    
    public void SaveAgentData()
    {
        current.AgentCount = AgentManager.Instance.agentCount;

        List<GameObject> AgentObj = new List<GameObject>();
        //List<Vector3> agentsPos = new List<Vector3>();
        //List<Vector3> agentsGoals = new List<Vector3>();
        foreach(Transform trans in AgentManager.Instance.agentTransforms)
        {
            AgentObj.Add(trans.gameObject);
            //agentsPos.Add(trans.position);
            //agentsGoals.Add(trans.GetComponent<SimpleFlocking>().Goal);
        }

       // current.Agents = AgentObj;
        //current.AgentPos = agentsPos;
        //current.AgentGoals = agentsGoals;
    }   

    public void SaveAgentFlockingData()
    {
        current.CohesionRadius = GlobalFlockingData.Instance.CohesionRadius;
        current.AlignmentRadius = GlobalFlockingData.Instance.AlignmentRadius;
        current.SeperationRadius = GlobalFlockingData.Instance.SeperationRadius;
        current.CohesionInfuence = GlobalFlockingData.Instance.CohesionInfuence;
        current.AlignmentInfluence = GlobalFlockingData.Instance.AlignmentInfluence;
        current.SeperationInfuence = GlobalFlockingData.Instance.SeperationInfuence;
        current.WanderingInfuence = GlobalFlockingData.Instance.WanderingInfuence;
        current.SeperationCase = GlobalFlockingData.Instance.SeperationCase;
        current.Speed = GlobalFlockingData.Instance.Speed;
    }

    public void LoadVariables()
    {
        GameObject map = GameObject.Find("Map");
        if (!map)
        {
            Debug.Log("No Map Game Object Found!");
            return;
        }

        Map Map = map.GetComponent<Map>();
        Map.LoadMap(current.MapWidth,
            current.MapHeight,
            current.TileWidth,
            current.TileHeight,
            current.WallCoords);


        // load flocking data
        GlobalFlockingData.Instance.CohesionRadius = current.CohesionRadius;
        GlobalFlockingData.Instance.AlignmentRadius = current.AlignmentRadius;
        GlobalFlockingData.Instance.SeperationRadius = current.SeperationRadius;
        GlobalFlockingData.Instance.CohesionInfuence = current.CohesionInfuence;
        GlobalFlockingData.Instance.AlignmentInfluence = current.AlignmentInfluence;
        GlobalFlockingData.Instance.SeperationInfuence = current.SeperationInfuence;
        GlobalFlockingData.Instance.WanderingInfuence = current.WanderingInfuence;
        GlobalFlockingData.Instance.SeperationCase = current.SeperationCase;
        GlobalFlockingData.Instance.Speed = current.Speed;

        // load agent stuff
       // AgentManager.Instance.Load(current.Agents/*, current.AgentPos, current.AgentGoals*/);
    }
}
