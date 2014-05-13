using System;
using System.Collections.Generic;

public interface ILoadLevelEventHandler
{
    void OnDuplicateTrackablesDisabled(IEnumerable<TrackableBehaviour> disabledTrackables);
    void OnLevelLoaded(IEnumerable<TrackableBehaviour> keptAliveTrackables);
}

