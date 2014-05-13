using System;
using UnityEngine;

public interface IEditorTrackableBehaviour
{
    bool CorrectScale();
    bool SetInitializedInEditor(bool initializedInEditor);
    bool SetNameForTrackable(string name);
    bool SetPreserveChildSize(bool preserveChildSize);
    bool SetPreviousScale(Vector3 previousScale);
    void UnregisterTrackable();

    bool enabled { get; set; }

    GameObject gameObject { get; }

    bool InitializedInEditor { get; }

    bool PreserveChildSize { get; }

    Vector3 PreviousScale { get; }

    Renderer renderer { get; }

    Trackable Trackable { get; }

    string TrackableName { get; }

    Transform transform { get; }
}

