using System;

public interface ITrackableEventHandler
{
    void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus);
}

