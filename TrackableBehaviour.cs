using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackableBehaviour : MonoBehaviour, IEditorTrackableBehaviour
{
    [HideInInspector, SerializeField]
    protected bool mInitializedInEditor;
    [HideInInspector, SerializeField]
    protected bool mPreserveChildSize;
    [SerializeField, HideInInspector]
    protected Vector3 mPreviousScale = Vector3.zero;
    protected Status mStatus;
    protected Trackable mTrackable;
    private List<ITrackableEventHandler> mTrackableEventHandlers = new List<ITrackableEventHandler>();
    [HideInInspector, SerializeField]
    protected string mTrackableName = "";

    protected TrackableBehaviour()
    {
    }

    protected virtual bool CorrectScaleImpl()
    {
        return false;
    }

    bool IEditorTrackableBehaviour.CorrectScale()
    {
        return this.CorrectScaleImpl();
    }

    bool IEditorTrackableBehaviour.SetInitializedInEditor(bool initializedInEditor)
    {
        if (this.Trackable == null)
        {
            this.mInitializedInEditor = initializedInEditor;
            return true;
        }
        return false;
    }

    bool IEditorTrackableBehaviour.SetNameForTrackable(string name)
    {
        if (this.mTrackable == null)
        {
            this.mTrackableName = name;
            return true;
        }
        return false;
    }

    bool IEditorTrackableBehaviour.SetPreserveChildSize(bool preserveChildSize)
    {
        if (this.Trackable == null)
        {
            this.mPreserveChildSize = preserveChildSize;
            return true;
        }
        return false;
    }

    bool IEditorTrackableBehaviour.SetPreviousScale(Vector3 previousScale)
    {
        if (this.Trackable == null)
        {
            this.mPreviousScale = previousScale;
            return true;
        }
        return false;
    }

    void IEditorTrackableBehaviour.UnregisterTrackable()
    {
        this.InternalUnregisterTrackable();
    }

    protected abstract void InternalUnregisterTrackable();
    private void OnDisable()
    {
        Status mStatus = this.mStatus;
        this.mStatus = Status.NOT_FOUND;
        if (mStatus != Status.NOT_FOUND)
        {
            foreach (ITrackableEventHandler handler in this.mTrackableEventHandlers)
            {
                handler.OnTrackableStateChanged(mStatus, Status.NOT_FOUND);
            }
        }
    }

    public virtual void OnFrameIndexUpdate(int newFrameIndex)
    {
    }

    public void OnTrackerUpdate(Status newStatus)
    {
        Status mStatus = this.mStatus;
        this.mStatus = newStatus;
        if (mStatus != newStatus)
        {
            foreach (ITrackableEventHandler handler in this.mTrackableEventHandlers)
            {
                handler.OnTrackableStateChanged(mStatus, newStatus);
            }
        }
    }

    public void RegisterTrackableEventHandler(ITrackableEventHandler trackableEventHandler)
    {
        this.mTrackableEventHandlers.Add(trackableEventHandler);
        trackableEventHandler.OnTrackableStateChanged(Status.UNKNOWN, this.mStatus);
    }

    private void Start()
    {
        if ((KeepAliveAbstractBehaviour.Instance != null) && KeepAliveAbstractBehaviour.Instance.KeepTrackableBehavioursAlive)
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }
    }

    public bool UnregisterTrackableEventHandler(ITrackableEventHandler trackableEventHandler)
    {
        return this.mTrackableEventHandlers.Remove(trackableEventHandler);
    }

    public Status CurrentStatus
    {
        get
        {
            return this.mStatus;
        }
    }

    bool IEditorTrackableBehaviour.enabled
    {
        get
        {
            return base.enabled;
        }
        set
        {
            base.enabled = value;
        }
    }

    GameObject IEditorTrackableBehaviour.gameObject
    {
        get
        {
            return base.gameObject;
        }
    }

    bool IEditorTrackableBehaviour.InitializedInEditor
    {
        get
        {
            return this.mInitializedInEditor;
        }
    }

    bool IEditorTrackableBehaviour.PreserveChildSize
    {
        get
        {
            return this.mPreserveChildSize;
        }
    }

    Vector3 IEditorTrackableBehaviour.PreviousScale
    {
        get
        {
            return this.mPreviousScale;
        }
    }

    Renderer IEditorTrackableBehaviour.renderer
    {
        get
        {
            return base.renderer;
        }
    }

    Transform IEditorTrackableBehaviour.transform
    {
        get
        {
            return base.transform;
        }
    }

    public Trackable Trackable
    {
        get
        {
            return this.mTrackable;
        }
    }

    public string TrackableName
    {
        get
        {
            return this.mTrackableName;
        }
    }

    public enum Status
    {
        DETECTED = 2,
        EXTENDED_TRACKED = 4,
        NOT_FOUND = -1,
        TRACKED = 3,
        UNDEFINED = 1,
        UNKNOWN = 0
    }
}

