using System;

public interface IUserDefinedTargetEventHandler
{
    void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality);
    void OnInitialized();
    void OnNewTrackableSource(TrackableSource trackableSource);
}

