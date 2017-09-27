using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CC_Grid {

    float TopographicalSpeed(Vector2 position, Vector2 direction)
    {
        Vector2 coord = Utility.Instance.Vec3ToCoord(position);
        float slope = HeightDifference((int)coord.x, (int)coord.y, direction) - SlopeMin;
        slope = slope / (SlopeMax - SlopeMin);

        return Fmax + slope * (Fmin - Fmax);
    }

    float FlowSpeed(Vector2 position, Vector2 direction)
    {
        Vector2 displacedPosition = position + AgentRadius * direction.normalized;
        Vector2 displacedCoord = Utility.Instance.Vec3ToCoord(displacedPosition);
        int x = (int)Mathf.Clamp(displacedCoord.x, 0, Width - 1);
        int y = (int)Mathf.Clamp(displacedCoord.y, 0, Height - 1);
        Vector2 DisplacedAvgVel = GlobalGrid[x, y].AverageVelocity;
        return Vector3.Dot(DisplacedAvgVel, direction);
    }

    float NormalSpeed(Vector2 position, Vector2 direction)
    {
        float TSpeed = TopographicalSpeed(position, direction);
        float FSpeed = FlowSpeed(position, direction);

        Vector2 offset = position + AgentRadius * direction.normalized;
        offset = Utility.Instance.Vec3ToCoord(offset);
        int x = (int)Mathf.Clamp(offset.x, 0, Width - 1);
        int y = (int)Mathf.Clamp(offset.y, 0, Height - 1);
        float densityOffset = GlobalGrid[x, y].Density;
        densityOffset = (densityOffset - DensityMin) / (DensityMax - DensityMin);

        return TSpeed + densityOffset * (FSpeed - TSpeed);
    }

    float GetSpeedinDirection(int x, int y, Vector2 dir)
    {
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
            return NormalSpeed(worldPos, AvgVel);
        else // density >= DensityMax
            return FlowSpeed(worldPos, AvgVel);
    }

    void UnitCost()
    {
        // assuming that the agent is affecting the speed field
        CC_GridCell[,] groupGrid = GroupGridList[0];
        foreach (Transform agent in AgentList)
        {
            Vector3 agentPos = agent.position;
            Vector2 agentDir = agent.forward;
            Vector2 agentCoord = Utility.Instance.Vec3ToCoord(agentPos);
            int x = (int)agentCoord.x;
            int y = (int)agentCoord.y;
            float coordDensity = GlobalGrid[x, y].Density;
            float speed = 0f;

            if (coordDensity <= DensityMin)
            {
                speed = TopographicalSpeed(agentPos, agentDir);
            }
            else if (coordDensity >= DensityMin && coordDensity <= DensityMax)
            {
                speed = NormalSpeed(agentPos, agentDir);
            }
            else // coordDensity >= DensityMax
            {
                speed = FlowSpeed(agentPos, agentDir);
            }
        }

        // assuming that the grid cell is affecting speed field
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3 worldPos = GlobalGrid[x, y].WorldPos;
                float cellDensity = GlobalGrid[x, y].Density;
                Vector2 cellVelocity = GlobalGrid[x, y].AverageVelocity;
                float speedEast = 0f;
                float speedWest = 0f;
                float speedNorth = 0f;
                float speedSouth = 0f;

                
            }
        }
    }
}
