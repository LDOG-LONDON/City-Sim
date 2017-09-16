using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : Singleton<Utility> {

    protected Utility() {}

    public uint Width;
    public uint Height;

    public float TileWidth;
    public float TileHeight;

    public Vector3 MapPosition;

    public bool DebugInfo = true;


    public Vector3 CoordToVec3(int x, int y)
    {
        float X = x * (TileWidth) + MapPosition.x;
        float Y = y * (TileHeight) + MapPosition.y;
        float Z = MapPosition.z;
        return new Vector3(X, Y, Z);

        //return new Vector3((-(Width - 1) * TileWidth / 2f) + (TileWidth) * x + MapPosition.x,
        //                   (-(Height - 1) * TileHeight / 2f) + (TileHeight) * y + MapPosition.y,
        //                   MapPosition.z);
    }

    public Vector3 CoordToVec3(float x, float y)
    {
        float X = x * (TileWidth) + MapPosition.x;
        float Y = y * (TileHeight) + MapPosition.y;
        float Z = MapPosition.z;
        return new Vector3(X, Y, Z);
        //return new Vector3((-(Width - 1) * TileWidth / 2f) + (TileWidth) * x + MapPosition.x,
        //                   (-(Height - 1) * TileHeight / 2f) + (TileHeight) * y + MapPosition.y,
        //                   MapPosition.z);
    }

    public Vector3 Vec3ToCoord(Vector3 pos)
    {
        /*
        float X = ((Mathf.Round(pos.x * 100.0f) / 100.0f) + ((Width + 1) * TileWidth / 2f) - MapPosition.x) / TileWidth;
        float Y = ((Mathf.Round(pos.y * 100.0f) / 100.0f) + ((Height + 1) * TileHeight / 2f) - MapPosition.y) / TileHeight;
        float Z = MapPosition.z;
        X = Mathf.Clamp(X, 0, Width - 1);
        Y = Mathf.Clamp(Y, 0, Height - 1);
        return new Vector3(X, Y, Z);
        */
        pos = pos - MapPosition;

        //float X = pos.x + ((Width / 2.0f) - (TileWidth / 2.0f));
        //float Y = pos.y + ((Height / 2.0f) - (TileHeight / 2.0f));
        //float Z = MapPosition.z;
        float X = pos.x / (TileWidth) + 0.5f;
        float Y = pos.y / (TileHeight) + 0.5f;
        float Z = MapPosition.z;

        X = Mathf.Clamp(X, 0, Width-1);
        Y = Mathf.Clamp(Y, 0, Height-1);
        return new Vector3(X, Y, Z);
    }
}
