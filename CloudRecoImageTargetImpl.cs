using System;
using System.Collections.Generic;
using UnityEngine;

internal class CloudRecoImageTargetImpl : TrackableImpl, ImageTarget, ExtendedTrackable, Trackable
{
    private readonly Vector2 mSize;

    public CloudRecoImageTargetImpl(string name, int id, Vector2 size) : base(name, id)
    {
        this.mSize = size;
    }

    public VirtualButton CreateVirtualButton(string name, RectangleData area)
    {
        Debug.LogError("Virtual buttons are currently not supported for cloud reco targets.");
        return null;
    }

    public bool DestroyVirtualButton(VirtualButton vb)
    {
        Debug.LogError("Virtual buttons are currently not supported for cloud reco targets.");
        return false;
    }

    public Vector2 GetSize()
    {
        return this.mSize;
    }

    public VirtualButton GetVirtualButtonByName(string name)
    {
        Debug.LogError("Virtual buttons are currently not supported for cloud reco targets.");
        return null;
    }

    public IEnumerable<VirtualButton> GetVirtualButtons()
    {
        Debug.LogError("Virtual buttons are currently not supported for cloud reco targets.");
        return new List<VirtualButton>();
    }

    public void SetSize(Vector2 size)
    {
        Debug.LogError("Setting the size of cloud reco targets is currently not supported.");
    }

    public bool StartExtendedTracking()
    {
        return (QCARWrapper.Instance.StartExtendedTracking(IntPtr.Zero, base.ID) > 0);
    }

    public bool StopExtendedTracking()
    {
        return (QCARWrapper.Instance.StopExtendedTracking(IntPtr.Zero, base.ID) > 0);
    }

    public ImageTargetType ImageTargetType
    {
        get
        {
            return ImageTargetType.CLOUD_RECO;
        }
    }
}

