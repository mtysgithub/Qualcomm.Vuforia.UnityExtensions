using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public abstract class StateManager
{
    protected StateManager()
    {
    }

    public abstract void DestroyTrackableBehavioursForTrackable(Trackable trackable, [Optional, DefaultParameterValue(true)] bool destroyGameObjects);
    public abstract IEnumerable<TrackableBehaviour> GetActiveTrackableBehaviours();
    public abstract IEnumerable<TrackableBehaviour> GetTrackableBehaviours();
    public abstract WordManager GetWordManager();
}

