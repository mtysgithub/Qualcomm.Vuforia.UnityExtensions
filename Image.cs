using System;
using UnityEngine;

public abstract class Image
{
    protected Image()
    {
    }

    public abstract void CopyToTexture(Texture2D texture2D);
    public abstract bool IsValid();

    public abstract int BufferHeight { get; set; }

    public abstract int BufferWidth { get; set; }

    public abstract int Height { get; set; }

    public abstract PIXEL_FORMAT PixelFormat { get; set; }

    public abstract byte[] Pixels { get; set; }

    public abstract int Stride { get; set; }

    public abstract int Width { get; set; }

    public enum PIXEL_FORMAT
    {
        GRAYSCALE = 4,
        RGB565 = 1,
        RGB888 = 2,
        RGBA8888 = 0x10,
        UNKNOWN_FORMAT = 0,
        YUV = 8
    }
}

