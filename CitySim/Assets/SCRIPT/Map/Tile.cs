using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    public Tile(int x, int y, Vector3 worldPos, Transform prefab = null)
    {
        X = x;
        Y = y;
        Prefab = prefab;
    }

    public enum searchType
    {
        NONE,
        OPEN,
        CLOSED
    }

    public searchType type = searchType.NONE;
    public Tile Parent;
    public Transform Prefab = null;
    public Vector3 Pos;
    public int X;
    public int Y;
    public bool Wall = false;
    public int searchIteration = 0;
    public float Cost;
    public float Given;
}
