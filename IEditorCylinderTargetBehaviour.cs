using System;

public interface IEditorCylinderTargetBehaviour : IEditorDataSetTrackableBehaviour, IEditorTrackableBehaviour
{
    void InitializeCylinderTarget(CylinderTarget cylinderTarget);
    void SetAspectRatio(float topRatio, float bottomRatio);
}

