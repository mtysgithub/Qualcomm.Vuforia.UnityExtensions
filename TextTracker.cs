using System;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class TextTracker : Tracker
{
    protected TextTracker()
    {
    }

    public abstract bool GetRegionOfInterest(out Rect detectionRegion, out Rect trackingRegion);
    public abstract bool SetRegionOfInterest(Rect detectionRegion, Rect trackingRegion);

    public abstract WordList WordList { get; }
}

