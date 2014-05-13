using System;

public interface IEditorMarkerBehaviour : IEditorTrackableBehaviour
{
    void InitializeMarker(Marker marker);
    bool SetMarkerID(int markerID);

    int MarkerID { get; }
}

