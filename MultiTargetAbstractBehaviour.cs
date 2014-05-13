using System;
using UnityEngine;

public abstract class MultiTargetAbstractBehaviour : DataSetTrackableBehaviour, IEditorMultiTargetBehaviour, IEditorDataSetTrackableBehaviour, IEditorTrackableBehaviour
{
    private MultiTarget mMultiTarget;

    protected MultiTargetAbstractBehaviour()
    {
    }

    void IEditorMultiTargetBehaviour.InitializeMultiTarget(MultiTarget multiTarget)
    {
        base.mTrackable = this.mMultiTarget = multiTarget;
        if (base.mExtendedTracking)
        {
            this.mMultiTarget.StartExtendedTracking();
        }
    }

    protected override void InternalUnregisterTrackable()
    {
        base.mTrackable = (Trackable) (this.mMultiTarget = null);
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

    public MultiTarget MultiTarget
    {
        get
        {
            return this.mMultiTarget;
        }
    }
}

