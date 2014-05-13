using System;
using System.Runtime.InteropServices;

public abstract class CameraDevice
{
    private static CameraDevice mInstance;

    protected CameraDevice()
    {
    }

    public abstract bool Deinit();
    public abstract CameraDirection GetCameraDirection();
    public abstract Image GetCameraImage(Image.PIXEL_FORMAT format);
    public abstract VideoModeData GetVideoMode(CameraDeviceMode mode);
    public abstract bool Init(CameraDirection cameraDirection);
    public abstract bool SelectVideoMode(CameraDeviceMode mode);
    public abstract bool SetFlashTorchMode(bool on);
    public abstract bool SetFocusMode(FocusMode mode);
    public abstract bool SetFrameFormat(Image.PIXEL_FORMAT format, bool enabled);
    public abstract bool Start();
    public abstract bool Stop();

    public static CameraDevice Instance
    {
        get
        {
            if (mInstance == null)
            {
                lock (typeof(CameraDevice))
                {
                    if (mInstance == null)
                    {
                        mInstance = new CameraDeviceImpl();
                    }
                }
            }
            return mInstance;
        }
    }

    public enum CameraDeviceMode
    {
        MODE_DEFAULT = -1,
        MODE_OPTIMIZE_QUALITY = -3,
        MODE_OPTIMIZE_SPEED = -2
    }

    public enum CameraDirection
    {
        CAMERA_DEFAULT,
        CAMERA_BACK,
        CAMERA_FRONT
    }

    public enum FocusMode
    {
        FOCUS_MODE_NORMAL,
        FOCUS_MODE_TRIGGERAUTO,
        FOCUS_MODE_CONTINUOUSAUTO,
        FOCUS_MODE_INFINITY,
        FOCUS_MODE_MACRO
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VideoModeData
    {
        public int width;
        public int height;
        public float frameRate;
    }
}

