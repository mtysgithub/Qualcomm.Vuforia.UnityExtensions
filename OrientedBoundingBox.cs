using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct OrientedBoundingBox
{
    public OrientedBoundingBox(Vector2 center, Vector2 halfExtents, float rotation)
    {
        this = new OrientedBoundingBox();
        this.Center = center;
        this.HalfExtents = halfExtents;
        this.Rotation = rotation;
    }

    public Vector2 Center { get; private set; }
    public Vector2 HalfExtents { get; private set; }
    public float Rotation { get; private set; }
}

