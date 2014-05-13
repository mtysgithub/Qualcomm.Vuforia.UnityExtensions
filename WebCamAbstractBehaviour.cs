using System;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public abstract class WebCamAbstractBehaviour : MonoBehaviour
{
    public Camera BackgroundCameraPrefab;
    private Camera mBackgroundCameraInstance;
    [HideInInspector, SerializeField]
    private string mDeviceNameSetInEditor;
    [SerializeField, HideInInspector]
    private bool mFlipHorizontally;
    [HideInInspector, SerializeField]
    private bool mTurnOffWebCam;
    private WebCamImpl mWebCamImpl;
    public int RenderTextureLayer;

    protected WebCamAbstractBehaviour()
    {
    }

    public bool CheckNativePluginSupport()
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            return true;
        }
        int num = 0;
        try
        {
            num = qcarCheckNativePluginSupport();
        }
        catch (Exception)
        {
            num = 0;
        }
        return (num == 1);
    }

    public void InitCamera()
    {
        if (this.mWebCamImpl == null)
        {
            Application.runInBackground = true;
            Camera component = base.gameObject.GetComponent<Camera>();
            if (this.BackgroundCameraPrefab != null)
            {
                this.mBackgroundCameraInstance = UnityEngine.Object.Instantiate(this.BackgroundCameraPrefab) as Camera;
                if ((KeepAliveAbstractBehaviour.Instance != null) && KeepAliveAbstractBehaviour.Instance.KeepARCameraAlive)
                {
                    UnityEngine.Object.DontDestroyOnLoad(this.mBackgroundCameraInstance.gameObject);
                }
            }
            this.mWebCamImpl = new WebCamImpl(component, this.mBackgroundCameraInstance, this.RenderTextureLayer, this.mDeviceNameSetInEditor, this.mFlipHorizontally);
        }
    }

    public bool IsWebCamUsed()
    {
        return ((!this.mTurnOffWebCam && this.CheckNativePluginSupport()) && (WebCamTexture.devices.Length > 0));
    }

    private void OnDestroy()
    {
        if (QCARRuntimeUtilities.IsPlayMode() && (this.mWebCamImpl != null))
        {
            this.mWebCamImpl.OnDestroy();
            UnityEngine.Object.Destroy(this.mBackgroundCameraInstance);
        }
    }

    private void OnLevelWasLoaded()
    {
        if (QCARRuntimeUtilities.IsPlayMode() && (this.mWebCamImpl != null))
        {
            this.mWebCamImpl.ResetPlaying();
        }
    }

    [DllImport("QCARWrapper")]
    private static extern int qcarCheckNativePluginSupport();
    public void StartCamera()
    {
        this.mWebCamImpl.StartCamera();
    }

    public void StopCamera()
    {
        this.mWebCamImpl.StopCamera();
    }

    private void Update()
    {
        if (QCARRuntimeUtilities.IsPlayMode() && (this.mWebCamImpl != null))
        {
            this.mWebCamImpl.Update();
        }
    }

    public string DeviceName
    {
        get
        {
            return this.mDeviceNameSetInEditor;
        }
        set
        {
            this.mDeviceNameSetInEditor = value;
        }
    }

    public bool FlipHorizontally
    {
        get
        {
            return this.mFlipHorizontally;
        }
        set
        {
            this.mFlipHorizontally = value;
        }
    }

    public WebCamImpl ImplementationClass
    {
        get
        {
            return this.mWebCamImpl;
        }
    }

    public bool IsPlaying
    {
        get
        {
            return this.mWebCamImpl.IsPlaying;
        }
    }

    public bool TurnOffWebCam
    {
        get
        {
            return this.mTurnOffWebCam;
        }
        set
        {
            this.mTurnOffWebCam = value;
        }
    }
}

