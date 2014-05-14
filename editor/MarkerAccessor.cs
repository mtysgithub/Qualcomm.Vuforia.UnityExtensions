using System;

public class MarkerAccessor : TrackableAccessor
{
    private const string MARKER_NAME_PREFIX = "mymarker";

    public MarkerAccessor(MarkerAbstractBehaviour target)
    {
        base.mTarget = target;
    }

    public override void ApplyDataSetAppearance()
    {
        throw new NotImplementedException();
    }

    public override void ApplyDataSetProperties()
    {
        QCARUtilities.GetPrefabType(base.mTarget);
    }

    public static string MarkerNamePrefix
    {
        get
        {
            return "mymarker";
        }
    }
}

