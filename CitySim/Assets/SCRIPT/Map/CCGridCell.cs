using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFaceInfo
{
    public float Speed_HT; // here to there
    public float UnitCost_HT; //here to there
    public float TH_Speed; // there to here
    public float TH_UnitCost; //there to here

    public float DeltaHeight; // height difference
    public float GradientPotential; // difference in potential field
    public Vector2 Velocity; 
}

public class CC_GridCell {
    
    public CC_GridCell(Vector3 worldPos, Vector2 Coordinate)
    {
        WorldPos = worldPos;
        Coord = Coordinate;
    }

    public Vector3 WorldPos;
    public Vector2 Coord;

    public CellFaceInfo East;
    public CellFaceInfo West;
    public CellFaceInfo South;
    public CellFaceInfo North;

    public float Potential;
}

public class CC_GlobalCell
{

    public CC_GlobalCell(Vector3 worldPos, Vector2 Coordinate)
    {
        WorldPos = worldPos;
        Coord = Coordinate;
    }
    public Vector3 WorldPos;
    public Vector2 Coord;

    public Vector2 AverageVelocity;
    public float Discomfort;
    public float Density;
    public float Height;
}
