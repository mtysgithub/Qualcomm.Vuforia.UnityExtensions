using System;

public abstract class TrackerManager
{
    private static TrackerManager mInstance;

    protected TrackerManager()
    {
    }

    public abstract bool DeinitTracker<T>() where T: Tracker;
    public abstract StateManager GetStateManager();
    public abstract T GetTracker<T>() where T: Tracker;
    public abstract T InitTracker<T>() where T: Tracker;

    public static TrackerManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                lock (typeof(TrackerManager))
                {
                    if (mInstance == null)
                    {
                        mInstance = new TrackerManagerImpl();
                    }
                }
            }
            return mInstance;
        }
    }
}

