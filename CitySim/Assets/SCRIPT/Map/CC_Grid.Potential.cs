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
            east = null;
            return group.Grid[x + 1, y];
        }   
        else if (x >= Width - 1)
        {
            west = null;
            return group.Grid[x - 1, y];
        }
        else
        {
            west = group.Grid[x + 1, y];
            east = group.Grid[x - 1, y];
            eastTotal = east.UnitCost + east.Potential;
            westTotal = west.UnitCost + west.Potential;

            if (eastTotal <= westTotal)
                return east;
            else
                return west;
        }
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
            return group.Grid[x, y + 1];
        }
        else if (y >= Height - 1)
        {
            north = null;
            return group.Grid[x, y - 1];
        }

        south = group.Grid[x, y - 1];
        north = group.Grid[x, y + 1];

        northTotal = north.UnitCost + north.Potential;
        southTotal = south.UnitCost + south.Potential;
        if (southTotal <= northTotal)
            return south;
        else
            return north;

    }

    float Scenario2(CC_GridCell cell, CC_GridCell Mx, CC_GridCell My)
    {
        float pot1 = Mathf.Infinity;
        float pot2 = Mathf.Infinity;
        float potFinal = Mathf.Infinity;

        float MxCost = cell.GetNeighborCost(Mx);
        float MyCost = cell.GetNeighborCost(My);

        if (Mx.Potential < My.Potential) //
        {
            pot1 = Mx.Potential + MxCost;
            pot2 = Mx.Potential - MxCost;
            potFinal = Mathf.Max(pot1, pot2);
        }
        else // Mx.Potential > My.Potential ()
        {
            pot1 = My.Potential + MyCost;
            pot2 = My.Potential - MyCost;
            potFinal = Mathf.Max(pot1, pot2);
        }
        return potFinal;
    }

    float Scenario3(CC_GridCell Mx, CC_GridCell My, CC_GridCell cell, CC_GroupGrid group, float pot)
    {
        int x = (int)cell.Coord.x;
        int y = (int)cell.Coord.y;
        bool east = false;
        bool west = false;
        bool south = false;
        bool north = false;

        float eastPot = Mathf.Infinity;
        float westPot = Mathf.Infinity;
        float southPot = Mathf.Infinity;
        float northPot = Mathf.Infinity;

        if (x - 1 >= 0)
        {
            eastPot = group.Grid[x - 1, y].Potential;
            if (pot < eastPot)
                east = true;
        }
        if (x +1 <= Width-1)
        {
            westPot = group.Grid[x + 1, y].Potential;
            if (pot < westPot)
                west = true;
        }
        if (y - 1 >= 0)
        {
            southPot = group.Grid[x, y - 1].Potential;
            if (pot < southPot)
                south = true;
        }
        if (y+1 <= Height-1)
        {
            northPot = group.Grid[x, y + 1].Potential;
            if (pot < northPot)
                north = true;
        }

        if (north && south && east && west)
            return Scenario2(cell, Mx, My);
        else
            return pot;

    }

    float CellFiniteDiff(CC_GridCell cell, CC_GridCell neighbor, CC_GroupGrid group)
    {
        float cost = cell.GetNeighborCost(neighbor);
        float pot = neighbor.Potential;

        float pot1 = pot - cost;
        float pot2 = pot + cost;

        return Mathf.Max(pot1, pot2);
    }

    float CellsFiniteDiff(CC_GridCell cell, CC_GridCell Mx, CC_GridCell My, CC_GroupGrid group)
    {
        float MxCost = cell.GetNeighborCost(Mx);
        float MyCost = cell.GetNeighborCost(My);

        float a = (MxCost * MxCost) + (MyCost * MyCost);
        float b = -2.0f * ((MxCost * MxCost * My.Potential) + (MyCost * MyCost * Mx.Potential));
        float c = (MxCost * MxCost) * (My.Potential * My.Potential)
            + (MyCost * MyCost) * (Mx.Potential * Mx.Potential)
            - (MxCost * MxCost) * (MyCost * MyCost);

        float pot1 = Mathf.Infinity;
        float pot2 = Mathf.Infinity;
        float potFinal = Mathf.Infinity;

        float quad1 = (b * b) - (4f * a * c);
        
        if (quad1 >= 0)
        {
            float denom = 1 / (2 * a);
            float sqrt = Mathf.Sqrt(quad1);
            pot1 = (-b - sqrt) * denom;
            pot2 = (-b + sqrt) * denom;
            potFinal = Mathf.Max(pot1, pot2);
            if (potFinal >= Mx.Potential && potFinal >= My.Potential)
                return potFinal;
        }

        if (Mx.Potential < My.Potential)
            return CellFiniteDiff(cell, Mx, group);
        else
            return CellFiniteDiff(cell, My, group);
    }

    bool GetCellMin(CC_GridCell cell, bool horizontal,CC_GridCell[,] grid, out CC_GridCell outCell)
    {
        int x = (int)cell.Coord.x;
        int y = (int)cell.Coord.y;

        int x1 = x;
        int y1 = y;
        int x2 = x;
        int y2 = y;
        bool lower = true;
        bool higher = true;

        if (horizontal)
        {
            x1--;
            x2++;
        }
        else
        {
            y1--;
            y2++;
        }

        float pot;
        float cost;
        float lowerTotal = 0f;
        float higherTotal = 0f;

        CC_GridCell higherCell = null;
        CC_GridCell lowerCell = null;

        if (Utility.Instance.Vec2ToCoordExists(x1, y1))
        {
            lowerCell = grid[x1, y1];
            pot = lowerCell.Potential;
            if (pot < Mathf.Infinity)
            {
                cost = cell.GetNeighborCost(lowerCell);
                if (cost < Mathf.Infinity)
                    lowerTotal = pot + cost;
                else
                    lower = false;
            }
            else
                lower = false;
        }
        else
            lower = false;

        if (Utility.Instance.Vec2ToCoordExists(x2, y2))
        {
            higherCell = grid[x2, y2];
            pot = higherCell.Potential;
            if (pot < Mathf.Infinity)
            {
                cost = cell.GetNeighborCost(higherCell);
                if (cost < Mathf.Infinity)
                    higherTotal = pot + cost;
                else
                    higher = false;
            }
            else
                higher = false;
        }
        else
            higher = false;


        outCell = cell;
        if (higher == false && lower == false)
            return false;
        else if (higher == false)
        {
            outCell = lowerCell;
            return true;
        } 
        else if (lower == false)
        {
            outCell = higherCell;
            return true;
        }
        if (lowerTotal < higherTotal)
            outCell = lowerCell;
        else
            outCell = higherCell;
        return true;
    }

    void FiniteDifferenceApproximation(CC_GridCell cell, CC_GroupGrid group)
    {
        CC_GridCell Mx;
        CC_GridCell My;

        bool MxGood = GetCellMin(cell, true, group.Grid, out Mx);
        bool MyGood = GetCellMin(cell, false, group.Grid, out My);

        //float MxCost = cell.GetNeighborCost(Mx);
        //float MyCost = cell.GetNeighborCost(My);
        //
        //float a = (MxCost * MxCost) + (MyCost * MyCost);
        //float b = -2.0f * (MxCost * My.Potential + MyCost * Mx.Potential);
        //float c = (MxCost * MxCost) * (My.Potential * My.Potential)
        //    + (MyCost * MyCost) * (Mx.Potential * Mx.Potential)
        //    - (MxCost * MxCost) * (MyCost * MyCost);
        //
        //float pot1 = Mathf.Infinity;
        //float pot2 = Mathf.Infinity;
        //float potFinal = Mathf.Infinity;
        //
        //float quad1 = b * b - (4f * a * c);

        // scenario 2
        //if (quad1 < 0)
        //{
        //    potFinal = Scenario2(cell, Mx, My);
        //    cell.Potential = potFinal;
        //    return;
        //}

       float FinalPot = 0f;
       if (!MxGood)
       {
           if (!MyGood)
               FinalPot = 0f;
           else
               FinalPot = CellFiniteDiff(cell, My, group);
       }
       else if (!MyGood)
           FinalPot = CellFiniteDiff(cell, Mx, group);
       else
           FinalPot = CellsFiniteDiff(cell, Mx, My, group);
       
       cell.TempPotential = FinalPot; 
       
        

        //potFinal = Scenario2(Mx, My);
        //cell.Potential = potFinal;

         //scenario 1
        //if ((Mx.UnitCost >= Mathf.Infinity || Mx.Potential >= Mathf.Infinity))
        //{
        //    pot1 = My.Potential + MyCost;
        //    pot2 = My.Potential - MyCost;
        //    potFinal = Mathf.Max(pot1, pot2);
        //}
        //else if ((My.UnitCost >= Mathf.Infinity || My.Potential >= Mathf.Infinity))
        //{
        //    pot1 = Mx.Potential + MxCost;
        //    pot2 = Mx.Potential - MxCost;
        //    potFinal = Mathf.Max(pot1, pot2);
        //}
        //else
        //{
        //    pot1 = -b + Mathf.Sqrt(b * b - 4 * a * c) / 2 * a;
        //    pot2 = -b - Mathf.Sqrt(b * b - 4 * a * c) / 2 * a;
        //    potFinal = Mathf.Max(pot1, pot2);
        //}
        //  
        //// scenario 3
        //float potFinal2 = Scenario3(Mx, My, cell, group, potFinal);
        //cell.Potential = potFinal2;
    }

    void AddToHeap(CC_GridCell cell, CC_GroupGrid group)
    {
        if (group.heap.Contains(cell))
            return;
        //ChangeDebugTileColor((int)cell.Coord.x, (int)cell.Coord.y, Color.yellow);
        group.heap.Add(cell);
    }

    CC_GridCell GetCheapestCandidate(CC_GroupGrid group)
    {
        float cheap = Mathf.Infinity;
        CC_GridCell cheapest = null;
        foreach(CC_GridCell cell in group.heap)
        {
            if (cell.TempPotential < cheap)
            {
                cheap = cell.TempPotential;
                cheapest = cell;
            }
        }

        if (cheapest != null)
        {
            group.heap.Remove(cheapest);
            cheapest.Potential = cheapest.TempPotential;
            cheapest.type = PotentialType.Known;
            GoalList.Add(cheapest);
            //ChangeDebugTileColor((int)cheapest.Coord.x, (int)cheapest.Coord.y, Color.blue);
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
            if (east.type != PotentialType.Known &&
                east.type != PotentialType.Candidate)
            {
                east.type = PotentialType.Candidate;
                AddToHeap(east, group);
            }
        }
        if (x < Width-1)
        {
            CC_GridCell west = group.Grid[x + 1, y];
            if (west.type != PotentialType.Known &&
                west.type != PotentialType.Candidate)
            {
                west.type = PotentialType.Candidate;
                AddToHeap(west, group);
            }
        }
        if (y > 0)
        {
            CC_GridCell south = group.Grid[x, y-1];
            if (south.type != PotentialType.Known &&
                south.type != PotentialType.Candidate)
            {
                south.type = PotentialType.Candidate;
                AddToHeap(south, group);
            }
        }
        if (y < Height-1)
        {
            CC_GridCell north = group.Grid[x, y + 1];
            if (north.type != PotentialType.Known &&
                north.type != PotentialType.Candidate)
            {
                north.type = PotentialType.Candidate;
                AddToHeap(north, group);
            }
        }
    }

    void CalculateGradient(int x, int y, CC_GroupGrid groupGrid)
    {
        float eastPot = 0f;
        float westPot = 0f;
        float southPot = 0f;
        float northPot = 0f;

        float pot = groupGrid.Grid[x, y].Potential;

        float gradEast;
        float gradWest;
        float gradSouth;
        float gradNorth;

        if (x + 1 <= Width - 1)
        {
            westPot = groupGrid.Grid[x + 1, y].Potential;
            gradWest = westPot - pot;
        }
        else
            gradWest = 0f;

        if (x - 1 >= 0)
        {
            eastPot = groupGrid.Grid[x - 1, y].Potential;
            gradEast = eastPot - pot;
        }
        else
            gradEast = 0f;

        if (y + 1 <= Height - 1)
        {
            northPot = groupGrid.Grid[x, y + 1].Potential;
            gradNorth = northPot - pot;
        }
        else
            gradNorth = 0f;

        if (y - 1 >= 0)
        {
            southPot = groupGrid.Grid[x, y - 1].Potential;
            gradSouth = southPot - pot;
        }
        else
            gradSouth = 0f;

        float deltaX = gradEast - gradWest;
        float deltaY = gradNorth - gradSouth;

        Vector2 gradVec = new Vector2(deltaX, deltaY).normalized;

        float gradX = gradVec.x;
        float gradY = gradVec.y;

        float multx = 1.0f;
        float multy = 1.0f;

        if (deltaX != 0)
            multx = gradX / deltaX;
        if (deltaY != 0)
            multy = gradY / deltaY;

        //float max = Mathf.Max(Mathf.Abs(gradEast), Mathf.Abs(gradWest), Mathf.Abs(gradSouth), Mathf.Abs(gradNorth));
        //if (max != 0)
        //{
        //    gradEast /= max;
        //    gradWest /= max;
        //    gradNorth /= max;
        //    gradSouth /= max;
        //}

        gradEast *= multx;
        gradWest *= multx;
        gradNorth *= multy;
        gradSouth *= multy;

        groupGrid.Grid[x, y].South.PotentialGradient = gradSouth;
        groupGrid.Grid[x, y].North.PotentialGradient = gradNorth;
        groupGrid.Grid[x, y].East.PotentialGradient = gradEast;
        groupGrid.Grid[x, y].West.PotentialGradient = gradWest;
    }

    void CalculateCellVelocity(int x, int y, CC_GroupGrid groupGrid)
    {
        CC_GridCell[,] grid = groupGrid.Grid;
        CC_GridCell cell = grid[x, y];

        if (cell.Potential <= 0.0f)
        {
            cell.TotalVelocity = Vector2.zero;
            return;
        }

        float gradEast = cell.East.PotentialGradient;
        float gradWest = cell.West.PotentialGradient;
        float gradNorth = cell.North.PotentialGradient;
        float gradSouth = cell.South.PotentialGradient;

        float speedEast = groupGrid.Grid[x, y].East.Speed;
        float speedWest = groupGrid.Grid[x, y].West.Speed;
        float speedNorth = groupGrid.Grid[x, y].North.Speed;
        float speedSouth = groupGrid.Grid[x, y].South.Speed;

        float velEast = gradEast * -speedEast;
        float velWest = gradWest * -speedWest;
        float velSouth = gradSouth * -speedSouth;
        float velNorth = gradNorth * -speedNorth;

        groupGrid.Grid[x, y].East.Velocity = velEast;
        groupGrid.Grid[x, y].West.Velocity = velWest;
        groupGrid.Grid[x, y].South.Velocity = velSouth;
        groupGrid.Grid[x, y].North.Velocity = velNorth;
        groupGrid.Grid[x, y].TotalVelocity = new Vector2(velWest - velEast, velNorth - velSouth);
    }

    void ResetGroupGrid(CC_GridCell[,] grid)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                grid[x, y].Potential = Mathf.Infinity;
                grid[x, y].type = PotentialType.Unknown;
            }
        }
    }

    void Potential(int groupNumber)
    {
        CC_GroupGrid groupGrid = GroupGridList[groupNumber];
        CC_GridCell[,] grid = GroupGridList[groupNumber].Grid;
        GoalList.Clear();
        ResetGroupGrid(grid);
        groupGrid.iteration++;
        groupGrid.heap.Clear();
        groupGrid.Goals.Clear();
       groupGrid.Goals.Add(new Vector2(7f, 7f));
       groupGrid.Goals.Add(new Vector2(7f, 8f));
       groupGrid.Goals.Add(new Vector2(7f, 6f));
       groupGrid.Goals.Add(new Vector2(8f, 7f));
       groupGrid.Goals.Add(new Vector2(6f, 7f));
       groupGrid.Goals.Add(new Vector2(6f, 6f));
       groupGrid.Goals.Add(new Vector2(8f, 8f));
       groupGrid.Goals.Add(new Vector2(8f, 6f));
       groupGrid.Goals.Add(new Vector2(6f, 6f));
        //
        //groupGrid.Goals.Add(new Vector2(1f, 7f));
        //groupGrid.Goals.Add(new Vector2(1f, 8f));
        //groupGrid.Goals.Add(new Vector2(1f, 6f));
        //groupGrid.Goals.Add(new Vector2(1f, 5f));

        //groupGrid.Goals.Add(new Vector2(13f, 7f));
        //groupGrid.Goals.Add(new Vector2(13f, 8f));
        //groupGrid.Goals.Add(new Vector2(13f, 6f));
        //groupGrid.Goals.Add(new Vector2(13f, 5f));

        foreach (Vector2 goal in groupGrid.Goals)
        {
            int x = (int)goal.x;
            int y = (int)goal.y;
            CC_GridCell goalCell = grid[x, y];
            goalCell.type = PotentialType.Known;
            goalCell.Potential = 0f;
            GoalList.Add(goalCell);
            
        }

        foreach (Vector2 goal in groupGrid.Goals)
        {
            int x = (int)goal.x;
            int y = (int)goal.y;
            CC_GridCell goalCell = grid[x, y];
            AddCanidates(goalCell, groupGrid);
        }

        int i = 0;
        while(groupGrid.heap.Count > 0)
        {
            foreach (CC_GridCell canidate in groupGrid.heap)
            {
                FiniteDifferenceApproximation(canidate, groupGrid);
            }

            CC_GridCell cur = GetCheapestCandidate(groupGrid);
            if (cur == null)
                return;

            AddCanidates(cur, groupGrid);
            i++;
        }

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                CalculateGradient(x, y, groupGrid);

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                CalculateCellVelocity(x, y, groupGrid);
    }

}
