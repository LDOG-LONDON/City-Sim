using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : Singleton<DebugManager> {

    public enum DB_Text
    {
        NONE,
        Density,
        AverageVelocity,
        Height,
        Potential,
        MAX
    };
    public DB_Text DebugText;
    //public bool DB_TextDensity;
    //public bool DB_TextAvgVelocity;

    public enum DB_Color
    {
        NONE,
        Density,
        AverageVelocity,
        Height,
        Potential,
        MAX
    };
    public DB_Color DebugColor;
    ///public bool DB_ColorDensity;
    ///public bool DB_ColorAvgVelocity;

    public enum DB_AStar
    {
        NONE,
        Color,
        WaypointLine,
        MAX
    }
    public DB_AStar DebugAStar;
    //public bool DB_ColorAStar;
    //public bool DB_LineAStar;

    public bool DB_AgentTrail;
    public bool DB_ColorAgentTile;

    public bool DB_Editor;
    public bool DB_PlaceWalls;
    public bool DB_PlaceAgents;

    public bool DB_RunSimulation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
