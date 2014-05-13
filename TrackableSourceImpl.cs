using System;
using System.Runtime.CompilerServices;

public class TrackableSourceImpl : TrackableSource
{
    public TrackableSourceImpl(IntPtr trackableSourcePtr)
    {
        this.TrackableSourcePtr = trackableSourcePtr;
    }

    public IntPtr TrackableSourcePtr { get; private set; }
}

