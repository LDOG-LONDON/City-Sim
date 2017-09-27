using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DB_Text
{
    NONE,
    Density,
    AverageVelocity,
    Height,
    Potential,
    MAX
};

public enum DB_Color
{
    NONE,
    Density,
    AverageVelocity,
    Height,
    Potential,
    MAX
};

public enum DB_AStar
{
    NONE,
    Color,
    WaypointLine,
    MAX
};

public class DebugManager : Singleton<DebugManager> {

    protected DebugManager() {}

    
    public DB_Text DebugText;
    public DB_Color DebugColor;
    public DB_AStar DebugAStar;

    public bool DB_AgentTrail;
    public bool DB_ColorAgentTile;

    public bool DB_Editor;
    public bool DB_PlaceWalls;
    public bool DB_PlaceAgents;

    public bool DB_RunSimulation;
}
