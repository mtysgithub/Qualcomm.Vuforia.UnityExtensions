using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack=1)]
public struct ImageTargetData
{
    public int id;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
    public Vector2 size;
}

