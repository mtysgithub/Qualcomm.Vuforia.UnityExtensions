using System;

internal interface IPremiumObjectFactory
{
    InternalRigidBodyTarget CreateRigidBodyTarget(string trackableName, int trackableID);
}

