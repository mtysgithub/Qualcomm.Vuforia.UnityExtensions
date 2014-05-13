using System;

public interface IEditorMultiTargetBehaviour : IEditorDataSetTrackableBehaviour, IEditorTrackableBehaviour
{
    void InitializeMultiTarget(MultiTarget multiTarget);
}

