using System;
using UnityEngine;

public abstract class DataSetTrackableBehaviour : TrackableBehaviour, IEditorDataSetTrackableBehaviour, IEditorTrackableBehaviour
{
    [SerializeField, HideInInspector]
    protected string mDataSetPath = "";
    [HideInInspector, SerializeField]
    protected bool mExtendedTracking;

    protected DataSetTrackableBehaviour()
    {
    }

    bool IEditorDataSetTrackableBehaviour.SetDataSetPath(string dataSetPath)
    {
        if (base.mTrackable == null)
        {
            this.mDataSetPath = dataSetPath;
            return true;
        }
        return false;
    }

    void IEditorDataSetTrackableBehaviour.SetExtendedTracking(bool extendedTracking)
    {
        this.mExtendedTracking = extendedTracking;
    }

    string IEditorDataSetTrackableBehaviour.DataSetName
    {
        get
        {
            string str = QCARRuntimeUtilities.StripFileNameFromPath(this.mDataSetPath);
            int length = QCARRuntimeUtilities.StripExtensionFromPath(this.mDataSetPath).Length;
            if (length > 0)
            {
                length++;
                return str.Remove(str.Length - length);
            }
            return str;
        }
    }

    string IEditorDataSetTrackableBehaviour.DataSetPath
    {
        get
        {
            return this.mDataSetPath;
        }
    }

    bool IEditorDataSetTrackableBehaviour.ExtendedTracking
    {
        get
        {
            return this.mExtendedTracking;
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
}

