using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public abstract class SetBGCameraLayerAbstractBehaviour : MonoBehaviour
{
    public int CameraLayer;

    protected SetBGCameraLayerAbstractBehaviour()
    {
    }

    private void ApplyCameraLayerRecursive(GameObject go)
    {
        go.layer = this.CameraLayer;
        for (int i = 0; i < go.transform.GetChildCount(); i++)
        {
            this.ApplyCameraLayerRecursive(go.transform.GetChild(i).gameObject);
        }
    }

    private void Awake()
    {
        this.ApplyCameraLayerRecursive(base.gameObject);
        Camera component = base.GetComponent<Camera>();
        component.cullingMask |= ((int) 1) << this.CameraLayer;
    }
}

