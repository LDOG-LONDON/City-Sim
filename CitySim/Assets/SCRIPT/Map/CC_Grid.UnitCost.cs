using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CC_Grid {

    float TopographicalSpeed(Vector2 position, Vector2 direction)
    {
        Vector2 coord = Utility.Instance.Vec3ToCoord(position);
        float slope = HeightDifference((int)coord.x, (int)coord.y, direction) - SlopeMin;
        slope = slope / (SlopeMax - SlopeMin);

        return Mathf.Max(Fmax + slope * (Fmin - Fmax),0.01f);
    }

    float FlowSpeed(Vector2 position, Vector2 direction)
    {
        Vector2 displacedPosition = position + direction.normalized;
        if (direction.x < 0 || direction.x > 0)
            direction *= -1f;
        Vector2 coord;
        bool good = Utility.Instance.Vec2ToCoordExists(displacedPosition, out coord);
        if (!good)
        {
            return 0.01f;
        }

        int x = (int)coord.x;
        int y = (int)coord.y;
        Vector2 DisplacedAvgVel = GlobalGrid[x, y].AverageVelocity;
        float speed = Vector2.Dot(DisplacedAvgVel, direction);
        return Mathf.Max(speed, 0.01f);
    }

    float NormalSpeed(Vector2 position, Vector2 direction, float flow, float topographical)
    {
        Vector2 offset = position + AgentRadius * direction.normalized;
        offset = Utility.Instance.Vec3ToCoord(offset);
        int x = (int)offset.x;
        int y = (int)offset.y;
        float densityOffset = GlobalGrid[x, y].Density;

        if (densityOffset >= DensityMax)
            return flow;
        if (densityOffset <= DensityMin)
            return topographical;

        densityOffset = (densityOffset - DensityMin) / (DensityMax - DensityMin);

        return topographical + densityOffset * (flow - topographical);
    }

    float GetSpeedinDirection(int x, int y, Vector2 dir, float good)
    {
        // dont know if this is used somewhere
        Vector2 tile = GetTileInDirection(x, y, dir);
        int tx = (int)tile.x;
        int ty = (int)tile.y;
        
        Vector2 badTile = new Vector2(-1, -1);
        if (tile == badTile)
            return 0f;

        Vector2 worldPos = GlobalGrid[tx, ty].WorldPos;
        Vector2 AvgVel = GlobalGrid[tx, ty].AverageVelocity;
        float density = GlobalGrid[tx, ty].Density;

        if (density <= DensityMin)
            return TopographicalSpeed(worldPos, AvgVel);
        else if (density >= DensityMin && density <= DensityMax)
            return NormalSpeed(worldPos, AvgVel, 1,1);
        else // density >= DensityMax
            return FlowSpeed(worldPos, AvgVel);
    }

    float GetTileSpeed(int x, int y, CC_GroupGrid group)
    {
        CC_GridCell[,] grid = group.Grid;
        CC_GridCell cell = grid[x, y];

        Vector2 worldPos = GlobalGrid[x, y].WorldPos;
        Vector2 AvgVel = GlobalGrid[x, y].AverageVelocity;
        float density = GlobalGrid[x, y].Density;

        float Top = TopographicalSpeed(worldPos, Vector2.right);
        float FlowEast = FlowSpeed(worldPos, Vector2.left);
        float FlowWest = FlowSpeed(worldPos, Vector2.right);
        float FlowNorth = FlowSpeed(worldPos, Vector2.up);
        float FlowSouth = FlowSpeed(worldPos, Vector2.down);

        float finalEast = NormalSpeed(worldPos, Vector2.right, FlowEast, Top);
        float finalWest = NormalSpeed(worldPos, Vector2.left, FlowEast, Top);
        float finalNorth = NormalSpeed(worldPos, Vector2.up, FlowEast, Top);
        float finalSouth = NormalSpeed(worldPos, Vector2.down, FlowEast, Top);

        cell.East.Speed = finalEast;
        cell.West.Speed = finalWest;
        cell.North.Speed = finalNorth;
        cell.South.Speed = finalSouth;

        return (finalEast + finalWest + finalNorth + finalSouth) / 4f;
        //if (density <= DensityMin)
        //    return TopographicalSpeed(worldPos, AvgVel);
        //else if (density > DensityMin && density <= DensityMax)
        //    return NormalSpeed(worldPos, AvgVel);
        //else // density >= DensityMax
        //    return FlowSpeed(worldPos, AvgVel);
    }

    float GetTileUnitCost(int x, int y,CC_GroupGrid group)
    {
        Vector2 worldPos = GlobalGrid[x, y].WorldPos;
        CC_GridCell[,] grid = group.Grid;
        CC_GridCell cell = grid[x, y];
        bool east = Utility.Instance.Vec2ToCoordExists(x - 1, y);
        bool west = Utility.Instance.Vec2ToCoordExists(x +1, y);
        bool north = Utility.Instance.Vec2ToCoordExists(x, y+1);
        bool south = Utility.Instance.Vec2ToCoordExists(x, y-1);
        float discomfort = 0.0f;
        float faceSpeed = 0.0f;
        float eastCost = Mathf.Infinity;
        float westCost = Mathf.Infinity;
        float northCost = Mathf.Infinity;
        float southCost = Mathf.Infinity;
        if (east)
        {
            faceSpeed = cell.East.Speed;
            discomfort = GlobalGrid[x - 1, y].Discomfort;
            eastCost = faceSpeed * DistanceWeight + TimeWeight + discomfort * DiscomfortWeight;
        }
            
        if (west)
        {
            faceSpeed = cell.West.Speed;
            discomfort = GlobalGrid[x + 1, y].Discomfort;
            westCost = faceSpeed * DistanceWeight + TimeWeight + discomfort * DiscomfortWeight;
        }
        if (north)
        {
            faceSpeed = cell.North.Speed;
            discomfort = GlobalGrid[x, y+1].Discomfort;
            northCost = faceSpeed * DistanceWeight + TimeWeight + discomfort * DiscomfortWeight;
        }
        if (south)
        {
            faceSpeed = cell.South.Speed;
            discomfort = GlobalGrid[x, y-1].Discomfort;
            southCost = faceSpeed * DistanceWeight + TimeWeight + discomfort * DiscomfortWeight;
        }

        cell.East.UnitCost = eastCost;
        cell.West.UnitCost = westCost;
        cell.North.UnitCost = northCost;
        cell.South.UnitCost = southCost;

        return (eastCost + westCost + northCost + southCost) / 4.0f;
        //if (speed <= 0f)
        //    return Mathf.Infinity;
        //float density = GlobalGrid[x, y].Density;
        //float cost = DistanceWeight * speed + TimeWeight + DistanceWeight * density;
        //cost /= speed;
        //return cost;
    }

    // dont need anymore
    void GiveSpeed_Cost(int x, int y, int groupNumber, float speed, float cost)
    {
        CC_GridCell[,] group = GroupGridList[groupNumber].Grid;
        if (x > 0)
        {
            group[x - 1, y].West.Speed = speed;
            group[x - 1, y].West.UnitCost = cost;
        }
            
        if (x < Width - 1)
        {
            group[x + 1, y].East.Speed = speed;
            group[x + 1, y].East.UnitCost = cost;
        }
        if (y > 0)
        {
            group[x, y - 1].North.Speed = speed;
            group[x, y - 1].North.UnitCost = cost;
        }
            
        if (y < Height - 1)
        {
            group[x, y + 1].South.Speed = speed;
            group[x, y + 1].South.UnitCost = cost;
        }
            
    }

    void UnitCost(int groupNumber)
    {
        // assuming that the grid cell is affecting speed field
        CC_GroupGrid group = GroupGridList[groupNumber];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float tileSpeed = GetTileSpeed(x, y, group);
                float tileCost = GetTileUnitCost(x, y, group);
                group.Grid[x, y].Speed = tileSpeed;
                group.Grid[x, y].UnitCost = tileCost;
                //GiveSpeed_Cost(x, y, groupNumber, tileSpeed,tileCost);
            }
        }
    }
}
