using System;
using UnityEngine;

public abstract class MarkerAbstractBehaviour : TrackableBehaviour, IEditorMarkerBehaviour, IEditorTrackableBehaviour
{
    private Marker mMarker;
    [SerializeField, HideInInspector]
    private int mMarkerID = -1;

    protected override bool CorrectScaleImpl()
    {
        for (int i = 0; i < 3; i++)
        {
            if (base.transform.localScale[i] != this.mPreviousScale[i])
            {
                base.transform.localScale = new Vector3(base.transform.localScale[i], base.transform.localScale[i], base.transform.localScale[i]);
                base.mPreviousScale = base.transform.localScale;
                return true;
            }
        }
        return false;
    }

    void IEditorMarkerBehaviour.InitializeMarker(Marker marker)
    {
        base.mTrackable = this.mMarker = marker;
    }

    bool IEditorMarkerBehaviour.SetMarkerID(int markerID)
    {
        if (base.mTrackable == null)
        {
            this.mMarkerID = markerID;
            return true;
        }
        return false;
    }

    protected override void InternalUnregisterTrackable()
    {
        base.mTrackable = (Trackable) (this.mMarker = null);
    }

    int IEditorMarkerBehaviour.MarkerID
    {
        get
        {
            return this.mMarkerID;
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

    public Marker Marker
    {
        get
        {
            return this.mMarker;
        }
    }
}

