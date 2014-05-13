using System;
using System.Runtime.CompilerServices;

public class MarkerImpl : TrackableImpl, Marker, Trackable
{
    private float mSize;

    public MarkerImpl(string name, int id, float size, int markerID) : base(name, id)
    {
        this.mSize = size;
        this.MarkerID = markerID;
    }

    public float GetSize()
    {
        return this.mSize;
    }

    public void SetSize(float size)
    {
        this.mSize = size;
        QCARWrapper.Instance.MarkerSetSize(base.ID, size);
    }

    public bool StartExtendedTracking()
    {
        return (QCARWrapper.Instance.StartExtendedTracking(IntPtr.Zero, base.ID) > 0);
    }

    public bool StopExtendedTracking()
    {
        return (QCARWrapper.Instance.StopExtendedTracking(IntPtr.Zero, base.ID) > 0);
    }

    public int MarkerID { get; private set; }
}

