using System;

public interface ITrackerEventHandler
{
    void OnInitialized();
    void OnTrackablesUpdated();
}

