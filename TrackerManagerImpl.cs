using System;
using UnityEngine;

public class TrackerManagerImpl : TrackerManager
{
    private ImageTracker mImageTracker;
    private MarkerTracker mMarkerTracker;
    private StateManager mStateManager = new StateManagerImpl();
    private TextTracker mTextTracker;

    public override bool DeinitTracker<T>() /*where T: Tracker*/
    {
        if (QCARWrapper.Instance.TrackerManagerDeinitTracker(TypeMapping.GetTypeID(typeof(T))) == 0)
        {
            Debug.LogError("Could not deinitialize the tracker.");
            return false;
        }
        if (typeof(T) == typeof(ImageTracker))
        {
            this.mImageTracker = null;
        }
        else if (typeof(T) == typeof(MarkerTracker))
        {
            this.mMarkerTracker = null;
        }
        else if (typeof(T) == typeof(TextTracker))
        {
            this.mTextTracker = null;
        }
        else
        {
            Debug.LogError("Could not deinitialize tracker. Unknown tracker type.");
            return false;
        }
        return true;
    }

    public override StateManager GetStateManager()
    {
        return this.mStateManager;
    }

    public override T GetTracker<T>() /*where T: Tracker*/
    {
        if (typeof(T) == typeof(ImageTracker))
        {
            return (this.mImageTracker as T);
        }
        if (typeof(T) == typeof(MarkerTracker))
        {
            return (this.mMarkerTracker as T);
        }
        if (typeof(T) == typeof(TextTracker))
        {
            return (this.mTextTracker as T);
        }
        Debug.LogError("Could not return tracker. Unknow tracker type.");
        return default(T);
    }

    public override T InitTracker<T>() /*where T: Tracker*/
    {
        if (QCARRuntimeUtilities.IsQCAREnabled())
        {
            if (QCARWrapper.Instance.TrackerManagerInitTracker(TypeMapping.GetTypeID(typeof(T))) == 0)
            {
                Debug.LogError("Could not initialize the tracker.");
                return default(T);
            }
            if (typeof(T) == typeof(ImageTracker))
            {
                if (this.mImageTracker == null)
                {
                    this.mImageTracker = new ImageTrackerImpl();
                }
                return (this.mImageTracker as T);
            }
            if (typeof(T) == typeof(MarkerTracker))
            {
                if (this.mMarkerTracker == null)
                {
                    this.mMarkerTracker = new MarkerTrackerImpl();
                }
                return (this.mMarkerTracker as T);
            }
            if (typeof(T) == typeof(TextTracker))
            {
                if (this.mTextTracker == null)
                {
                    this.mTextTracker = new TextTrackerImpl();
                }
                return (this.mTextTracker as T);
            }
            Debug.LogError("Could not initialize tracker. Unknown tracker type.");
        }
        return default(T);
    }
}

