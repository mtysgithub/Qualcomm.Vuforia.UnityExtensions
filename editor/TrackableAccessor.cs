using System;

public abstract class TrackableAccessor
{
    protected TrackableBehaviour mTarget;

    protected TrackableAccessor()
    {
    }

    public abstract void ApplyDataSetAppearance();
    public abstract void ApplyDataSetProperties();
}

