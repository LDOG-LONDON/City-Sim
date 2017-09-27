using System;
using System.Collections;
using UnityEngine;

public struct SerializableVector3 {

    public float x;
    public float y;
    public float z;

    public SerializableVector3(float rx, float ry, float rz)
    {
        x = rx;
        y = ry;
        z = rz;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }

    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}
