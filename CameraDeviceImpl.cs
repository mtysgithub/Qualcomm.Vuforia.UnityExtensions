using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

internal class CameraDeviceImpl : CameraDevice
{
    private CameraDevice.CameraDirection mCameraDirection;
    private Dictionary<Image.PIXEL_FORMAT, Image> mCameraImages = new Dictionary<Image.PIXEL_FORMAT, Image>();
    private bool mCameraReady;
    private bool mIsDirty;
    private static WebCamImpl mWebCam;

    public override bool Deinit()
    {
        if (this.DeinitCameraDevice() == 0)
        {
            return false;
        }
        this.mCameraReady = false;
        return true;
    }

    private int DeinitCameraDevice()
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            return QCARWrapper.Instance.CameraDeviceDeinitCamera();
        }
        int num = 0;
        if (mWebCam != null)
        {
            mWebCam.StopCamera();
            num = 1;
        }
        QCARWrapper.Instance.CameraDeviceDeinitCamera();
        return num;
    }

    public Dictionary<Image.PIXEL_FORMAT, Image> GetAllImages()
    {
        return this.mCameraImages;
    }

    public override CameraDevice.CameraDirection GetCameraDirection()
    {
        return this.mCameraDirection;
    }

    public override Image GetCameraImage(Image.PIXEL_FORMAT format)
    {
        if (this.mCameraImages.ContainsKey(format))
        {
            Image image = this.mCameraImages[format];
            if (image.IsValid())
            {
                return image;
            }
        }
        return null;
    }

    public override CameraDevice.VideoModeData GetVideoMode(CameraDevice.CameraDeviceMode mode)
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            return this.WebCam.GetVideoMode();
        }
        IntPtr videoMode = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CameraDevice.VideoModeData)));
        QCARWrapper.Instance.CameraDeviceGetVideoMode((int) mode, videoMode);
        CameraDevice.VideoModeData data = (CameraDevice.VideoModeData) Marshal.PtrToStructure(videoMode, typeof(CameraDevice.VideoModeData));
        Marshal.FreeHGlobal(videoMode);
        return data;
    }

    public override bool Init(CameraDevice.CameraDirection cameraDirection)
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            cameraDirection = CameraDevice.CameraDirection.CAMERA_BACK;
        }
        if (this.InitCameraDevice((int) cameraDirection) == 0)
        {
            return false;
        }
        this.mCameraDirection = cameraDirection;
        this.mCameraReady = true;
        if (this.CameraReady)
        {
            QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
            if (behaviour != null)
            {
                behaviour.ResetClearBuffers();
                behaviour.ConfigureVideoBackground(true);
            }
        }
        return true;
    }

    private int InitCameraDevice(int camera)
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            return QCARWrapper.Instance.CameraDeviceInitCamera(camera);
        }
        int num = 0;
        try
        {
            WebCamAbstractBehaviour behaviour = (WebCamAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(WebCamAbstractBehaviour));
            behaviour.InitCamera();
            mWebCam = behaviour.ImplementationClass;
            QCARWrapper.Instance.CameraDeviceSetCameraConfiguration(mWebCam.ResampledTextureSize.x, mWebCam.ResampledTextureSize.y);
            num = 1;
        }
        catch (NullReferenceException exception)
        {
            Debug.LogError(exception.Message);
        }
        QCARWrapper.Instance.CameraDeviceInitCamera(camera);
        this.mCameraImages.Clear();
        return num;
    }

    public bool IsDirty()
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            return this.mIsDirty;
        }
        if (!this.mIsDirty)
        {
            return this.WebCam.IsRendererDirty();
        }
        return true;
    }

    public void ResetDirtyFlag()
    {
        this.mIsDirty = false;
    }

    public override bool SelectVideoMode(CameraDevice.CameraDeviceMode mode)
    {
        if (QCARWrapper.Instance.CameraDeviceSelectVideoMode((int) mode) == 0)
        {
            return false;
        }
        return true;
    }

    public override bool SetFlashTorchMode(bool on)
    {
        bool flag = QCARWrapper.Instance.CameraDeviceSetFlashTorchMode(on ? 1 : 0) != 0;
        Debug.Log("Toggle flash " + (on ? "ON" : "OFF") + " " + (flag ? "WORKED" : "FAILED"));
        return flag;
    }

    public override bool SetFocusMode(CameraDevice.FocusMode mode)
    {
        bool flag = QCARWrapper.Instance.CameraDeviceSetFocusMode((int) mode) != 0;
        Debug.Log("Requested Focus mode " + mode + (flag ? " successfully." : ".  Not supported on this device."));
        return flag;
    }

    public override bool SetFrameFormat(Image.PIXEL_FORMAT format, bool enabled)
    {
        if (enabled)
        {
            if (!this.mCameraImages.ContainsKey(format))
            {
                if (QCARWrapper.Instance.QcarSetFrameFormat((int) format, 1) == 0)
                {
                    Debug.LogError("Failed to set frame format");
                    return false;
                }
                Image image = new ImageImpl {
                    PixelFormat = format
                };
                this.mCameraImages.Add(format, image);
                return true;
            }
        }
        else if (this.mCameraImages.ContainsKey(format))
        {
            if (QCARWrapper.Instance.QcarSetFrameFormat((int) format, 0) == 0)
            {
                Debug.LogError("Failed to set frame format");
                return false;
            }
            return this.mCameraImages.Remove(format);
        }
        return true;
    }

    public override bool Start()
    {
        this.mIsDirty = true;
        GL.Clear(false, true, new Color(0f, 0f, 0f, 1f));
        if (this.StartCameraDevice() == 0)
        {
            return false;
        }
        return true;
    }

    private int StartCameraDevice()
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            return QCARWrapper.Instance.CameraDeviceStartCamera();
        }
        int num = 0;
        if (mWebCam != null)
        {
            mWebCam.StartCamera();
            num = 1;
        }
        QCARWrapper.Instance.CameraDeviceStartCamera();
        return num;
    }

    public override bool Stop()
    {
        if (this.StopCameraDevice() == 0)
        {
            return false;
        }
        return true;
    }

    private int StopCameraDevice()
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            return QCARWrapper.Instance.CameraDeviceStopCamera();
        }
        int num = 0;
        if (mWebCam != null)
        {
            mWebCam.StopCamera();
            num = 1;
        }
        QCARWrapper.Instance.CameraDeviceStopCamera();
        return num;
    }

    public bool CameraReady
    {
        get
        {
            if (!QCARRuntimeUtilities.IsPlayMode())
            {
                return this.mCameraReady;
            }
            return ((mWebCam != null) && mWebCam.IsTextureSizeAvailable);
        }
    }

    public WebCamImpl WebCam
    {
        get
        {
            return mWebCam;
        }
    }
}

