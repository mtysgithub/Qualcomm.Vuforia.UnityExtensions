using System;
using UnityEngine;

public abstract class CylinderTargetAbstractBehaviour : DataSetTrackableBehaviour, IEditorCylinderTargetBehaviour, IEditorDataSetTrackableBehaviour, IEditorTrackableBehaviour
{
    [SerializeField, HideInInspector]
    private float mBottomDiameterRatio;
    private CylinderTarget mCylinderTarget;
    private int mFrameIndex = -1;
    private float mFutureScale;
    [HideInInspector, SerializeField]
    private float mTopDiameterRatio;
    private int mUpdateFrameIndex = -1;

    protected CylinderTargetAbstractBehaviour()
    {
    }

    private void ApplyScale(float value)
    {
        base.transform.localScale = new Vector3(value, value, value);
    }

    protected override bool CorrectScaleImpl()
    {
        for (int i = 0; i < 3; i++)
        {
            if (base.transform.localScale[i] != this.mPreviousScale[i])
            {
                base.transform.localScale = new Vector3(base.transform.localScale[i], base.transform.localScale[i], base.transform.localScale[i]);
                base.mPreviousScale = base.transform.localScale;
                return true;
            }
        }
        return false;
    }

    private float GetScale()
    {
        return base.transform.localScale.x;
    }

    void IEditorCylinderTargetBehaviour.InitializeCylinderTarget(CylinderTarget cylinderTarget)
    {
        base.mTrackable = this.mCylinderTarget = cylinderTarget;
        cylinderTarget.SetSideLength(this.SideLength);
        if (base.mExtendedTracking)
        {
            this.mCylinderTarget.StartExtendedTracking();
        }
    }

    void IEditorCylinderTargetBehaviour.SetAspectRatio(float topRatio, float bottomRatio)
    {
        this.mTopDiameterRatio = topRatio;
        this.mBottomDiameterRatio = bottomRatio;
    }

    protected override void InternalUnregisterTrackable()
    {
        base.mTrackable = (Trackable) (this.mCylinderTarget = null);
    }

    public override void OnFrameIndexUpdate(int newFrameIndex)
    {
        if ((this.mUpdateFrameIndex >= 0) && (this.mUpdateFrameIndex != newFrameIndex))
        {
            this.ApplyScale(this.mFutureScale);
            this.mUpdateFrameIndex = -1;
        }
        this.mFrameIndex = newFrameIndex;
    }

    public bool SetBottomDiameter(float value)
    {
        return ((Math.Abs(this.mBottomDiameterRatio) > 1E-05f) && this.SetScale(value / this.mBottomDiameterRatio));
    }

    private bool SetScale(float value)
    {
        if (base.transform.localScale.x != value)
        {
            if (this.mCylinderTarget != null)
            {
                if (!this.mCylinderTarget.SetSideLength(value))
                {
                    return false;
                }
                this.mUpdateFrameIndex = this.mFrameIndex;
                this.mFutureScale = value;
            }
            else
            {
                this.ApplyScale(value);
            }
        }
        return true;
    }

    public bool SetSideLength(float value)
    {
        return this.SetScale(value);
    }

    public bool SetTopDiameter(float value)
    {
        return ((Math.Abs(this.mTopDiameterRatio) > 1E-05f) && this.SetScale(value / this.mTopDiameterRatio));
    }

    public float BottomDiameter
    {
        get
        {
            return (this.mBottomDiameterRatio * this.GetScale());
        }
    }

    public CylinderTarget CylinderTarget
    {
        get
        {
            return this.mCylinderTarget;
        }
    }

    bool IEditorTrackableBehaviour.enabled
    {
        get
        {
            return base.enabled;
        }
        set
        {
            base.enabled = value;
        }
    }

    GameObject IEditorTrackableBehaviour.gameObject
    {
        get
        {
            return base.gameObject;
        }
    }

    Renderer IEditorTrackableBehaviour.renderer
    {
        get
        {
            return base.renderer;
        }
    }

    Transform IEditorTrackableBehaviour.transform
    {
        get
        {
            return base.transform;
        }
    }

    public float SideLength
    {
        get
        {
            return this.GetScale();
        }
    }

    public float TopDiameter
    {
        get
        {
            return (this.mTopDiameterRatio * this.GetScale());
        }
    }
}

