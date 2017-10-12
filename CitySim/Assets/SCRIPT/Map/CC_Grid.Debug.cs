using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CC_Grid {

    void InitDebug()
    {
        if (holder)
        {
            foreach (Transform obj in Db_TileList)
            {
                Destroy(obj.gameObject);
            }
            Destroy(holder);
        }

        Db_TileList = new List<Transform>();
        Db_TextList = new List<TextMesh>();
        Db_MeshList = new List<MeshRenderer>();
        Db_LineGrid = new LineRenderer[Width, Height];

        Db_TextGrid = new List<TextMesh>[Width, Height];
        Db_VelocitiesGrid = new List<LineRenderer>[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Db_TextGrid[x, y] = new List<TextMesh>();
                Db_VelocitiesGrid[x, y] = new List<LineRenderer>();
            }
        }

        Db_MeshGrid = new MeshRenderer[Width, Height];

        holder = new GameObject("DebugHolder");
    }

    void SetUpDebug(int x, int y, Vector3 WorldPos)
    {
        WorldPos.z += 0.25f;
        GameObject dbTile = (GameObject)Instantiate(
            Resources.Load("DebugTile"),
            WorldPos,
            Quaternion.identity);

        dbTile.transform.localScale =
            new Vector3(Utility.Instance.TileWidth,
            Utility.Instance.TileHeight,
            0.13f);

        //transform
        Db_TileList.Add(dbTile.transform);

        // debug text
        TextMesh text = dbTile.transform.GetChild(0).GetComponent<TextMesh>();
        LineRenderer velocity;
        text.text = new Vector2((float)x, (float)y).ToString();
        Db_TextList.Add(text);
        Db_TextGrid[x, y].Add(text);
        // north tile text & vel
        text = dbTile.transform.GetChild(1).GetComponent<TextMesh>();
        velocity = text.GetComponent<LineRenderer>();
        velocity.enabled = false;
        Db_VelocitiesGrid[x, y].Add(velocity);
        text.text = "";
        Db_TextGrid[x, y].Add(text);
        // west tile text
        text = dbTile.transform.GetChild(2).GetComponent<TextMesh>();
        velocity = text.GetComponent<LineRenderer>();
        velocity.enabled = false;
        Db_VelocitiesGrid[x, y].Add(velocity);
        text.text = "";
        Db_TextGrid[x, y].Add(text);
        // south tile text
        text = dbTile.transform.GetChild(3).GetComponent<TextMesh>();
        velocity = text.GetComponent<LineRenderer>();
        velocity.enabled = false;
        Db_VelocitiesGrid[x, y].Add(velocity);
        text.text = "";
        Db_TextGrid[x, y].Add(text);
        // east tile text
        text = dbTile.transform.GetChild(4).GetComponent<TextMesh>();
        velocity = text.GetComponent<LineRenderer>();
        velocity.enabled = false;
        Db_VelocitiesGrid[x, y].Add(velocity);
        text.text = "";
        Db_TextGrid[x, y].Add(text);

        // debug line draw
        LineRenderer line = dbTile.GetComponent<LineRenderer>();
        line.enabled = false;
        Db_LineGrid[x, y] = line;


        // debug mesh (for coloring)
        MeshRenderer mesh = dbTile.GetComponent<MeshRenderer>();
        Db_MeshList.Add(mesh);
        Db_MeshGrid[x, y] = mesh;

        dbTile.transform.parent = holder.transform;

    }

    void DrawTileVelocity(int x, int y)
    {
        Vector3 avgVel = GlobalGrid[x, y].AverageVelocity;
        Vector3 pos = GlobalGrid[x, y].WorldPos;
        pos.z += 0.5f;
        Vector3 end = pos + avgVel;
        LineRenderer line = Db_LineGrid[x, y];
        line.enabled = true;
        line.SetPosition(0, pos);
        line.SetPosition(1, end);
    }

    void DrawFaceVelocity(int x, int y, int group)
    {
        //CC_GridCell[,] grid = GroupGridList[group].Grid;
        //CC_GridCell cell = grid[x, y];
        //
        //Vector3 velEast = cell.East.Velocity / 4;
        //Vector3 velWest = cell.West.Velocity / 4;
        //Vector3 velNorth = cell.North.Velocity / 4;
        //Vector3 velSouth = cell.South.Velocity /4;
        //
        //Vector3 pos = GlobalGrid[x, y].WorldPos;
        //Vector3 posEast = pos + Vector3.left * 0.2f + Vector3.forward * 0.5f;
        //Vector3 posWest = pos + Vector3.right * 0.2f + Vector3.forward * 0.5f;
        //Vector3 posNorth = pos + Vector3.up * 0.2f + Vector3.forward * 0.5f;
        //Vector3 posSouth = pos + Vector3.down * 0.2f + Vector3.forward * 0.5f;
        //
        //LineRenderer vel = Db_VelocitiesGrid[x, y][0];
        //vel.enabled = true;
        //vel.positionCount = 2;
        //vel.SetPosition(0, posNorth);
        //vel.SetPosition(1, posNorth + velNorth);
        //
        //vel = Db_VelocitiesGrid[x, y][1];
        //vel.enabled = true;
        //vel.positionCount = 2;
        //vel.SetPosition(0, posSouth);
        //vel.SetPosition(1, posSouth + velSouth);
        //
        //vel = Db_VelocitiesGrid[x, y][2];
        //vel.enabled = true;
        //vel.positionCount = 2;
        //vel.SetPosition(0, posWest);
        //vel.SetPosition(1, posWest + velWest);
        //
        //vel = Db_VelocitiesGrid[x, y][3];
        //vel.enabled = true;
        //vel.positionCount = 2;
        //vel.SetPosition(0, posEast);
        //vel.SetPosition(1, posEast + velEast);
    }

    void DrawCellTotalVelocity(int x, int y, int group)
    {
        LineRenderer line = Db_LineGrid[x, y];
        Vector3 Vel = GroupGridList[group].Grid[x, y].TotalVelocity;
        Vector3 pos = GlobalGrid[x, y].WorldPos;
        pos.z += 0.5f;
        Vector3 end = pos + Vel;
        line.enabled = true;
        line.SetPosition(0, pos);
        line.SetPosition(1, end);
    }

    void TurnOffLines(int x, int y)
    {
        LineRenderer line = Db_LineGrid[x, y];
        line.enabled = false;
    }

    void TurnOffFaceVelocity(int x, int y, int group)
    {
        foreach (LineRenderer vel in Db_VelocitiesGrid[x, y])
        {
            vel.enabled = false;
        }
    }

    void PotentialColor(int groupNum, Color color)
    {
        CC_GroupGrid group = GroupGridList[groupNum];
        CC_GridCell[,] grid = group.Grid;

        float max = -Mathf.Infinity;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (grid[x, y].Potential > max)
                    max = grid[x, y].Potential;
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float mult = grid[x, y].Potential / max;
                Db_MeshGrid[x, y].material.color = color * mult;
            }
        }
    }

    void SpeedColor(int groupNum, Color color)
    {
        CC_GroupGrid group = GroupGridList[groupNum];
        CC_GridCell[,] grid = group.Grid;

        float max = -Mathf.Infinity;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (grid[x, y].Speed > max)
                    max = grid[x, y].Speed;
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float mult = (max + 0.1f - max- grid[x, y].Speed) / max;
                Db_MeshGrid[x, y].material.color = color * mult;
            }
        }
    }

    void UnitCostColor(int groupNum, Color color)
    {
        CC_GroupGrid group = GroupGridList[groupNum];
        CC_GridCell[,] grid = group.Grid;

        float max = -Mathf.Infinity;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (grid[x, y].UnitCost > max && grid[x, y].UnitCost < Mathf.Infinity)
                    max = grid[x, y].UnitCost;
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float mult = (max + 0.1f - grid[x, y].UnitCost) / max;
                Db_MeshGrid[x, y].material.color = color * mult;
            }
        }
    }

    void DensityColor(Color color)
    {
        float max = -Mathf.Infinity;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (GlobalGrid[x, y].Density > max)
                    max = GlobalGrid[x, y].Density;
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float mult = GlobalGrid[x, y].Density / max;
                Db_MeshGrid[x, y].material.color = color * mult;
            }
        }
    }

    void Debug()
    {
        bool on = DebugManager.Instance.DB_On;
        DB_Color colorType = DebugManager.Instance.DebugColor;
        DB_Text textType = DebugManager.Instance.DebugText;
        DB_Line lineType = DebugManager.Instance.DebugLine;
        int CCGroup = DebugManager.Instance.DB_CCGroupNumber;
        if (CCGroup >= GroupGridList.Count)
            CCGroup = 0;

       //if (Utility.Instance.DebugInfo == false && debugOn == true)
       //{
       //    foreach (MeshRenderer obj in Db_MeshList)
       //    {
       //        obj.enabled = false;
       //    }
       //    foreach (TextMesh obj in Db_TextList)
       //    {
       //        obj.transform.GetComponent<MeshRenderer>().enabled = false;
       //    }
       //    debugOn = false;
       //}
       //
       //if (Utility.Instance.DebugInfo == true && debugOn == false)
       //{
       //    foreach (MeshRenderer obj in Db_MeshList)
       //    {
       //        obj.enabled = true;
       //    }
       //    foreach (TextMesh obj in Db_TextList)
       //    {
       //        obj.transform.GetComponent<MeshRenderer>().enabled = true;
       //    }
       //    debugOn = true;
       //}

        if (on == true)
        {
            if (colorType == DB_Color.Potential)
                PotentialColor(CCGroup, Color.blue);
            else if (colorType == DB_Color.Density)
                DensityColor(Color.gray);
            else if (colorType == DB_Color.Speed)
                SpeedColor(CCGroup, Color.red);
            else if (colorType == DB_Color.UnitCost)
                UnitCostColor(CCGroup, Color.yellow);

                for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float density = GlobalGrid[x, y].Density;
                    Vector2 avgVel = GlobalGrid[x, y].AverageVelocity;
                    float height = GlobalGrid[x, y].Height;
                    
                    string debug = "";
                    string north = "";
                    string west = "";
                    string south = "";
                    string east = "";

                    if (textType == DB_Text.AverageVelocity)
                        debug = avgVel.ToString("F2");
                    else if (textType == DB_Text.Density)
                        debug = density.ToString("F2");
                    else if (textType == DB_Text.Height)
                        debug = height.ToString("F2");
                    else if (textType == DB_Text.Potential)
                    {
                        CC_GridCell[,] group = GroupGridList[CCGroup].Grid;
                        debug = group[x, y].Potential.ToString("F2");
                    }
                    else if (textType == DB_Text.CostFields)
                    {
                        CC_GridCell[,] group = GroupGridList[CCGroup].Grid;
                        //north = group[x, y].North.UnitCost.ToString("F2");
                        //south = group[x, y].South.UnitCost.ToString("F2");
                        //west = group[x, y].West.UnitCost.ToString("F2");
                        //east = group[x, y].East.UnitCost.ToString("F2");
                        debug = group[x, y].UnitCost.ToString("F2");
                    }
                    else if (textType == DB_Text.SpeedFields)
                    {
                        CC_GridCell[,] group = GroupGridList[CCGroup].Grid;
                        //north = group[x, y].North.Speed.ToString("F2");
                        //south = group[x, y].South.Speed.ToString("F2");
                        //west = group[x, y].West.Speed.ToString("F2");
                        //east = group[x, y].East.Speed.ToString("F2");
                        debug = group[x, y].Speed.ToString("F2");
                    }
                    else if (textType == DB_Text.PotentialGradients)
                    {
                        CC_GridCell[,] group = GroupGridList[CCGroup].Grid;
                        north = group[x, y].North.PotentialGradient.ToString("F2");
                        south = group[x, y].South.PotentialGradient.ToString("F2");
                        west = group[x, y].West.PotentialGradient.ToString("F2");
                        east = group[x, y].East.PotentialGradient.ToString("F2");
                    }

                    if (lineType == DB_Line.AverageVelocity)
                        DrawTileVelocity(x, y);
                    else if (lineType == DB_Line.CellVelocity)
                        DrawCellTotalVelocity(x, y, CCGroup);
                    else
                        TurnOffLines(x, y);

                    if (DebugManager.Instance.CellFaceVelocities == true)
                        DrawFaceVelocity(x, y, CCGroup);
                    else
                        TurnOffFaceVelocity(x, y, CCGroup);

                    Db_TextGrid[x, y][0].text = debug;
                    Db_TextGrid[x, y][1].text = north;
                    Db_TextGrid[x, y][2].text = west;
                    Db_TextGrid[x, y][3].text = south;
                    Db_TextGrid[x, y][4].text = east;
                }
            }
        }
    }

    void ChangeDebugTileColor(int x, int y, Color color)
    {
        Db_MeshGrid[x, y].material.color = color;
    }

    void ClearDebugTileColor()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                ChangeDebugTileColor(x, y, Color.white);
            }
        }
    }
}
