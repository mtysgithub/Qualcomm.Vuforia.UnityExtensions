using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class ImageTrackerImpl : ImageTracker
{
    private List<DataSetImpl> mActiveDataSets = new List<DataSetImpl>();
    private List<DataSet> mDataSets = new List<DataSet>();
    private ImageTargetBuilder mImageTargetBuilder = new ImageTargetBuilderImpl();
    private TargetFinder mTargetFinder = new TargetFinderImpl();

    public override bool ActivateDataSet(DataSet dataSet)
    {
        if (dataSet == null)
        {
            Debug.LogError("Dataset is null.");
            return false;
        }
        DataSetImpl item = (DataSetImpl) dataSet;
        if (QCARWrapper.Instance.ImageTrackerActivateDataSet(item.DataSetPtr) == 0)
        {
            Debug.LogError("Could not activate dataset.");
            return false;
        }
        StateManagerImpl stateManager = (StateManagerImpl) TrackerManager.Instance.GetStateManager();
        foreach (Trackable trackable in item.GetTrackables())
        {
            stateManager.EnableTrackableBehavioursForTrackable(trackable, true);
        }
        this.mActiveDataSets.Add(item);
        return true;
    }

    public override DataSet CreateDataSet()
    {
        IntPtr dataSetPtr = QCARWrapper.Instance.ImageTrackerCreateDataSet();
        if (dataSetPtr == IntPtr.Zero)
        {
            Debug.LogError("Could not create dataset.");
            return null;
        }
        DataSet item = new DataSetImpl(dataSetPtr);
        this.mDataSets.Add(item);
        return item;
    }

    public override bool DeactivateDataSet(DataSet dataSet)
    {
        if (dataSet == null)
        {
            Debug.LogError("Dataset is null.");
            return false;
        }
        DataSetImpl item = (DataSetImpl) dataSet;
        if (QCARWrapper.Instance.ImageTrackerDeactivateDataSet(item.DataSetPtr) == 0)
        {
            Debug.LogError("Could not deactivate dataset.");
            return false;
        }
        StateManagerImpl stateManager = (StateManagerImpl) TrackerManager.Instance.GetStateManager();
        foreach (Trackable trackable in dataSet.GetTrackables())
        {
            stateManager.EnableTrackableBehavioursForTrackable(trackable, false);
        }
        this.mActiveDataSets.Remove(item);
        return true;
    }

    public override void DestroyAllDataSets(bool destroyTrackables)
    {
        List<DataSetImpl> list = new List<DataSetImpl>(this.mActiveDataSets);
        foreach (DataSetImpl impl in list)
        {
            this.DeactivateDataSet(impl);
        }
        for (int i = this.mDataSets.Count - 1; i >= 0; i--)
        {
            this.DestroyDataSet(this.mDataSets[i], destroyTrackables);
        }
        this.mDataSets.Clear();
    }

    public override bool DestroyDataSet(DataSet dataSet, bool destroyTrackables)
    {
        if (dataSet == null)
        {
            Debug.LogError("Dataset is null.");
            return false;
        }
        if (destroyTrackables)
        {
            dataSet.DestroyAllTrackables(true);
        }
        DataSetImpl impl = (DataSetImpl) dataSet;
        if (QCARWrapper.Instance.ImageTrackerDestroyDataSet(impl.DataSetPtr) == 0)
        {
            Debug.LogError("Could not destroy dataset.");
            return false;
        }
        this.mDataSets.Remove(dataSet);
        return true;
    }

    public override IEnumerable<DataSet> GetActiveDataSets()
    {
        return this.mActiveDataSets.Cast<DataSet>();
    }

    public override IEnumerable<DataSet> GetDataSets()
    {
        return this.mDataSets;
    }

    public override bool PersistExtendedTracking(bool on)
    {
        if (QCARWrapper.Instance.ImageTrackerPersistExtendedTracking(on ? 1 : 0) == 0)
        {
            Debug.LogError("Could not set persist extended tracking.");
            return false;
        }
        return true;
    }

    public override bool ResetExtendedTracking()
    {
        if (QCARWrapper.Instance.ImageTrackerResetExtendedTracking() == 0)
        {
            Debug.LogError("Could not reset extended tracking.");
            return false;
        }
        return true;
    }

    public override bool Start()
    {
        if (QCARWrapper.Instance.ImageTrackerStart() == 0)
        {
            Debug.LogError("Could not start tracker.");
            return false;
        }
        return true;
    }

    public override void Stop()
    {
        QCARWrapper.Instance.ImageTrackerStop();
        StateManagerImpl stateManager = (StateManagerImpl) TrackerManager.Instance.GetStateManager();
        foreach (DataSetImpl impl2 in this.mActiveDataSets)
        {
            foreach (Trackable trackable in impl2.GetTrackables())
            {
                stateManager.SetTrackableBehavioursForTrackableToNotFound(trackable);
            }
        }
    }

    public override ImageTargetBuilder ImageTargetBuilder
    {
        get
        {
            return this.mImageTargetBuilder;
        }
    }

    public override TargetFinder TargetFinder
    {
        get
        {
            return this.mTargetFinder;
        }
    }
}

