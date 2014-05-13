using System;
using System.Collections.Generic;

public abstract class ImageTracker : Tracker
{
    protected ImageTracker()
    {
    }

    public abstract bool ActivateDataSet(DataSet dataSet);
    public abstract DataSet CreateDataSet();
    public abstract bool DeactivateDataSet(DataSet dataSet);
    public abstract void DestroyAllDataSets(bool destroyTrackables);
    public abstract bool DestroyDataSet(DataSet dataSet, bool destroyTrackables);
    public abstract IEnumerable<DataSet> GetActiveDataSets();
    public abstract IEnumerable<DataSet> GetDataSets();
    public abstract bool PersistExtendedTracking(bool on);
    public abstract bool ResetExtendedTracking();

    public abstract ImageTargetBuilder ImageTargetBuilder { get; }

    public abstract TargetFinder TargetFinder { get; }
}

