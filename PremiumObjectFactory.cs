using System;

internal class PremiumObjectFactory
{
    private static IPremiumObjectFactory sInstance;

    internal static IPremiumObjectFactory Instance
    {
        get
        {
            if (sInstance == null)
            {
                sInstance = new NullPremiumObjectFactory();
            }
            return sInstance;
        }
        set
        {
            sInstance = value;
        }
    }

    private class NullPremiumObjectFactory : IPremiumObjectFactory
    {
        public InternalRigidBodyTarget CreateRigidBodyTarget(string trackableName, int trackableID)
        {
            return null;
        }
    }
}

