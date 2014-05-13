using System;
using UnityEngine;

internal class WordResultImpl : WordResult
{
    private OrientedBoundingBox mObb;
    private Quaternion mOrientation;
    private Vector3 mPosition;
    private TrackableBehaviour.Status mStatus;
    private readonly Word mWord;

    public WordResultImpl(Word word)
    {
        this.mWord = word;
    }

    public void SetObb(OrientedBoundingBox obb)
    {
        this.mObb = obb;
    }

    public void SetPose(Vector3 position, Quaternion orientation)
    {
        this.mPosition = position;
        this.mOrientation = orientation;
    }

    public void SetStatus(TrackableBehaviour.Status status)
    {
        this.mStatus = status;
    }

    public override TrackableBehaviour.Status CurrentStatus
    {
        get
        {
            return this.mStatus;
        }
    }

    public override OrientedBoundingBox Obb
    {
        get
        {
            return this.mObb;
        }
    }

    public override Quaternion Orientation
    {
        get
        {
            return this.mOrientation;
        }
    }

    public override Vector3 Position
    {
        get
        {
            return this.mPosition;
        }
    }

    public override Word Word
    {
        get
        {
            return this.mWord;
        }
    }
}

