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
        for(int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Db_TextGrid[x,y] = new List<TextMesh>();
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
        text.text = new Vector2((float)x, (float)y).ToString();
        Db_TextList.Add(text);
        Db_TextGrid[x, y].Add(text);
             // north tile text
        text = dbTile.transform.GetChild(1).GetComponent<TextMesh>();
        text.text = "";
        Db_TextGrid[x, y].Add(text);
            // west tile text
        text = dbTile.transform.GetChild(2).GetComponent<TextMesh>();
        text.text = "";
        Db_TextGrid[x, y].Add(text);
            // south tile text
        text = dbTile.transform.GetChild(3).GetComponent<TextMesh>();
        text.text = "";
        Db_TextGrid[x, y].Add(text);
             // east tile text
        text = dbTile.transform.GetChild(4).GetComponent<TextMesh>();
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
        Vector3 avgVel = GlobalGrid[x, y].AverageVelocity / 4f;
        Vector3 pos = GlobalGrid[x, y].WorldPos;
        pos.z += 0.5f;
        Vector3 end = pos + avgVel;
        LineRenderer line = Db_LineGrid[x, y];
        line.enabled = true;
        line.SetPosition(0, pos);
        line.SetPosition(1, end);
    }
    void TurnOffLines(int x, int y)
    {
        LineRenderer line = Db_LineGrid[x, y];
        line.enabled = false;
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
                    else if (textType == DB_Text.CostFields)
                    {
                        CC_GridCell[,] group = GroupGridList[CCGroup];
                        //north = group[x, y].North.UnitCost.ToString("F2");
                        //south = group[x, y].South.UnitCost.ToString("F2");
                        //west = group[x, y].West.UnitCost.ToString("F2");
                        //east = group[x, y].East.UnitCost.ToString("F2");
                        debug = group[x, y].UnitCost.ToString("F2");
                    }
                    else if (textType == DB_Text.SpeedFields)
                    {
                        CC_GridCell[,] group = GroupGridList[CCGroup];
                        //north = group[x, y].North.Speed.ToString("F2");
                        //south = group[x, y].South.Speed.ToString("F2");
                        //west = group[x, y].West.Speed.ToString("F2");
                        //east = group[x, y].East.Speed.ToString("F2");
                        debug = group[x, y].Speed.ToString("F2");
                    }

                    if (lineType == DB_Line.AverageVelocity)
                        DrawTileVelocity(x, y);
                    else
                        TurnOffLines(x, y);

                    Db_TextGrid[x, y][0].text = debug;
                    Db_TextGrid[x, y][1].text = north;
                    Db_TextGrid[x, y][2].text = west;
                    Db_TextGrid[x, y][3].text = south;
                    Db_TextGrid[x, y][4].text = east;
                }
            }
        }
    }
}
