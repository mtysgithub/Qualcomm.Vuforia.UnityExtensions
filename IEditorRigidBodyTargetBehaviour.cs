using System;

internal interface IEditorRigidBodyTargetBehaviour : IEditorDataSetTrackableBehaviour, IEditorTrackableBehaviour
{
    void InitializeRigidBodyTarget(InternalRigidBodyTarget rigidBodyTarget);
}

