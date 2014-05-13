using System;
using UnityEngine;

internal class ImageTargetBuilderImpl : ImageTargetBuilder
{
    private TrackableSource mTrackableSource;

    public override bool Build(string targetName, float sceenSizeWidth)
    {
        if (targetName.Length > 0x40)
        {
            Debug.LogError("Invalid parameters to build User Defined Target:Target name exceeds 64 character limit");
            return false;
        }
        this.mTrackableSource = null;
        return (QCARWrapper.Instance.ImageTargetBuilderBuild(targetName, sceenSizeWidth) == 1);
    }

    public override ImageTargetBuilder.FrameQuality GetFrameQuality()
    {
        return (ImageTargetBuilder.FrameQuality) QCARWrapper.Instance.ImageTargetBuilderGetFrameQuality();
    }

    public override TrackableSource GetTrackableSource()
    {
        IntPtr trackableSourcePtr = QCARWrapper.Instance.ImageTargetBuilderGetTrackableSource();
        if ((this.mTrackableSource == null) && (trackableSourcePtr != IntPtr.Zero))
        {
            this.mTrackableSource = new TrackableSourceImpl(trackableSourcePtr);
        }
        return this.mTrackableSource;
    }

    public override void StartScan()
    {
        QCARWrapper.Instance.ImageTargetBuilderStartScan();
    }

    public override void StopScan()
    {
        QCARWrapper.Instance.ImageTargetBuilderStopScan();
    }
}

