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

        Db_TextGrid = new TextMesh[Width, Height];
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
        Db_TextGrid[x, y] = text;

        // debug mesh (for coloring)
        MeshRenderer mesh = dbTile.GetComponent<MeshRenderer>();
        Db_MeshList.Add(mesh);
        Db_MeshGrid[x, y] = mesh;

        dbTile.transform.parent = holder.transform;

    }

    void Debug()
    {
        if (Utility.Instance.DebugInfo == false && debugOn == true)
        {
            foreach (MeshRenderer obj in Db_MeshList)
            {
                obj.enabled = false;
            }
            foreach (TextMesh obj in Db_TextList)
            {
                obj.transform.GetComponent<MeshRenderer>().enabled = false;
            }
            debugOn = false;
        }

        if (Utility.Instance.DebugInfo == true && debugOn == false)
        {
            foreach (MeshRenderer obj in Db_MeshList)
            {
                obj.enabled = true;
            }
            foreach (TextMesh obj in Db_TextList)
            {
                obj.transform.GetComponent<MeshRenderer>().enabled = true;
            }
            debugOn = true;
        }

        if (Utility.Instance.DebugInfo == true)
        {
            if (DensityText == true)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        float density = GlobalGrid[x, y].Density;
                        Vector2 avgVel = GlobalGrid[x, y].AverageVelocity;
                        string AV = avgVel.ToString("F2");
                        string den = density.ToString("F2");
                        Db_TextGrid[x, y].text = den;
                    }
                }
            }
        }
    }
}
