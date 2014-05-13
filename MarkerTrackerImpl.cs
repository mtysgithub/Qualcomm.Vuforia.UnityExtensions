using System;
using System.Collections.Generic;
using UnityEngine;

internal class MarkerTrackerImpl : MarkerTracker
{
    private readonly Dictionary<int, Marker> mMarkerDict = new Dictionary<int, Marker>();

    public override MarkerAbstractBehaviour CreateMarker(int markerID, string trackableName, float size)
    {
        int id = this.RegisterMarker(markerID, trackableName, size);
        if (id == -1)
        {
            Debug.LogError("Could not create marker with id " + markerID + ".");
            return null;
        }
        Marker trackable = new MarkerImpl(trackableName, id, size, markerID);
        this.mMarkerDict[id] = trackable;
        StateManagerImpl stateManager = (StateManagerImpl) TrackerManager.Instance.GetStateManager();
        return stateManager.CreateNewMarkerBehaviourForMarker(trackable, trackableName);
    }

    public override void DestroyAllMarkers(bool destroyGameObject)
    {
        List<Marker> list = new List<Marker>(this.mMarkerDict.Values);
        foreach (Marker marker in list)
        {
            this.DestroyMarker(marker, destroyGameObject);
        }
    }

    public override bool DestroyMarker(Marker marker, bool destroyGameObject)
    {
        if (QCARWrapper.Instance.MarkerTrackerDestroyMarker(marker.ID) == 0)
        {
            Debug.LogError("Could not destroy marker with id " + marker.MarkerID + ".");
            return false;
        }
        this.mMarkerDict.Remove(marker.ID);
        if (destroyGameObject)
        {
            TrackerManager.Instance.GetStateManager().DestroyTrackableBehavioursForTrackable(marker, true);
        }
        return true;
    }

    public override Marker GetMarkerByMarkerID(int markerID)
    {
        foreach (Marker marker in this.mMarkerDict.Values)
        {
            if (marker.MarkerID == markerID)
            {
                return marker;
            }
        }
        return null;
    }

    public override IEnumerable<Marker> GetMarkers()
    {
        return this.mMarkerDict.Values;
    }

    public Marker InternalCreateMarker(int markerID, string name, float size)
    {
        int key = this.RegisterMarker(markerID, name, size);
        if (key == -1)
        {
            Debug.LogWarning("Marker named " + name + " could not be created");
            return null;
        }
        if (!this.mMarkerDict.ContainsKey(key))
        {
            Marker marker = new MarkerImpl(name, key, size, markerID);
            this.mMarkerDict[key] = marker;
            Debug.Log(string.Concat(new object[] { "Created Marker named ", marker.Name, " with id ", marker.ID }));
        }
        return this.mMarkerDict[key];
    }

    private int RegisterMarker(int markerID, string trackableName, float size)
    {
        return QCARWrapper.Instance.MarkerTrackerCreateMarker(markerID, trackableName, size);
    }

    public override bool Start()
    {
        if (QCARWrapper.Instance.MarkerTrackerStart() == 0)
        {
            Debug.LogError("Could not start tracker.");
            return false;
        }
        return true;
    }

    public override void Stop()
    {
        QCARWrapper.Instance.MarkerTrackerStop();
        StateManagerImpl stateManager = (StateManagerImpl) TrackerManager.Instance.GetStateManager();
        foreach (Marker marker in this.mMarkerDict.Values)
        {
            stateManager.SetTrackableBehavioursForTrackableToNotFound(marker);
        }
    }
}

