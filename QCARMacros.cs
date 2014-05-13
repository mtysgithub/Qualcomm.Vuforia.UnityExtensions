using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size=1)]
internal struct QCARMacros
{
    public const string PLATFORM_DLL_IOS = "__Internal";
    public const string PLATFORM_DLL = "QCARWrapper";
}

