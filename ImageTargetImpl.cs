using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

internal class ImageTargetImpl : TrackableImpl, ImageTarget, ExtendedTrackable, Trackable
{
    private readonly DataSetImpl mDataSet;
    private readonly ImageTargetType mImageTargetType;
    private Vector2 mSize;
    private readonly Dictionary<int, VirtualButton> mVirtualButtons;

    public ImageTargetImpl(string name, int id, ImageTargetType imageTargetType, DataSet dataSet) : base(name, id)
    {
        this.mImageTargetType = imageTargetType;
        this.mDataSet = (DataSetImpl) dataSet;
        IntPtr size = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Vector2)));
        QCARWrapper.Instance.ImageTargetGetSize(this.mDataSet.DataSetPtr, base.Name, size);
        this.mSize = (Vector2) Marshal.PtrToStructure(size, typeof(Vector2));
        Marshal.FreeHGlobal(size);
        this.mVirtualButtons = new Dictionary<int, VirtualButton>();
        this.CreateVirtualButtonsFromNative();
    }

    private VirtualButton CreateNewVirtualButtonInNative(string name, RectangleData rectangleData)
    {
        if (this.ImageTargetType != ImageTargetType.PREDEFINED)
        {
            Debug.LogError("DataSet.RegisterVirtualButton: virtual button '" + name + "' cannot be registered for a user defined target.");
            return null;
        }
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RectangleData)));
        Marshal.StructureToPtr(rectangleData, ptr, false);
        bool flag = QCARWrapper.Instance.ImageTargetCreateVirtualButton(this.mDataSet.DataSetPtr, base.Name, name, ptr) != 0;
        VirtualButton button = null;
        if (!flag)
        {
            return button;
        }
        int key = QCARWrapper.Instance.VirtualButtonGetId(this.mDataSet.DataSetPtr, base.Name, name);
        if (!this.mVirtualButtons.ContainsKey(key))
        {
            button = new VirtualButtonImpl(name, key, rectangleData, this, this.mDataSet);
            this.mVirtualButtons.Add(key, button);
            return button;
        }
        return this.mVirtualButtons[key];
    }

    public VirtualButton CreateVirtualButton(string name, RectangleData area)
    {
        VirtualButton button = this.CreateNewVirtualButtonInNative(name, area);
        if (button == null)
        {
            Debug.LogError("Could not create Virtual Button.");
            return button;
        }
        Debug.Log("Created Virtual Button successfully.");
        return button;
    }

    private void CreateVirtualButtonsFromNative()
    {
        int virtualButtonDataArrayLength = QCARWrapper.Instance.ImageTargetGetNumVirtualButtons(this.mDataSet.DataSetPtr, base.Name);
        if (virtualButtonDataArrayLength > 0)
        {
            IntPtr virtualButtonDataArray = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(QCARManagerImpl.VirtualButtonData)) * virtualButtonDataArrayLength));
            IntPtr rectangleDataArray = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(RectangleData)) * virtualButtonDataArrayLength));
            QCARWrapper.Instance.ImageTargetGetVirtualButtons(virtualButtonDataArray, rectangleDataArray, virtualButtonDataArrayLength, this.mDataSet.DataSetPtr, base.Name);
            for (int i = 0; i < virtualButtonDataArrayLength; i++)
            {
                IntPtr ptr = new IntPtr(virtualButtonDataArray.ToInt32() + (i * Marshal.SizeOf(typeof(QCARManagerImpl.VirtualButtonData))));
                QCARManagerImpl.VirtualButtonData data = (QCARManagerImpl.VirtualButtonData) Marshal.PtrToStructure(ptr, typeof(QCARManagerImpl.VirtualButtonData));
                if (!this.mVirtualButtons.ContainsKey(data.id))
                {
                    IntPtr ptr4 = new IntPtr(rectangleDataArray.ToInt32() + (i * Marshal.SizeOf(typeof(RectangleData))));
                    RectangleData area = (RectangleData) Marshal.PtrToStructure(ptr4, typeof(RectangleData));
                    int capacity = 0x80;
                    StringBuilder vbName = new StringBuilder(capacity);
                    if (QCARWrapper.Instance.ImageTargetGetVirtualButtonName(this.mDataSet.DataSetPtr, base.Name, i, vbName, capacity) == 0)
                    {
                        Debug.LogError("Failed to get virtual button name.");
                    }
                    else
                    {
                        VirtualButton button = new VirtualButtonImpl(vbName.ToString(), data.id, area, this, this.mDataSet);
                        this.mVirtualButtons.Add(data.id, button);
                    }
                }
            }
            Marshal.FreeHGlobal(virtualButtonDataArray);
            Marshal.FreeHGlobal(rectangleDataArray);
        }
    }

    public bool DestroyVirtualButton(VirtualButton vb)
    {
        bool flag = false;
        ImageTracker tracker = TrackerManager.Instance.GetTracker<ImageTracker>();
        if (tracker != null)
        {
            bool flag2 = false;
            foreach (DataSet set in tracker.GetActiveDataSets())
            {
                if (this.mDataSet == set)
                {
                    flag2 = true;
                }
            }
            if (flag2)
            {
                tracker.DeactivateDataSet(this.mDataSet);
            }
            if (this.UnregisterVirtualButtonInNative(vb))
            {
                Debug.Log("Unregistering virtual button successfully");
                flag = true;
                this.mVirtualButtons.Remove(vb.ID);
            }
            else
            {
                Debug.LogError("Failed to unregister virtual button.");
            }
            if (flag2)
            {
                tracker.ActivateDataSet(this.mDataSet);
            }
        }
        return flag;
    }

    public Vector2 GetSize()
    {
        return this.mSize;
    }

    public VirtualButton GetVirtualButtonByName(string name)
    {
        foreach (VirtualButton button in this.mVirtualButtons.Values)
        {
            if (button.Name == name)
            {
                return button;
            }
        }
        return null;
    }

    public IEnumerable<VirtualButton> GetVirtualButtons()
    {
        return this.mVirtualButtons.Values;
    }

    public void SetSize(Vector2 size)
    {
        this.mSize = size;
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Vector2)));
        Marshal.StructureToPtr(size, ptr, false);
        QCARWrapper.Instance.ImageTargetSetSize(this.mDataSet.DataSetPtr, base.Name, ptr);
        Marshal.FreeHGlobal(ptr);
    }

    public bool StartExtendedTracking()
    {
        return (QCARWrapper.Instance.StartExtendedTracking(this.mDataSet.DataSetPtr, base.ID) > 0);
    }

    public bool StopExtendedTracking()
    {
        return (QCARWrapper.Instance.StopExtendedTracking(this.mDataSet.DataSetPtr, base.ID) > 0);
    }

    private bool UnregisterVirtualButtonInNative(VirtualButton vb)
    {
        int key = QCARWrapper.Instance.VirtualButtonGetId(this.mDataSet.DataSetPtr, base.Name, vb.Name);
        bool flag = false;
        if ((QCARWrapper.Instance.ImageTargetDestroyVirtualButton(this.mDataSet.DataSetPtr, base.Name, vb.Name) != 0) && this.mVirtualButtons.Remove(key))
        {
            flag = true;
        }
        if (!flag)
        {
            Debug.LogError("UnregisterVirtualButton: Failed to destroy the Virtual Button.");
        }
        return flag;
    }

    public ImageTargetType ImageTargetType
    {
        get
        {
            return this.mImageTargetType;
        }
    }
}

