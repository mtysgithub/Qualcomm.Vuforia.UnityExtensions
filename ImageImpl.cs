using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ImageImpl : Image
{
    private int mBufferHeight = 0;
    private int mBufferWidth = 0;
    private byte[] mData = null;
    private bool mDataSet = false;
    private int mHeight = 0;
    private Image.PIXEL_FORMAT mPixelFormat = Image.PIXEL_FORMAT.UNKNOWN_FORMAT;
    private int mStride = 0;
    private IntPtr mUnmanagedData = IntPtr.Zero;
    private int mWidth = 0;

    private TextureFormat ConvertPixelFormat(Image.PIXEL_FORMAT input)
    {
        Image.PIXEL_FORMAT mPixelFormat = this.mPixelFormat;
        switch (mPixelFormat)
        {
            case Image.PIXEL_FORMAT.RGB565:
                return TextureFormat.RGB565;

            case Image.PIXEL_FORMAT.RGB888:
                return TextureFormat.RGB24;
        }
        if (mPixelFormat != Image.PIXEL_FORMAT.RGBA8888)
        {
            return TextureFormat.Alpha8;
        }
        return TextureFormat.RGBA32;
    }

    internal void CopyPixelsFromUnmanagedBuffer()
    {
        if ((this.mData == null) || (this.mUnmanagedData == IntPtr.Zero))
        {
            Debug.LogError("Image: Cannot copy image image data.");
        }
        else
        {
            int num;
            switch (this.mPixelFormat)
            {
                case Image.PIXEL_FORMAT.RGB565:
                    num = (this.mBufferWidth * this.mBufferHeight) * 2;
                    break;

                case Image.PIXEL_FORMAT.RGB888:
                    num = (this.mBufferWidth * this.mBufferHeight) * 3;
                    break;

                case Image.PIXEL_FORMAT.RGBA8888:
                    num = (this.mBufferWidth * this.mBufferHeight) * 4;
                    break;

                default:
                    num = this.mBufferWidth * this.mBufferHeight;
                    break;
            }
            Marshal.Copy(this.mUnmanagedData, this.mData, 0, num);
            this.mDataSet = true;
        }
    }

    public override void CopyToTexture(Texture2D texture2D)
    {
        TextureFormat format = this.ConvertPixelFormat(this.mPixelFormat);
        if (((texture2D.width != this.mWidth) || (texture2D.height != this.mHeight)) || (format != texture2D.format))
        {
            texture2D.Resize(this.mWidth, this.mHeight, format, false);
        }
        int num = 1;
        Image.PIXEL_FORMAT mPixelFormat = this.mPixelFormat;
        switch (mPixelFormat)
        {
            case Image.PIXEL_FORMAT.RGB565:
            case Image.PIXEL_FORMAT.RGB888:
                num = 3;
                break;

            default:
                if (mPixelFormat == Image.PIXEL_FORMAT.RGBA8888)
                {
                    num = 4;
                }
                break;
        }
        Color[] pixels = texture2D.GetPixels();
        int num2 = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            for (int j = 0; j < num; j++)
            {
                pixels[i][j] = ((float) this.mData[num2++]) / 255f;
            }
            for (int k = num; k < 4; k++)
            {
                pixels[i][k] = pixels[i][k - 1];
            }
        }
        texture2D.SetPixels(pixels);
    }

    ~ImageImpl()
    {
        Marshal.FreeHGlobal(this.mUnmanagedData);
        this.mUnmanagedData = IntPtr.Zero;
    }

    public override bool IsValid()
    {
        return (((((this.mWidth > 0) && (this.mHeight > 0)) && ((this.mStride > 0) && (this.mBufferWidth > 0))) && ((this.mBufferHeight > 0) && (this.mData != null))) && this.mDataSet);
    }

    public override int BufferHeight
    {
        get
        {
            return this.mBufferHeight;
        }
        set
        {
            this.mBufferHeight = value;
        }
    }

    public override int BufferWidth
    {
        get
        {
            return this.mBufferWidth;
        }
        set
        {
            this.mBufferWidth = value;
        }
    }

    public override int Height
    {
        get
        {
            return this.mHeight;
        }
        set
        {
            this.mHeight = value;
        }
    }

    public override Image.PIXEL_FORMAT PixelFormat
    {
        get
        {
            return this.mPixelFormat;
        }
        set
        {
            this.mPixelFormat = value;
        }
    }

    public override byte[] Pixels
    {
        get
        {
            return this.mData;
        }
        set
        {
            this.mData = value;
        }
    }

    public override int Stride
    {
        get
        {
            return this.mStride;
        }
        set
        {
            this.mStride = value;
        }
    }

    public IntPtr UnmanagedData
    {
        get
        {
            return this.mUnmanagedData;
        }
        set
        {
            this.mUnmanagedData = value;
        }
    }

    public override int Width
    {
        get
        {
            return this.mWidth;
        }
        set
        {
            this.mWidth = value;
        }
    }
}

