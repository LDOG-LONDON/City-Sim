using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CC_Grid {

    float QuadraticSolver(float potMX, float potMY, float costMX, float costMY)
    {
        float costMXSqu;
        float costMYSqu;
        float potMXSqu;
        float potMYSqu;
        float pot;

        if ((potMY >= Mathf.Infinity || potMY == float.NaN) &&
            (potMX >= Mathf.Infinity || potMX == float.NaN))
        {
            costMXSqu = costMX * costMX;
            costMYSqu = costMY * costMY;
            return (costMX * costMY) / Mathf.Sqrt(costMXSqu + costMYSqu);
        }
        else if (potMX >= Mathf.Infinity || potMX == float.NaN)
        {
            costMXSqu = costMX * costMX; // Z
            costMYSqu = costMY * costMY; //W
            potMYSqu = potMY * potMY; // Y

            pot = potMY * costMXSqu
                - Mathf.Sqrt(costMYSqu * costMXSqu
                * (costMYSqu - potMYSqu + costMXSqu));
            pot /= (costMYSqu + costMXSqu);

            return pot;
        }
        else if (potMY >= Mathf.Infinity || potMY == float.NaN)
        {
            costMXSqu = costMX * costMX; // Z
            costMYSqu = costMY * costMY; //W
            potMXSqu = potMX * potMX; // X

            pot = potMX * costMYSqu
                - Mathf.Sqrt(costMYSqu * costMXSqu
                * (costMYSqu - potMXSqu + costMXSqu));
            pot /= (costMYSqu + costMXSqu);

            return pot;
        }
        
        costMXSqu = costMX * costMX; // Z
        costMYSqu = costMY * costMY; // W
        potMXSqu = potMX * potMX; // X
        potMYSqu = potMY * potMY; // Y

        pot = -1f * Mathf.Sqrt(costMYSqu * costMXSqu
            * (costMXSqu - potMXSqu
            + (2 * potMX * potMY)
            - potMYSqu + costMYSqu))
            + costMYSqu * potMX
            + costMXSqu * potMY;

        pot /= (costMYSqu + costMXSqu);
        return pot;
    }

    CC_GridCell MinEastWest(CC_GridCell cell, CC_GroupGrid group)
    {
        int x = (int)cell.Coord.x;
        int y = (int)cell.Coord.y;
        CC_GridCell east;
        CC_GridCell west;
        float eastTotal = Mathf.Infinity;
        float westTotal = Mathf.Infinity;

        if (x <= 0)
        {
            return group.Grid[x + 1, y];
        }   
        else if (x >= Width - 1)
        {
            return group.Grid[x - 1, y];
        }
        else
        {
            west = group.Grid[x + 1, y];
            east = group.Grid[x - 1, y];
        }

        if (east != null)
            eastTotal = east.UnitCost + east.Potential;
        else
        {
            if (west.iteration != group.iteration)
            {
                west.iteration = group.iteration;
                west.Potential = Mathf.Infinity;
                west.type = PotentialType.Unknown;
            }
            return west;
        }

        if (west != null)
            westTotal = west.UnitCost + west.Potential;
        else
        {
            if (east.iteration != group.iteration)
            {
                east.iteration = group.iteration;
                east.Potential = Mathf.Infinity;
                east.type = PotentialType.Unknown;
            }
            return east;
        }
         
        if (eastTotal <= westTotal)
            return east;
        else
            return west;
    }

    CC_GridCell MinNorthSouth(CC_GridCell cell, CC_GroupGrid group)
    {
        int x = (int)cell.Coord.x;
        int y = (int)cell.Coord.y;
        CC_GridCell north;
        CC_GridCell south;
        float northTotal = Mathf.Infinity;
        float southTotal = Mathf.Infinity;

        if (y <= 0)
        {
            south = null;
            north = group.Grid[x, y + 1];
        }
        else if (y >= Height - 1)
        {
            north = null;
            south = group.Grid[x, y - 1];
        }
        else
        {
            south = group.Grid[x, y - 1];
            north = group.Grid[x, y + 1];
        }

        

        if (north != null)
        {
            if (north.iteration != group.iteration)
            {
                north.iteration = group.iteration;
                north.Potential = Mathf.Infinity;
                north.type = PotentialType.Unknown;
            }
            northTotal = north.UnitCost + north.Potential;
        }
        else 
        {
            if (south.iteration != group.iteration)
            {
                south.iteration = group.iteration;
                south.Potential = Mathf.Infinity;
                south.type = PotentialType.Unknown;
            }
            return south;
        }

        if (south != null)
        {
            if (south.iteration != group.iteration)
            {
                south.iteration = group.iteration;
                south.Potential = Mathf.Infinity;
                south.type = PotentialType.Unknown;
            }
            southTotal = south.UnitCost + south.Potential;
        }
        else
        {
            if (north.iteration != group.iteration)
            {
                north.iteration = group.iteration;
                north.Potential = Mathf.Infinity;
                north.type = PotentialType.Unknown;
            }
            return north;
        }

        if (southTotal <= northTotal)
            return south;
        else
            return north;
    }

    void FiniteDifferenceApproximation(CC_GridCell cell, CC_GroupGrid group)
    {
        CC_GridCell Mx = MinEastWest(cell, group);
        CC_GridCell My = MinNorthSouth(cell, group);

        cell.Potential = QuadraticSolver(
            Mx.Potential , My.Potential,
            Mx.UnitCost, My.UnitCost);
    }

    void AddToHeap(CC_GridCell cell, CC_GroupGrid group)
    {
        if (group.heap.Contains(cell))
            return;
        ChangeDebugTileColor((int)cell.Coord.x, (int)cell.Coord.y, Color.yellow);
        group.heap.Add(cell);
    }

    CC_GridCell GetCheapestCandidate(CC_GroupGrid group)
    {
        float cheap = Mathf.Infinity;
        CC_GridCell cheapest = null;
        foreach(CC_GridCell cell in group.heap)
        {
            if (cell.Potential < cheap)
            {
                cheap = cell.Potential;
                cheapest = cell;
            }
        }

        if (cheapest != null)
        {
            group.heap.Remove(cheapest);
            cheapest.type = PotentialType.Known;
            ChangeDebugTileColor((int)cheapest.Coord.x, (int)cheapest.Coord.y, Color.blue);
            return cheapest;
        }
        return null;
    }

    void AddCanidates(CC_GridCell cell, CC_GroupGrid group)
    {
        int x = (int)cell.Coord.x;
        int y = (int)cell.Coord.y;

        if (x > 0)
        {
            CC_GridCell east = group.Grid[x - 1, y];
            if (east.iteration != group.iteration)
            {
                east.iteration = group.iteration;
                east.Potential = Mathf.Infinity;
                east.type = PotentialType.Unknown;
            }
            if (east.type != PotentialType.Known)
            {
                east.type = PotentialType.Candidate;
                FiniteDifferenceApproximation(east, group);
                AddToHeap(east, group);
            }
        }
        if (x < Width-1)
        {
            CC_GridCell west = group.Grid[x + 1, y];
            if (west.iteration != group.iteration)
            {
                west.iteration = group.iteration;
                west.Potential = Mathf.Infinity;
                west.type = PotentialType.Unknown;
            }
            if (west.type != PotentialType.Known)
            {
                west.type = PotentialType.Candidate;
                FiniteDifferenceApproximation(west, group);
                AddToHeap(west, group);
            }
        }
        if (y > 0)
        {
            CC_GridCell south = group.Grid[x, y-1];
            if (south.iteration != group.iteration)
            {
                south.iteration = group.iteration;
                south.Potential = Mathf.Infinity;
                south.type = PotentialType.Unknown;
            }
            if (south.type != PotentialType.Known)
            {
                south.type = PotentialType.Candidate;
                FiniteDifferenceApproximation(south, group);
                AddToHeap(south, group);
            }
        }
        if (y < Height-1)
        {
            CC_GridCell north = group.Grid[x, y + 1];
            if (north.iteration != group.iteration)
            {
                north.iteration = group.iteration;
                north.Potential = Mathf.Infinity;
                north.type = PotentialType.Unknown;
            }
            if (north.type != PotentialType.Known)
            {
                north.type = PotentialType.Candidate;
                FiniteDifferenceApproximation(north, group);
                AddToHeap(north, group);
            }
        }


    }

    void Potential(int groupNumber)
    {
        CC_GroupGrid groupGrid = GroupGridList[groupNumber];
        CC_GridCell[,] grid = GroupGridList[groupNumber].Grid;
        groupGrid.iteration++;
        groupGrid.heap.Clear();
        groupGrid.Goals.Clear();
        groupGrid.Goals.Add(new Vector2(0f, 0f));
        ClearDebugTileColor();

        foreach(Vector2 goal in groupGrid.Goals)
        {
            int x = (int)goal.x;
            int y = (int)goal.y;
            CC_GridCell goalCell = grid[x, y];
            goalCell.type = PotentialType.Known;
            goalCell.Potential = 0f;
            goalCell.iteration = groupGrid.iteration;

            ChangeDebugTileColor(x, y, Color.blue);

            AddCanidates(goalCell, groupGrid);
        }

        while(groupGrid.heap.Count > 0)
        {
            CC_GridCell cur = GetCheapestCandidate(groupGrid);
            if (cur == null)
                return;

            AddCanidates(cur, groupGrid);
        }
    }

}
