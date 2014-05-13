using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack=1)]
public struct RectangleData
{
    public float leftTopX;
    public float leftTopY;
    public float rightBottomX;
    public float rightBottomY;
}

