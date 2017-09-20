using System.Collections;
using System;
using UnityEngine;

public class SerializableVector2 {

    public float x;
    public float y;

    public SerializableVector2(float rx, float ry)
    {
        x = rx;
        y = ry;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}]", x, y);
    }

    public static implicit operator Vector2(SerializableVector2 rValue)
    {
        return new Vector2(rValue.x, rValue.y);
    }

    public static implicit operator SerializableVector2(Vector2 rValue)
    {
        return new SerializableVector2(rValue.x, rValue.y);
    }
}
