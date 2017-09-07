using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : Singleton<Utility> {

    protected Utility() {}

    public uint Width;
    public uint Height;

    public float TileWidth;
    public float TileHeight;

    public Vector3 CoordToVec3(int x, int y)
    {
        return new Vector3((-(Width - 1) * TileWidth / 2f) + (TileWidth) * x + transform.position.x,
                           (-(Height - 1) * TileHeight / 2f) + (TileHeight) * y + transform.position.y,
                           transform.position.z - 1f);
    }

    public Vector3 Vec3ToCoord(Vector3 pos)
    {
        float X = ((Mathf.Round(pos.x * 100.0f) / 100.0f) + ((Width + 1) * TileWidth / 2f) - transform.position.x) / TileWidth;
        float Y = ((Mathf.Round(pos.y * 100.0f) / 100.0f) + ((Height + 1) * TileHeight / 2f) - transform.position.y) / TileHeight;
        float Z = transform.position.z + 1f;
        return new Vector3(X, Y, Z);
    }
}
