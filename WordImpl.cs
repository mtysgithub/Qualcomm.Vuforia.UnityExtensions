using System;
using System.Runtime.InteropServices;
using UnityEngine;

internal class WordImpl : TrackableImpl, Word, Trackable
{
    private RectangleData[] mLetterBoundingBoxes;
    private QCARManagerImpl.ImageHeaderData mLetterImageHeader;
    private Image mLetterMask;
    private Vector2 mSize;
    private string mText;

    public WordImpl(int id, string text, Vector2 size) : base(text, id)
    {
        this.mText = text;
        this.mSize = size;
    }

    private static void AllocateImage(ImageImpl image)
    {
        image.Pixels = new byte[QCARWrapper.Instance.QcarGetBufferSize(image.BufferWidth, image.BufferHeight, (int) image.PixelFormat)];
        Marshal.FreeHGlobal(image.UnmanagedData);
        image.UnmanagedData = Marshal.AllocHGlobal(QCARWrapper.Instance.QcarGetBufferSize(image.BufferWidth, image.BufferHeight, (int) image.PixelFormat));
    }

    private void CreateLetterMask()
    {
        this.InitImageHeader();
        ImageImpl mLetterMask = (ImageImpl) this.mLetterMask;
        SetImageValues(this.mLetterImageHeader, mLetterMask);
        AllocateImage(mLetterMask);
        this.mLetterImageHeader.data = mLetterMask.UnmanagedData;
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(QCARManagerImpl.ImageHeaderData)));
        Marshal.StructureToPtr(this.mLetterImageHeader, ptr, false);
        QCARWrapper.Instance.WordGetLetterMask(base.ID, ptr);
        this.mLetterImageHeader = (QCARManagerImpl.ImageHeaderData) Marshal.PtrToStructure(ptr, typeof(QCARManagerImpl.ImageHeaderData));
        if (this.mLetterImageHeader.reallocate == 1)
        {
            Debug.LogWarning("image wasn't allocated correctly");
        }
        else
        {
            mLetterMask.CopyPixelsFromUnmanagedBuffer();
            this.mLetterMask = mLetterMask;
            Marshal.FreeHGlobal(ptr);
        }
    }

    public RectangleData[] GetLetterBoundingBoxes()
    {
        if (!QCARRuntimeUtilities.IsQCAREnabled())
        {
            return new RectangleData[0];
        }
        if (this.mLetterBoundingBoxes == null)
        {
            int length = this.mText.Length;
            this.mLetterBoundingBoxes = new RectangleData[length];
            IntPtr letterBoundingBoxes = Marshal.AllocHGlobal((int) (length * Marshal.SizeOf(typeof(RectangleData))));
            QCARWrapper.Instance.WordGetLetterBoundingBoxes(base.ID, letterBoundingBoxes);
            IntPtr ptr = new IntPtr(letterBoundingBoxes.ToInt32());
            for (int i = 0; i < length; i++)
            {
                this.mLetterBoundingBoxes[i] = (RectangleData) Marshal.PtrToStructure(ptr, typeof(RectangleData));
                ptr = new IntPtr(ptr.ToInt32() + Marshal.SizeOf(typeof(RectangleData)));
            }
            Marshal.FreeHGlobal(letterBoundingBoxes);
        }
        return this.mLetterBoundingBoxes;
    }

    public Image GetLetterMask()
    {
        if (!QCARRuntimeUtilities.IsQCAREnabled())
        {
            return null;
        }
        if (this.mLetterMask == null)
        {
            this.CreateLetterMask();
        }
        return this.mLetterMask;
    }

    private void InitImageHeader()
    {
        this.mLetterImageHeader = new QCARManagerImpl.ImageHeaderData();
        this.mLetterImageHeader.width = this.mLetterImageHeader.bufferWidth = (int) (this.Size.x + 1f);
        this.mLetterImageHeader.height = this.mLetterImageHeader.bufferHeight = (int) (this.Size.y + 1f);
        this.mLetterImageHeader.format = 4;
        this.mLetterMask = new ImageImpl();
    }

    private static void SetImageValues(QCARManagerImpl.ImageHeaderData imageHeader, ImageImpl image)
    {
        image.Width = imageHeader.width;
        image.Height = imageHeader.height;
        image.Stride = imageHeader.stride;
        image.BufferWidth = imageHeader.bufferWidth;
        image.BufferHeight = imageHeader.bufferHeight;
        image.PixelFormat = (Image.PIXEL_FORMAT) imageHeader.format;
    }

    public Vector2 Size
    {
        get
        {
            return this.mSize;
        }
    }

    public string StringValue
    {
        get
        {
            return this.mText;
        }
    }
}

