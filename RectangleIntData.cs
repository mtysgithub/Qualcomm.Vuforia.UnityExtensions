using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack=1)]
public struct RectangleIntData
{
    public int leftTopX;
    public int leftTopY;
    public int rightBottomX;
    public int rightBottomY;
}

