using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebCamImpl
{
    private readonly Camera mARCamera;
    private readonly Camera mBackgroundCameraInstance;
    private readonly BGRenderingAbstractBehaviour mBgRenderingTexBehaviour;
    private readonly Queue<BufferedFrame> mBufferedFrames = new Queue<BufferedFrame>();
    private Texture2D mBufferReadTexture;
    private readonly bool mFlipHorizontally;
    private int mLastFrameIdx = -1;
    private int mLastScreenHeight;
    private int mLastScreenWidth;
    private readonly CameraClearFlags mOriginalCameraClearFlags;
    private readonly int mOriginalCameraCullMask;
    private Rect mReadPixelsRect = new Rect();
    private readonly int mRenderTextureLayer;
    private TextureRenderer mTextureRenderer;
    private CameraDevice.VideoModeData mVideoModeData = new CameraDevice.VideoModeData();
    private QCARRenderer.VideoTextureInfo mVideoTextureInfo = new QCARRenderer.VideoTextureInfo();
    private bool mWebcamPlaying;
    private readonly WebCamProfile.ProfileData mWebCamProfile = new WebCamProfile.ProfileData();
    private readonly WebCamTexAdaptor mWebCamTexture;

    public WebCamImpl(Camera arCamera, Camera backgroundCamera, int renderTextureLayer, string webcamDeviceName, bool flipHorizontally)
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            this.mRenderTextureLayer = renderTextureLayer;
            this.mARCamera = arCamera;
            this.mOriginalCameraClearFlags = this.mARCamera.clearFlags;
            this.mARCamera.clearFlags = CameraClearFlags.Depth;
            this.mBackgroundCameraInstance = backgroundCamera;
            this.mBgRenderingTexBehaviour = this.mBackgroundCameraInstance.GetComponentInChildren<BGRenderingAbstractBehaviour>();
            if (this.mBgRenderingTexBehaviour == null)
            {
                Debug.LogError("Instanciated Prefab does not contain VideoTextureBehaviour!");
            }
            else
            {
                this.mOriginalCameraCullMask = this.mARCamera.cullingMask;
                this.mARCamera.cullingMask &= ~(((int) 1) << this.mBgRenderingTexBehaviour.gameObject.layer);
                this.mARCamera.cullingMask &= ~(((int) 1) << this.mRenderTextureLayer);
                WebCamProfile profile = new WebCamProfile();
                if (QCARRuntimeUtilities.IsQCAREnabled() && (WebCamTexture.devices.Length > 0))
                {
                    bool flag = false;
                    foreach (WebCamDevice device in WebCamTexture.devices)
                    {
                        if (device.name.Equals(webcamDeviceName))
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        webcamDeviceName = WebCamTexture.devices[0].name;
                    }
                    this.mWebCamProfile = profile.GetProfile(webcamDeviceName);
                    this.mWebCamTexture = new WebCamTexAdaptorImpl(webcamDeviceName, this.mWebCamProfile.RequestedFPS, this.mWebCamProfile.RequestedTextureSize);
                }
                else
                {
                    this.mWebCamProfile = profile.Default;
                    this.mWebCamTexture = new NullWebCamTexAdaptor(this.mWebCamProfile.RequestedFPS, this.mWebCamProfile.RequestedTextureSize);
                }
                this.mBgRenderingTexBehaviour.SetFlipHorizontally(flipHorizontally);
                this.mFlipHorizontally = flipHorizontally;
            }
        }
    }

    internal Color32[] GetPixels32AndBufferFrame(int frameIndex)
    {
        RenderTexture texture = this.mTextureRenderer.Render();
        BufferedFrame item = new BufferedFrame {
            frame = texture,
            frameIndex = frameIndex
        };
        this.mBufferedFrames.Enqueue(item);
        RenderTexture.active = texture;
        this.mBufferReadTexture.ReadPixels(this.mReadPixelsRect, 0, 0, false);
        return this.mBufferReadTexture.GetPixels32();
    }

    internal CameraDevice.VideoModeData GetVideoMode()
    {
        return this.mVideoModeData;
    }

    internal QCARRenderer.VideoTextureInfo GetVideoTextureInfo()
    {
        return this.mVideoTextureInfo;
    }

    internal bool IsRendererDirty()
    {
        bool flag = this.IsTextureSizeAvailable && ((this.mLastScreenWidth != Screen.width) || (this.mLastScreenHeight != Screen.height));
        if (flag)
        {
            this.mLastScreenWidth = Screen.width;
            this.mLastScreenHeight = Screen.height;
        }
        return flag;
    }

    internal void OnDestroy()
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            this.mARCamera.clearFlags = this.mOriginalCameraClearFlags;
            this.mARCamera.cullingMask = this.mOriginalCameraCullMask;
            this.IsTextureSizeAvailable = false;
            if (this.mTextureRenderer != null)
            {
                this.mTextureRenderer.Destroy();
            }
        }
    }

    private void RenderFrame(RenderTexture frameToDraw)
    {
        if (QCARRenderer.Instance.DrawVideoBackground)
        {
            this.mBgRenderingTexBehaviour.SetTexture(frameToDraw);
        }
        else
        {
            Texture2D videoBackgroundForEmulator = ((QCARRendererImpl) QCARRenderer.Instance).VideoBackgroundForEmulator;
            if (videoBackgroundForEmulator != null)
            {
                if (((videoBackgroundForEmulator.width != frameToDraw.width) || (videoBackgroundForEmulator.height != frameToDraw.height)) || (videoBackgroundForEmulator.format != TextureFormat.ARGB32))
                {
                    videoBackgroundForEmulator.Resize(frameToDraw.width, frameToDraw.height, TextureFormat.ARGB32, false);
                }
                RenderTexture.active = frameToDraw;
                videoBackgroundForEmulator.ReadPixels(new Rect(0f, 0f, (float) frameToDraw.width, (float) frameToDraw.height), 0, 0);
                videoBackgroundForEmulator.Apply();
            }
        }
    }

    internal void ResetPlaying()
    {
        if (this.mWebcamPlaying)
        {
            this.StartCamera();
        }
        else
        {
            this.StopCamera();
        }
    }

    internal void SetFrameIndex(int frameIndex)
    {
        int mLastFrameIdx = this.mLastFrameIdx;
        if (mLastFrameIdx != frameIndex)
        {
            while (mLastFrameIdx != frameIndex)
            {
                if (this.mBufferedFrames.Count == 0)
                {
                    break;
                }
                BufferedFrame frame = this.mBufferedFrames.Peek();
                mLastFrameIdx = frame.frameIndex;
                if (mLastFrameIdx == frameIndex)
                {
                    this.RenderFrame(frame.frame);
                    break;
                }
                this.mBufferedFrames.Dequeue();
                RenderTexture.ReleaseTemporary(frame.frame);
            }
            this.mLastFrameIdx = frameIndex;
        }
    }

    internal void StartCamera()
    {
        this.mWebcamPlaying = true;
        if (!this.mWebCamTexture.IsPlaying)
        {
            this.mWebCamTexture.Play();
        }
    }

    internal void StopCamera()
    {
        this.mWebcamPlaying = false;
        this.mWebCamTexture.Stop();
    }

    internal void Update()
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            if (!this.IsTextureSizeAvailable && this.mWebCamTexture.DidUpdateThisFrame)
            {
                QCARRenderer.Vec2I resampledTextureSize = this.mWebCamProfile.ResampledTextureSize;
                CameraDevice.VideoModeData data = new CameraDevice.VideoModeData {
                    width = resampledTextureSize.x,
                    height = resampledTextureSize.y,
                    frameRate = this.mWebCamProfile.RequestedFPS
                };
                this.mVideoModeData = data;
                QCARRenderer.VideoTextureInfo info = new QCARRenderer.VideoTextureInfo {
                    imageSize = resampledTextureSize,
                    textureSize = resampledTextureSize
                };
                this.mVideoTextureInfo = info;
                this.mTextureRenderer = new TextureRenderer(this.mWebCamTexture.Texture, this.mRenderTextureLayer, resampledTextureSize);
                this.mBufferReadTexture = new Texture2D(resampledTextureSize.x, resampledTextureSize.y);
                this.mReadPixelsRect = new Rect(0f, 0f, (float) resampledTextureSize.x, (float) resampledTextureSize.y);
                this.IsTextureSizeAvailable = true;
            }
            this.mBgRenderingTexBehaviour.CheckAndSetActive(QCARRenderer.Instance.DrawVideoBackground);
        }
    }

    public int ActualHeight
    {
        get
        {
            return this.mTextureRenderer.Height;
        }
    }

    public int ActualWidth
    {
        get
        {
            return this.mTextureRenderer.Width;
        }
    }

    public bool DidUpdateThisFrame
    {
        get
        {
            return (this.IsTextureSizeAvailable && this.mWebCamTexture.DidUpdateThisFrame);
        }
    }

    public bool FlipHorizontally
    {
        get
        {
            return this.mFlipHorizontally;
        }
    }

    public bool IsPlaying
    {
        get
        {
            return this.mWebCamTexture.IsPlaying;
        }
    }

    public bool IsTextureSizeAvailable { get; private set; }

    public QCARRenderer.Vec2I ResampledTextureSize
    {
        get
        {
            return this.mWebCamProfile.ResampledTextureSize;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BufferedFrame
    {
        public int frameIndex;
        public RenderTexture frame;
    }
}

