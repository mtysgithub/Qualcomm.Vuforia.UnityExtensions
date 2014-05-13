using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class VirtualButtonAbstractBehaviour : MonoBehaviour, IEditorVirtualButtonBehaviour
{
    private List<IVirtualButtonEventHandler> mHandlers = new List<IVirtualButtonEventHandler>();
    [HideInInspector, SerializeField]
    private bool mHasUpdatedPose = false;
    private Vector2 mLeftTop;
    [SerializeField, HideInInspector]
    private string mName = "";
    private bool mPressed = false;
    private bool mPreviouslyEnabled;
    [SerializeField, HideInInspector]
    private GameObject mPrevParent;
    [SerializeField, HideInInspector]
    private Matrix4x4 mPrevTransform = Matrix4x4.zero;
    private Vector2 mRightBottom;
    [SerializeField, HideInInspector]
    private VirtualButton.Sensitivity mSensitivity = VirtualButton.Sensitivity.LOW;
    private bool mSensitivityDirty = false;
    private bool mUnregisterOnDestroy;
    private VirtualButton mVirtualButton;
    public const float TARGET_OFFSET = 0.001f;

    public bool CalculateButtonArea(out Vector2 topLeft, out Vector2 bottomRight)
    {
        ImageTargetAbstractBehaviour imageTargetBehaviour = this.GetImageTargetBehaviour();
        if (imageTargetBehaviour == null)
        {
            topLeft = bottomRight = Vector2.zero;
            return false;
        }
        Vector3 vector = imageTargetBehaviour.transform.InverseTransformPoint(base.transform.position);
        float num = imageTargetBehaviour.transform.lossyScale[0];
        Vector2 vector2 = new Vector2(vector[0] * num, vector[2] * num);
        Vector2 vector3 = new Vector2(base.transform.lossyScale[0], base.transform.lossyScale[2]);
        Vector2 vector4 = Vector2.Scale((Vector2) (vector3 * 0.5f), new Vector2(1f, -1f));
        topLeft = vector2 - vector4;
        bottomRight = vector2 + vector4;
        return true;
    }

    private static bool Equals(Vector2 vec1, Vector2 vec2, float threshold)
    {
        Vector2 vector = vec1 - vec2;
        return ((Math.Abs(vector.x) < threshold) && (Math.Abs(vector.y) < threshold));
    }

    public ImageTargetAbstractBehaviour GetImageTargetBehaviour()
    {
        if (base.transform.parent != null)
        {
            for (GameObject obj2 = base.transform.parent.gameObject; obj2 != null; obj2 = obj2.transform.parent.gameObject)
            {
                ImageTargetAbstractBehaviour component = obj2.GetComponent<ImageTargetAbstractBehaviour>();
                if (component != null)
                {
                    return component;
                }
                if (obj2.transform.parent == null)
                {
                    return null;
                }
            }
        }
        return null;
    }

    void IEditorVirtualButtonBehaviour.InitializeVirtualButton(VirtualButton virtualButton)
    {
        this.mVirtualButton = virtualButton;
    }

    bool IEditorVirtualButtonBehaviour.SetPosAndScaleFromButtonArea(Vector2 topLeft, Vector2 bottomRight)
    {
        ImageTargetAbstractBehaviour imageTargetBehaviour = this.GetImageTargetBehaviour();
        if (imageTargetBehaviour == null)
        {
            return false;
        }
        float num = imageTargetBehaviour.transform.lossyScale[0];
        Vector2 vector = (Vector2) ((topLeft + bottomRight) * 0.5f);
        Vector2 vector2 = new Vector2(bottomRight[0] - topLeft[0], topLeft[1] - bottomRight[1]);
        Vector3 position = new Vector3(vector[0] / num, 0.001f, vector[1] / num);
        Vector3 vector4 = new Vector3(vector2[0], (vector2[0] + vector2[1]) * 0.5f, vector2[1]);
        base.transform.position = imageTargetBehaviour.transform.TransformPoint(position);
        base.transform.localScale = (Vector3) (vector4 / base.transform.parent.lossyScale[0]);
        return true;
    }

    bool IEditorVirtualButtonBehaviour.SetPreviousParent(GameObject parent)
    {
        if (this.mVirtualButton == null)
        {
            this.mPrevParent = parent;
            return true;
        }
        return false;
    }

    bool IEditorVirtualButtonBehaviour.SetPreviousTransform(Matrix4x4 transform)
    {
        if (this.mVirtualButton == null)
        {
            this.mPrevTransform = transform;
            return true;
        }
        return false;
    }

    bool IEditorVirtualButtonBehaviour.SetSensitivitySetting(VirtualButton.Sensitivity sensibility)
    {
        if (this.mVirtualButton == null)
        {
            this.mSensitivity = sensibility;
            this.mSensitivityDirty = true;
            return true;
        }
        return false;
    }

    bool IEditorVirtualButtonBehaviour.SetVirtualButtonName(string virtualButtonName)
    {
        if (this.mVirtualButton == null)
        {
            this.mName = virtualButtonName;
            return true;
        }
        return false;
    }

    private void LateUpdate()
    {
        if (this.UpdatePose())
        {
            this.UpdateAreaRectangle();
        }
        if (this.mSensitivityDirty && this.UpdateSensitivity())
        {
            this.mSensitivityDirty = false;
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying && this.mUnregisterOnDestroy)
        {
            ImageTargetAbstractBehaviour imageTargetBehaviour = this.GetImageTargetBehaviour();
            if (imageTargetBehaviour != null)
            {
                imageTargetBehaviour.ImageTarget.DestroyVirtualButton(this.mVirtualButton);
            }
        }
    }

    private void OnDisable()
    {
        if (QCARRuntimeUtilities.IsQCAREnabled())
        {
            if (this.mPreviouslyEnabled != base.enabled)
            {
                this.mPreviouslyEnabled = base.enabled;
                this.UpdateEnabled();
            }
            if (this.mPressed && (this.mHandlers != null))
            {
                foreach (IVirtualButtonEventHandler handler in this.mHandlers)
                {
                    handler.OnButtonReleased(this);
                }
            }
            this.mPressed = false;
        }
    }

    public void OnTrackerUpdated(bool pressed)
    {
        if (this.mPreviouslyEnabled != base.enabled)
        {
            this.mPreviouslyEnabled = base.enabled;
            this.UpdateEnabled();
        }
        if (base.enabled)
        {
            if ((this.mPressed != pressed) && (this.mHandlers != null))
            {
                if (pressed)
                {
                    foreach (IVirtualButtonEventHandler handler in this.mHandlers)
                    {
                        handler.OnButtonPressed(this);
                    }
                }
                else
                {
                    foreach (IVirtualButtonEventHandler handler2 in this.mHandlers)
                    {
                        handler2.OnButtonReleased(this);
                    }
                }
            }
            this.mPressed = pressed;
        }
    }

    public void RegisterEventHandler(IVirtualButtonEventHandler eventHandler)
    {
        this.mHandlers.Add(eventHandler);
    }

    public bool UnregisterEventHandler(IVirtualButtonEventHandler eventHandler)
    {
        return this.mHandlers.Remove(eventHandler);
    }

    public bool UpdateAreaRectangle()
    {
        RectangleData area = new RectangleData {
            leftTopX = this.mLeftTop.x,
            leftTopY = this.mLeftTop.y,
            rightBottomX = this.mRightBottom.x,
            rightBottomY = this.mRightBottom.y
        };
        if (this.mVirtualButton == null)
        {
            return false;
        }
        return this.mVirtualButton.SetArea(area);
    }

    private bool UpdateEnabled()
    {
        return this.mVirtualButton.SetEnabled(base.enabled);
    }

    public bool UpdatePose()
    {
        Vector2 vector3;
        Vector2 vector4;
        ImageTargetAbstractBehaviour imageTargetBehaviour = this.GetImageTargetBehaviour();
        if (imageTargetBehaviour == null)
        {
            return false;
        }
        for (Transform transform = base.transform.parent; transform != null; transform = transform.parent)
        {
            if ((transform.localScale[0] != transform.localScale[1]) || (transform.localScale[0] != transform.localScale[2]))
            {
                Debug.LogWarning("Detected non-uniform scale in virtual  button object hierarchy. Forcing uniform scaling of object '" + transform.name + "'.");
                transform.localScale = new Vector3(transform.localScale[0], transform.localScale[0], transform.localScale[0]);
            }
        }
        this.mHasUpdatedPose = true;
        if ((base.transform.parent != null) && (base.transform.parent.gameObject != imageTargetBehaviour.gameObject))
        {
            base.transform.localPosition = Vector3.zero;
        }
        Vector3 position = imageTargetBehaviour.transform.InverseTransformPoint(base.transform.position);
        position.y = 0.001f;
        Vector3 vector2 = imageTargetBehaviour.transform.TransformPoint(position);
        base.transform.position = vector2;
        base.transform.rotation = imageTargetBehaviour.transform.rotation;
        this.CalculateButtonArea(out vector3, out vector4);
        float threshold = imageTargetBehaviour.transform.localScale[0] * 0.001f;
        if (Equals(vector3, this.mLeftTop, threshold) && Equals(vector4, this.mRightBottom, threshold))
        {
            return false;
        }
        this.mLeftTop = vector3;
        this.mRightBottom = vector4;
        return true;
    }

    public bool UpdateSensitivity()
    {
        if (this.mVirtualButton == null)
        {
            return false;
        }
        return this.mVirtualButton.SetSensitivity(this.mSensitivity);
    }

    public bool HasUpdatedPose
    {
        get
        {
            return this.mHasUpdatedPose;
        }
    }

    bool IEditorVirtualButtonBehaviour.enabled
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

    GameObject IEditorVirtualButtonBehaviour.gameObject
    {
        get
        {
            return base.gameObject;
        }
    }

    GameObject IEditorVirtualButtonBehaviour.PreviousParent
    {
        get
        {
            return this.mPrevParent;
        }
    }

    Matrix4x4 IEditorVirtualButtonBehaviour.PreviousTransform
    {
        get
        {
            return this.mPrevTransform;
        }
    }

    Renderer IEditorVirtualButtonBehaviour.renderer
    {
        get
        {
            return base.renderer;
        }
    }

    VirtualButton.Sensitivity IEditorVirtualButtonBehaviour.SensitivitySetting
    {
        get
        {
            return this.mSensitivity;
        }
    }

    Transform IEditorVirtualButtonBehaviour.transform
    {
        get
        {
            return base.transform;
        }
    }

    public bool Pressed
    {
        get
        {
            return this.mPressed;
        }
    }

    public bool UnregisterOnDestroy
    {
        get
        {
            return this.mUnregisterOnDestroy;
        }
        set
        {
            this.mUnregisterOnDestroy = value;
        }
    }

    public VirtualButton VirtualButton
    {
        get
        {
            return this.mVirtualButton;
        }
    }

    public string VirtualButtonName
    {
        get
        {
            return this.mName;
        }
    }
}

