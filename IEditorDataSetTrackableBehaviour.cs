using System;

public interface IEditorDataSetTrackableBehaviour : IEditorTrackableBehaviour
{
    bool SetDataSetPath(string dataSetPath);
    void SetExtendedTracking(bool extendedTracking);

    string DataSetName { get; }

    string DataSetPath { get; }

    bool ExtendedTracking { get; }
}

