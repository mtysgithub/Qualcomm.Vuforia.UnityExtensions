using System;
using UnityEngine;

public abstract class WordResult
{
    protected WordResult()
    {
    }

    public abstract TrackableBehaviour.Status CurrentStatus { get; }

    public abstract OrientedBoundingBox Obb { get; }

    public abstract Quaternion Orientation { get; }

    public abstract Vector3 Position { get; }

    public abstract Word Word { get; }
}

