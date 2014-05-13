using System;

public interface ICloudRecoEventHandler
{
    void OnInitError(TargetFinder.InitState initError);
    void OnInitialized();
    void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult);
    void OnStateChanged(bool scanning);
    void OnUpdateError(TargetFinder.UpdateState updateError);
}

