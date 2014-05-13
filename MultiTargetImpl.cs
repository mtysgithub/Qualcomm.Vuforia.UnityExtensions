using System;

internal class MultiTargetImpl : TrackableImpl, MultiTarget, ExtendedTrackable, Trackable
{
    private readonly DataSetImpl mDataSet;

    public MultiTargetImpl(string name, int id, DataSet dataSet) : base(name, id)
    {
        this.mDataSet = (DataSetImpl) dataSet;
    }

    public bool StartExtendedTracking()
    {
        return (QCARWrapper.Instance.StartExtendedTracking(this.mDataSet.DataSetPtr, base.ID) > 0);
    }

    public bool StopExtendedTracking()
    {
        return (QCARWrapper.Instance.StopExtendedTracking(this.mDataSet.DataSetPtr, base.ID) > 0);
    }
}

