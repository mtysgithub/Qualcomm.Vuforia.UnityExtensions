using System;

public class RigidBodyTargetAccessor : TrackableAccessor
{
    internal RigidBodyTargetAccessor(IEditorRigidBodyTargetBehaviour target)
    {
        base.mTarget = (TrackableBehaviour) target;
    }

    public override void ApplyDataSetAppearance()
    {
        QCARUtilities.GetPrefabType(base.mTarget);
    }

    public override void ApplyDataSetProperties()
    {
        QCARUtilities.GetPrefabType(base.mTarget);
    }
}

