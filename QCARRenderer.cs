using System;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class QCARRenderer
{
    private static QCARRenderer sInstance;

    protected QCARRenderer()
    {
    }

    public abstract void ClearVideoBackgroundConfig();
    public abstract VideoBGCfgData GetVideoBackgroundConfig();
    public abstract VideoTextureInfo GetVideoTextureInfo();
    public abstract bool IsVideoBackgroundInfoAvailable();
    public abstract void Pause(bool pause);
    public abstract void SetVideoBackgroundConfig(VideoBGCfgData config);
    public abstract bool SetVideoBackgroundTexture(Texture2D texture);

    public abstract bool DrawVideoBackground { get; set; }

    public static QCARRenderer Instance
    {
        get
        {
            if (sInstance == null)
            {
                lock (typeof(QCARRenderer))
                {
                    if (sInstance == null)
                    {
                        sInstance = new QCARRendererImpl();
                    }
                }
            }
            return sInstance;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct Vec2I
    {
        public int x;
        public int y;
        public Vec2I(int v1, int v2)
        {
            this.x = v1;
            this.y = v2;
        }
    }

    public enum VideoBackgroundReflection
    {
        DEFAULT,
        ON,
        OFF
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VideoBGCfgData
    {
        public int enabled;
        public int synchronous;
        public QCARRenderer.Vec2I position;
        public QCARRenderer.Vec2I size;
        [MarshalAs(UnmanagedType.SysInt)]
        public QCARRenderer.VideoBackgroundReflection reflection;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VideoTextureInfo
    {
        public QCARRenderer.Vec2I textureSize;
        public QCARRenderer.Vec2I imageSize;
    }
}

