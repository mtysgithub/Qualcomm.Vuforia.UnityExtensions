using System;
using System.Runtime.InteropServices;

internal class CylinderTargetImpl : TrackableImpl, CylinderTarget, ExtendedTrackable, Trackable
{
    private float mBottomDiameter;
    private readonly DataSetImpl mDataSet;
    private float mSideLength;
    private float mTopDiameter;

    public CylinderTargetImpl(string name, int id, DataSet dataSet) : base(name, id)
    {
        this.mDataSet = (DataSetImpl) dataSet;
        float[] destination = new float[3];
        IntPtr dimensions = Marshal.AllocHGlobal((int) (3 * Marshal.SizeOf(typeof(float))));
        QCARWrapper.Instance.CylinderTargetGetSize(this.mDataSet.DataSetPtr, base.Name, dimensions);
        Marshal.Copy(dimensions, destination, 0, 3);
        Marshal.FreeHGlobal(dimensions);
        this.mSideLength = destination[0];
        this.mTopDiameter = destination[1];
        this.mBottomDiameter = destination[2];
    }

    public float GetBottomDiameter()
    {
        return this.mBottomDiameter;
    }

    public float GetSideLength()
    {
        return this.mSideLength;
    }

    public float GetTopDiameter()
    {
        return this.mTopDiameter;
    }

    private void ScaleCylinder(float scale)
    {
        this.mSideLength *= scale;
        this.mTopDiameter *= scale;
        this.mBottomDiameter *= scale;
    }

    public bool SetBottomDiameter(float bottomDiameter)
    {
        this.ScaleCylinder(bottomDiameter / this.mBottomDiameter);
        return (QCARWrapper.Instance.CylinderTargetSetBottomDiameter(this.mDataSet.DataSetPtr, base.Name, bottomDiameter) == 1);
    }

    public bool SetSideLength(float sideLength)
    {
        this.ScaleCylinder(sideLength / this.mSideLength);
        return (QCARWrapper.Instance.CylinderTargetSetSideLength(this.mDataSet.DataSetPtr, base.Name, sideLength) == 1);
    }

    public bool SetTopDiameter(float topDiameter)
    {
        this.ScaleCylinder(topDiameter / this.mTopDiameter);
        return (QCARWrapper.Instance.CylinderTargetSetTopDiameter(this.mDataSet.DataSetPtr, base.Name, topDiameter) == 1);
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

