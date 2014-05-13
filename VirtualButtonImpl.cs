using System;
using System.Runtime.InteropServices;
using UnityEngine;

internal class VirtualButtonImpl : VirtualButton
{
    private RectangleData mArea;
    private int mID;
    private bool mIsEnabled;
    private string mName;
    private DataSetImpl mParentDataSet;
    private ImageTarget mParentImageTarget;

    public VirtualButtonImpl(string name, int id, RectangleData area, ImageTarget imageTarget, DataSet dataSet)
    {
        this.mName = name;
        this.mID = id;
        this.mArea = area;
        this.mIsEnabled = true;
        this.mParentImageTarget = imageTarget;
        this.mParentDataSet = (DataSetImpl) dataSet;
    }

    public override bool SetArea(RectangleData area)
    {
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RectangleData)));
        Marshal.StructureToPtr(area, ptr, false);
        int num = QCARWrapper.Instance.VirtualButtonSetAreaRectangle(this.mParentDataSet.DataSetPtr, this.mParentImageTarget.Name, this.Name, ptr);
        Marshal.FreeHGlobal(ptr);
        if (num == 0)
        {
            Debug.LogError("Virtual Button area rectangle could not be set.");
            return false;
        }
        return true;
    }

    public override bool SetEnabled(bool enabled)
    {
        if (QCARWrapper.Instance.VirtualButtonSetEnabled(this.mParentDataSet.DataSetPtr, this.mParentImageTarget.Name, this.mName, enabled ? 1 : 0) == 0)
        {
            Debug.LogError("Virtual Button enabled value could not be set.");
            return false;
        }
        this.mIsEnabled = enabled;
        return true;
    }

    public override bool SetSensitivity(VirtualButton.Sensitivity sensitivity)
    {
        if (QCARWrapper.Instance.VirtualButtonSetSensitivity(this.mParentDataSet.DataSetPtr, this.mParentImageTarget.Name, this.mName, (int) sensitivity) == 0)
        {
            Debug.LogError("Virtual Button sensitivity could not be set.");
            return false;
        }
        return true;
    }

    public override RectangleData Area
    {
        get
        {
            return this.mArea;
        }
    }

    public override bool Enabled
    {
        get
        {
            return this.mIsEnabled;
        }
    }

    public override int ID
    {
        get
        {
            return this.mID;
        }
    }

    public override string Name
    {
        get
        {
            return this.mName;
        }
    }
}

