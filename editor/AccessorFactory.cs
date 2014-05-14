using System;
using UnityEngine;

public class AccessorFactory
{
    public static TrackableAccessor Create(TrackableBehaviour target)
    {
        if (target is MarkerAbstractBehaviour)
        {
            return new MarkerAccessor((MarkerAbstractBehaviour) target);
        }
        if (target is ImageTargetAbstractBehaviour)
        {
            return new ImageTargetAccessor((ImageTargetAbstractBehaviour) target);
        }
        if (target is MultiTargetAbstractBehaviour)
        {
            return new MultiTargetAccessor((MultiTargetAbstractBehaviour) target);
        }
        if (target is CylinderTargetAbstractBehaviour)
        {
            return new CylinderTargetAccessor((CylinderTargetAbstractBehaviour) target);
        }
        if (target is IEditorRigidBodyTargetBehaviour)
        {
            return new RigidBodyTargetAccessor((IEditorRigidBodyTargetBehaviour) target);
        }
        Debug.LogWarning(target.GetType().ToString() + " is not derived from TrackableBehaviour.");
        return null;
    }
}

