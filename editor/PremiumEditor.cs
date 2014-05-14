using System;

internal class PremiumEditor
{
    private static IPremiumEditor sInstance;

    internal static IPremiumEditor Instance
    {
        get
        {
            if (sInstance == null)
            {
                sInstance = new NullPremiumEditor();
            }
            return sInstance;
        }
        set
        {
            sInstance = value;
        }
    }

    private class NullPremiumEditor : IPremiumEditor
    {
    }
}

