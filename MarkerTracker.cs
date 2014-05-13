using System;
using System.Collections.Generic;

public abstract class MarkerTracker : Tracker
{
    protected MarkerTracker()
    {
    }

    public abstract MarkerAbstractBehaviour CreateMarker(int markerID, string trackableName, float size);
    public abstract void DestroyAllMarkers(bool destroyGameObject);
    public abstract bool DestroyMarker(Marker marker, bool destroyGameObject);
    public abstract Marker GetMarkerByMarkerID(int markerID);
    public abstract IEnumerable<Marker> GetMarkers();
}

