using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFaceInfo
{
    public float Speed_; // here to there
    public float UnitCost_; //here to there
    public float _Speed; // there to here
    public float _UnitCost; //there to here

    float DeltaHeight; // height difference
    float GradientPotential; // difference in potential field
    Vector2 Velocity; 
}

public class CC_GridCell {

    CellFaceInfo East;
    CellFaceInfo West;
    CellFaceInfo South;
    CellFaceInfo North;

    
    float Potential;
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
