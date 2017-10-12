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
    PotentialGradients,
    SpeedFields,
    CostFields,
    DeltaHeights,
    MAX
};

public enum DB_Line
{
    NONE,
    Astar,
    AgentGoal,
    AverageVelocity,
    CellVelocity,
    MAX
};

public enum DB_Color
{
    NONE,
    Density,
    AverageVelocity,
    Height,
    Speed,
    UnitCost,
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

    public DB_Line DebugLine = DB_Line.NONE;
    public DB_Text DebugText = DB_Text.NONE;
    public DB_Color DebugColor = DB_Color.Density;
    public DB_AStar DebugAStar;
    public bool CellFaceVelocities = false;

    public int DB_CCGroupNumber = 0;

    public bool DB_AgentTrail;
    public bool DB_ColorAgentTile;

    public bool DB_Editor;
    public bool DB_PlaceWalls;
    public bool DB_PlaceAgents;

    public bool DB_On = true;

    public bool DB_RunSimulation;
}
