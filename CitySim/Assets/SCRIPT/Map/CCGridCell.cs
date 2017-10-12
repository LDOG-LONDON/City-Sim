using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFaceInfo
{
    public float Speed; // here to there
    public float UnitCost; //here to there

    public float DeltaHeight; // height difference
    public float PotentialGradient; // difference in potential field
    public float Velocity; 
}

public enum PotentialType
{
    Known,
    Candidate,
    Unknown,
};

public class CC_GridCell {
    
    public CC_GridCell(Vector3 worldPos, Vector2 Coordinate)
    {
        WorldPos = worldPos;
        Coord = Coordinate;
    }

    // used data
    public Vector3 WorldPos;
    public Vector2 Coord;

    public CellFaceInfo East;
    public CellFaceInfo West;
    public CellFaceInfo South;
    public CellFaceInfo North;
    

    // for Fast Marching Method
    public float Potential;
    public float TempPotential;
    public PotentialType type = PotentialType.Unknown;
    // heap


    public float GetNeighborCost(CC_GridCell neighbor)
    {
        int xdiff = (int)(Coord.x - neighbor.Coord.x);
        int ydiff = (int)(Coord.y - neighbor.Coord.y);

        if (xdiff < 0)
            return West.UnitCost;
        if (xdiff > 0)
            return East.UnitCost;
        if (ydiff < 0)
            return North.UnitCost;
        if (ydiff > 0)
            return South.UnitCost;

        return UnitCost;
    }

    public float GetNeighborCost(int nx, int ny)
    {
        int xdiff = (int)Coord.x - nx;
        int ydiff = (int)Coord.y - ny;

        if (xdiff < 0)
            return West.UnitCost;
        if (xdiff > 0)
            return East.UnitCost;
        if (ydiff < 0)
            return North.UnitCost;
        if (ydiff > 0)
            return South.UnitCost;

        return UnitCost;
    }

    // for debug
    public float Speed;
    public float UnitCost;
    public Vector2 TotalVelocity;
}

public class CC_GroupGrid
{
    public CC_GridCell[,] Grid;
    public List<Vector2> Goals;
    public int iteration = 0;
    public int GroupNumber;
    public List<CC_GridCell> heap;
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
