using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    public Tile(int x, int y, Transform prefab = null)
    {
        X = x;
        Y = y;
        Prefab = prefab;
    }

    Transform Prefab = null;
    public int X;
    public int Y;
    public bool Wall = false;
    public uint Version = 0;
}
