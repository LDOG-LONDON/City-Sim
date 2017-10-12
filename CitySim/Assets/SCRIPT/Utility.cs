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
    }

    public Vector3 CoordToVec3(float x, float y)
    {
        float X = x * (TileWidth) + MapPosition.x;
        float Y = y * (TileHeight) + MapPosition.y;
        float Z = MapPosition.z;
        return new Vector3(X, Y, Z);
    }

    public Vector3 Vec3ToCoord(Vector3 pos)
    {
        pos = pos - MapPosition;
        float X = pos.x / (TileWidth) + 0.5f;
        float Y = pos.y / (TileHeight) + 0.5f;
        float Z = MapPosition.z;

        X = Mathf.Clamp(X, 0, Width-1);
        Y = Mathf.Clamp(Y, 0, Height-1);
        return new Vector3(X, Y, Z);
    }

    public bool Vec2ToCoordExists(int X, int Y)
    {
        if (X < 0 || X > Width - 1)
            return false;
        if (Y < 0 || Y > Height - 1)
            return false;
        return true;
    }

    public bool Vec2ToCoordExists(Vector2 pos, out Vector2 coord)
    {
        int X = (int)(pos.x / (TileWidth) + 0.5f);
        int Y = (int)(pos.y / (TileHeight) + 0.5f);

        coord = new Vector2(X, Y);

        if (X < 0 || X > Width - 1)
            return false;
        if (Y < 0 || Y > Height - 1)
            return false;
        return true;
    }
}
