using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

internal class QCARRendererImpl : QCARRenderer
{
    private QCARRenderer.VideoBGCfgData mVideoBGConfig;
    private bool mVideoBGConfigSet;

    public override void ClearVideoBackgroundConfig()
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            this.mVideoBGConfigSet = false;
        }
    }

    public override QCARRenderer.VideoBGCfgData GetVideoBackgroundConfig()
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            return this.mVideoBGConfig;
        }
        IntPtr bgCfg = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(QCARRenderer.VideoBGCfgData)));
        QCARWrapper.Instance.RendererGetVideoBackgroundCfg(bgCfg);
        QCARRenderer.VideoBGCfgData data = (QCARRenderer.VideoBGCfgData) Marshal.PtrToStructure(bgCfg, typeof(QCARRenderer.VideoBGCfgData));
        Marshal.FreeHGlobal(bgCfg);
        return data;
    }

    public override QCARRenderer.VideoTextureInfo GetVideoTextureInfo()
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            CameraDeviceImpl instance = (CameraDeviceImpl) CameraDevice.Instance;
            return instance.WebCam.GetVideoTextureInfo();
        }
        IntPtr texInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(QCARRenderer.VideoTextureInfo)));
        QCARWrapper.Instance.RendererGetVideoBackgroundTextureInfo(texInfo);
        QCARRenderer.VideoTextureInfo info = (QCARRenderer.VideoTextureInfo) Marshal.PtrToStructure(texInfo, typeof(QCARRenderer.VideoTextureInfo));
        Marshal.FreeHGlobal(texInfo);
        return info;
    }

    public override bool IsVideoBackgroundInfoAvailable()
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            return this.mVideoBGConfigSet;
        }
        return (QCARWrapper.Instance.RendererIsVideoBackgroundTextureInfoAvailable() != 0);
    }

    public override void Pause(bool pause)
    {
        ((QCARManagerImpl) QCARManager.Instance).Pause(pause);
    }

    public override void SetVideoBackgroundConfig(QCARRenderer.VideoBGCfgData config)
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            this.mVideoBGConfig = config;
            this.mVideoBGConfigSet = true;
        }
        else
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(QCARRenderer.VideoBGCfgData)));
            Marshal.StructureToPtr(config, ptr, true);
            QCARWrapper.Instance.RendererSetVideoBackgroundCfg(ptr);
            Marshal.FreeHGlobal(ptr);
        }
    }

    public override bool SetVideoBackgroundTexture(Texture2D texture)
    {
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            this.VideoBackgroundForEmulator = texture;
            return true;
        }
        if (texture != null)
        {
            return (QCARWrapper.Instance.RendererSetVideoBackgroundTextureID(texture.GetNativeTextureID()) != 0);
        }
        return true;
    }

    public override bool DrawVideoBackground
    {
        get
        {
            return QCARManager.Instance.DrawVideoBackground;
        }
        set
        {
            QCARManager.Instance.DrawVideoBackground = value;
        }
    }

    public Texture2D VideoBackgroundForEmulator { get; private set; }
}

