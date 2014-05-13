using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class ImageTargetAbstractBehaviour : DataSetTrackableBehaviour, IEditorImageTargetBehaviour, IEditorDataSetTrackableBehaviour, IEditorTrackableBehaviour
{
    [SerializeField, HideInInspector]
    private float mAspectRatio = 1f;
    private ImageTarget mImageTarget;
    [HideInInspector, SerializeField]
    private ImageTargetType mImageTargetType;
    private Dictionary<int, VirtualButtonAbstractBehaviour> mVirtualButtonBehaviours;

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

    private bool CreateNewVirtualButtonFromBehaviour(VirtualButtonAbstractBehaviour newVBB)
    {
        Vector2 vector;
        Vector2 vector2;
        newVBB.CalculateButtonArea(out vector, out vector2);
        RectangleData area = new RectangleData {
            leftTopX = vector.x,
            leftTopY = vector.y,
            rightBottomX = vector2.x,
            rightBottomY = vector2.y
        };
        VirtualButton virtualButton = this.mImageTarget.CreateVirtualButton(newVBB.VirtualButtonName, area);
        if (virtualButton == null)
        {
            UnityEngine.Object.Destroy(newVBB.gameObject);
            return false;
        }
        IEditorVirtualButtonBehaviour behaviour = newVBB;
        behaviour.InitializeVirtualButton(virtualButton);
        this.mVirtualButtonBehaviours.Add(virtualButton.ID, newVBB);
        return true;
    }

    public static VirtualButtonAbstractBehaviour CreateVirtualButton(string vbName, Vector2 localScale, GameObject immediateParent)
    {
        GameObject gameObject = new GameObject(vbName);
        VirtualButtonAbstractBehaviour newVBB = BehaviourComponentFactory.Instance.AddVirtualButtonBehaviour(gameObject);
        ImageTargetAbstractBehaviour componentInChildren = immediateParent.transform.root.gameObject.GetComponentInChildren<ImageTargetAbstractBehaviour>();
        if ((componentInChildren == null) || (componentInChildren.ImageTarget == null))
        {
            Debug.LogError("Could not create Virtual Button. immediateParent\"immediateParent\" object is not an Image Target or a child of one.");
            UnityEngine.Object.Destroy(gameObject);
            return null;
        }
        gameObject.transform.parent = immediateParent.transform;
        IEditorVirtualButtonBehaviour behaviour3 = newVBB;
        behaviour3.SetVirtualButtonName(vbName);
        behaviour3.transform.localScale = new Vector3(localScale[0], 1f, localScale[1]);
        if (Application.isPlaying && !componentInChildren.CreateNewVirtualButtonFromBehaviour(newVBB))
        {
            return null;
        }
        newVBB.UnregisterOnDestroy = true;
        return newVBB;
    }

    public VirtualButtonAbstractBehaviour CreateVirtualButton(string vbName, Vector2 position, Vector2 size)
    {
        GameObject gameObject = new GameObject(vbName);
        VirtualButtonAbstractBehaviour newVBB = BehaviourComponentFactory.Instance.AddVirtualButtonBehaviour(gameObject);
        gameObject.transform.parent = base.transform;
        IEditorVirtualButtonBehaviour behaviour2 = newVBB;
        behaviour2.SetVirtualButtonName(vbName);
        behaviour2.transform.localScale = new Vector3(size.x, 1f, size.y);
        behaviour2.transform.localPosition = new Vector3(position.x, 1f, position.y);
        if (Application.isPlaying && !this.CreateNewVirtualButtonFromBehaviour(newVBB))
        {
            return null;
        }
        newVBB.UnregisterOnDestroy = true;
        return newVBB;
    }

    private void CreateVirtualButtonFromNative(VirtualButton virtualButton)
    {
        GameObject gameObject = new GameObject(virtualButton.Name);
        VirtualButtonAbstractBehaviour behaviour = BehaviourComponentFactory.Instance.AddVirtualButtonBehaviour(gameObject);
        behaviour.transform.parent = base.transform;
        IEditorVirtualButtonBehaviour behaviour2 = behaviour;
        Debug.Log(string.Concat(new object[] { "Creating Virtual Button with values: \n ID:           ", virtualButton.ID, "\n Name:         ", virtualButton.Name, "\n Rectangle:    ", virtualButton.Area.leftTopX, ",", virtualButton.Area.leftTopY, ",", virtualButton.Area.rightBottomX, ",", virtualButton.Area.rightBottomY }));
        behaviour2.SetVirtualButtonName(virtualButton.Name);
        behaviour2.SetPosAndScaleFromButtonArea(new Vector2(virtualButton.Area.leftTopX, virtualButton.Area.leftTopY), new Vector2(virtualButton.Area.rightBottomX, virtualButton.Area.rightBottomY));
        behaviour2.UnregisterOnDestroy = false;
        behaviour2.InitializeVirtualButton(virtualButton);
        this.mVirtualButtonBehaviours.Add(virtualButton.ID, behaviour);
    }

    public void DestroyVirtualButton(string vbName)
    {
        List<VirtualButtonAbstractBehaviour> list = new List<VirtualButtonAbstractBehaviour>(this.mVirtualButtonBehaviours.Values);
        foreach (VirtualButtonAbstractBehaviour behaviour in list)
        {
            if (behaviour.VirtualButtonName == vbName)
            {
                this.mVirtualButtonBehaviours.Remove(behaviour.VirtualButton.ID);
                behaviour.UnregisterOnDestroy = true;
                UnityEngine.Object.Destroy(behaviour.gameObject);
                break;
            }
        }
    }

    public Vector2 GetSize()
    {
        if (this.mAspectRatio <= 1f)
        {
            return new Vector2(base.transform.localScale.x, base.transform.localScale.x * this.mAspectRatio);
        }
        return new Vector2(base.transform.localScale.x / this.mAspectRatio, base.transform.localScale.x);
    }

    public IEnumerable<VirtualButtonAbstractBehaviour> GetVirtualButtonBehaviours()
    {
        return this.mVirtualButtonBehaviours.Values;
    }

    void IEditorImageTargetBehaviour.AssociateExistingVirtualButtonBehaviour(VirtualButtonAbstractBehaviour virtualButtonBehaviour)
    {
        VirtualButton virtualButtonByName = this.mImageTarget.GetVirtualButtonByName(virtualButtonBehaviour.VirtualButtonName);
        if (virtualButtonByName == null)
        {
            Vector2 vector;
            Vector2 vector2;
            virtualButtonBehaviour.CalculateButtonArea(out vector, out vector2);
            RectangleData area = new RectangleData {
                leftTopX = vector.x,
                leftTopY = vector.y,
                rightBottomX = vector2.x,
                rightBottomY = vector2.y
            };
            virtualButtonByName = this.mImageTarget.CreateVirtualButton(virtualButtonBehaviour.VirtualButtonName, area);
            if (virtualButtonByName != null)
            {
                Debug.Log("Successfully created virtual button " + virtualButtonBehaviour.VirtualButtonName + " at startup");
                virtualButtonBehaviour.UnregisterOnDestroy = true;
            }
            else
            {
                Debug.LogError("Failed to create virtual button " + virtualButtonBehaviour.VirtualButtonName + " at startup");
            }
        }
        if ((virtualButtonByName != null) && !this.mVirtualButtonBehaviours.ContainsKey(virtualButtonByName.ID))
        {
            IEditorVirtualButtonBehaviour behaviour = virtualButtonBehaviour;
            behaviour.InitializeVirtualButton(virtualButtonByName);
            this.mVirtualButtonBehaviours.Add(virtualButtonByName.ID, virtualButtonBehaviour);
            Debug.Log(string.Concat(new object[] { "Found VirtualButton named ", virtualButtonBehaviour.VirtualButton.Name, " with id ", virtualButtonBehaviour.VirtualButton.ID }));
            virtualButtonBehaviour.UpdatePose();
            if (!virtualButtonBehaviour.UpdateAreaRectangle() || !virtualButtonBehaviour.UpdateSensitivity())
            {
                Debug.LogError("Failed to update virtual button " + virtualButtonBehaviour.VirtualButton.Name + " at startup");
            }
            else
            {
                Debug.Log("Updated virtual button " + virtualButtonBehaviour.VirtualButton.Name + " at startup");
            }
        }
    }

    void IEditorImageTargetBehaviour.CreateMissingVirtualButtonBehaviours()
    {
        foreach (VirtualButton button in this.mImageTarget.GetVirtualButtons())
        {
            this.CreateVirtualButtonFromNative(button);
        }
    }

    void IEditorImageTargetBehaviour.InitializeImageTarget(ImageTarget imageTarget)
    {
        base.mTrackable = this.mImageTarget = imageTarget;
        this.mVirtualButtonBehaviours = new Dictionary<int, VirtualButtonAbstractBehaviour>();
        if (imageTarget.ImageTargetType == ImageTargetType.PREDEFINED)
        {
            Vector2 size = this.GetSize();
            imageTarget.SetSize(size);
        }
        else
        {
            Vector2 vector2 = imageTarget.GetSize();
            float x = (vector2.x > vector2.y) ? vector2.x : vector2.y;
            base.transform.localScale = new Vector3(x, x, x);
            IEditorImageTargetBehaviour behaviour = this;
            behaviour.CorrectScale();
            this.mAspectRatio = vector2.y / vector2.x;
        }
        if (base.mExtendedTracking)
        {
            this.mImageTarget.StartExtendedTracking();
        }
    }

    bool IEditorImageTargetBehaviour.SetAspectRatio(float aspectRatio)
    {
        if (base.mTrackable == null)
        {
            this.mAspectRatio = aspectRatio;
            return true;
        }
        return false;
    }

    bool IEditorImageTargetBehaviour.SetImageTargetType(ImageTargetType imageTargetType)
    {
        if (base.mTrackable == null)
        {
            this.mImageTargetType = imageTargetType;
            return true;
        }
        return false;
    }

    bool IEditorImageTargetBehaviour.TryGetVirtualButtonBehaviourByID(int id, out VirtualButtonAbstractBehaviour virtualButtonBehaviour)
    {
        return this.mVirtualButtonBehaviours.TryGetValue(id, out virtualButtonBehaviour);
    }

    protected override void InternalUnregisterTrackable()
    {
        base.mTrackable = (Trackable) (this.mImageTarget = null);
    }

    public void SetHeight(float height)
    {
        float x = (this.mAspectRatio <= 1f) ? (height / this.mAspectRatio) : height;
        base.transform.localScale = new Vector3(x, x, x);
    }

    public void SetWidth(float width)
    {
        float x = (this.mAspectRatio <= 1f) ? width : (width / this.mAspectRatio);
        base.transform.localScale = new Vector3(x, x, x);
    }

    float IEditorImageTargetBehaviour.AspectRatio
    {
        get
        {
            return this.mAspectRatio;
        }
    }

    ImageTargetType IEditorImageTargetBehaviour.ImageTargetType
    {
        get
        {
            return this.mImageTargetType;
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

    public ImageTarget ImageTarget
    {
        get
        {
            return this.mImageTarget;
        }
    }
}

