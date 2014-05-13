using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataSetLoadAbstractBehaviour : MonoBehaviour, ITrackerEventHandler
{
    [HideInInspector, SerializeField]
    public List<string> mDataSetsToActivate = new List<string>();
    [SerializeField, HideInInspector]
    public List<string> mDataSetsToLoad = new List<string>();

    protected DataSetLoadAbstractBehaviour()
    {
    }

    private void OnDestroy()
    {
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour != null)
        {
            behaviour.UnregisterTrackerEventHandler(this);
        }
    }

    public void OnInitialized()
    {
        if (QCARRuntimeUtilities.IsQCAREnabled())
        {
            foreach (string str in this.mDataSetsToLoad)
            {
                if (!DataSet.Exists(str))
                {
                    Debug.LogError("Data set " + str + " does not exist.");
                }
                else
                {
                    ImageTracker tracker = TrackerManager.Instance.GetTracker<ImageTracker>();
                    DataSet dataSet = tracker.CreateDataSet();
                    if (!dataSet.Load(str))
                    {
                        Debug.LogError("Failed to load data set " + str + ".");
                    }
                    else if (this.mDataSetsToActivate.Contains(str))
                    {
                        tracker.ActivateDataSet(dataSet);
                    }
                }
            }
        }
    }

    public void OnTrackablesUpdated()
    {
    }

    private void Start()
    {
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour != null)
        {
            behaviour.RegisterTrackerEventHandler(this, true);
        }
    }
}

