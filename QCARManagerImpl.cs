using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class QCARManagerImpl : QCARManager
{
    private Camera mARCamera;
    private AutoRotationState mAutoRotationState;
    private bool mDrawVideobackground = true;
    private FrameState mFrameState;
    private IntPtr mImageHeaderData = IntPtr.Zero;
    private bool mInitialized;
    private int mInjectedFrameIdx;
    private IntPtr mLastProcessedFrameStatePtr = IntPtr.Zero;
    private int mNumImageHeaders;
    private bool mPaused;
    private LinkedList<int> mTrackableFoundQueue = new LinkedList<int>();
    private TrackableResultData[] mTrackableResultDataArray;
    private WordData[] mWordDataArray;
    private WordResultData[] mWordResultDataArray;
    private TrackableBehaviour mWorldCenter;
    private QCARAbstractBehaviour.WorldCenterMode mWorldCenterMode;

    public override void Deinit()
    {
        if (this.mInitialized)
        {
            Marshal.FreeHGlobal(this.mImageHeaderData);
            QCARWrapper.Instance.DeinitFrameState(this.mLastProcessedFrameStatePtr);
            Marshal.FreeHGlobal(this.mLastProcessedFrameStatePtr);
            this.mInitialized = false;
            this.mPaused = false;
        }
    }

    internal void FinishRendering()
    {
        QCARWrapper.Instance.RendererEnd();
    }

    public override bool Init()
    {
        this.mTrackableResultDataArray = new TrackableResultData[0];
        this.mWordDataArray = new WordData[0];
        this.mWordResultDataArray = new WordResultData[0];
        this.mTrackableFoundQueue = new LinkedList<int>();
        this.mImageHeaderData = IntPtr.Zero;
        this.mNumImageHeaders = 0;
        this.mLastProcessedFrameStatePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FrameState)));
        QCARWrapper.Instance.InitFrameState(this.mLastProcessedFrameStatePtr);
        this.InitializeTrackableContainer(0);
        this.mInitialized = true;
        return true;
    }

    private void InitializeTrackableContainer(int numTrackableResults)
    {
        if (this.mTrackableResultDataArray.Length != numTrackableResults)
        {
            this.mTrackableResultDataArray = new TrackableResultData[numTrackableResults];
            Debug.Log("Num trackables detected: " + numTrackableResults);
        }
    }

    private void InjectCameraFrame()
    {
        CameraDeviceImpl instance = (CameraDeviceImpl) CameraDevice.Instance;
        GCHandle handle = GCHandle.Alloc(instance.WebCam.GetPixels32AndBufferFrame(this.mInjectedFrameIdx), GCHandleType.Pinned);
        IntPtr pixels = handle.AddrOfPinnedObject();
        int actualWidth = instance.WebCam.ActualWidth;
        int actualHeight = instance.WebCam.ActualHeight;
        QCARWrapper.Instance.QcarAddCameraFrame(pixels, actualWidth, actualHeight, 0x10, 4 * actualWidth, this.mInjectedFrameIdx, instance.WebCam.FlipHorizontally ? 1 : 0);
        this.mInjectedFrameIdx++;
        pixels = IntPtr.Zero;
        handle.Free();
    }

    internal void Pause(bool pause)
    {
        if (pause)
        {
            AutoRotationState state = new AutoRotationState {
                autorotateToLandscapeLeft = Screen.autorotateToLandscapeLeft,
                autorotateToLandscapeRight = Screen.autorotateToLandscapeRight,
                autorotateToPortrait = Screen.autorotateToPortrait,
                autorotateToPortraitUpsideDown = Screen.autorotateToPortraitUpsideDown,
                setOnPause = true
            };
            this.mAutoRotationState = state;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }
        else if (this.mAutoRotationState.setOnPause)
        {
            Screen.autorotateToLandscapeLeft = this.mAutoRotationState.autorotateToLandscapeLeft;
            Screen.autorotateToLandscapeRight = this.mAutoRotationState.autorotateToLandscapeRight;
            Screen.autorotateToPortrait = this.mAutoRotationState.autorotateToPortrait;
            Screen.autorotateToPortraitUpsideDown = this.mAutoRotationState.autorotateToPortraitUpsideDown;
        }
        this.mPaused = pause;
    }

    internal void PrepareRendering()
    {
        if (this.mDrawVideobackground)
        {
            this.RenderVideoBackgroundOrDrawIntoTextureInNative();
        }
    }

    private void RenderVideoBackgroundOrDrawIntoTextureInNative()
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            if (this.mDrawVideobackground)
            {
                GL.InvalidateState();
            }
            QCARWrapper.Instance.RendererRenderVideoBackground(this.mDrawVideobackground ? 0 : 1);
            GL.InvalidateState();
        }
    }

    internal bool Update(ScreenOrientation counterRotation, CameraDevice.CameraDeviceMode deviceMode, ref CameraDevice.VideoModeData videoMode)
    {
        if (this.mPaused)
        {
            QCARWrapper.Instance.PausedUpdateQCAR();
            return false;
        }
        if (!QCARRuntimeUtilities.IsQCAREnabled())
        {
            this.UpdateTrackablesEditor();
            return true;
        }
        this.UpdateImageContainer();
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            CameraDeviceImpl instance = (CameraDeviceImpl) CameraDevice.Instance;
            if (instance.WebCam.DidUpdateThisFrame)
            {
                this.InjectCameraFrame();
            }
        }
        if (QCARWrapper.Instance.UpdateQCAR(this.mImageHeaderData, this.mNumImageHeaders, this.mLastProcessedFrameStatePtr, (int) counterRotation, (int) deviceMode) == 0)
        {
            return false;
        }
        this.mFrameState = (FrameState) Marshal.PtrToStructure(this.mLastProcessedFrameStatePtr, typeof(FrameState));
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            videoMode = CameraDevice.Instance.GetVideoMode(deviceMode);
        }
        else
        {
            IntPtr videoModeData = this.mFrameState.videoModeData;
            videoMode = (CameraDevice.VideoModeData) Marshal.PtrToStructure(videoModeData, typeof(CameraDevice.VideoModeData));
        }
        this.InitializeTrackableContainer(this.mFrameState.numTrackableResults);
        this.UpdateCameraFrame();
        this.UpdateTrackers(this.mFrameState);
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            CameraDeviceImpl impl2 = (CameraDeviceImpl) CameraDevice.Instance;
            impl2.WebCam.SetFrameIndex(this.mFrameState.frameIndex);
        }
        if (!this.mDrawVideobackground)
        {
            this.RenderVideoBackgroundOrDrawIntoTextureInNative();
        }
        return true;
    }

    private void UpdateCameraFrame()
    {
        int num = 0;
        CameraDeviceImpl instance = (CameraDeviceImpl) CameraDevice.Instance;
        foreach (ImageImpl impl2 in instance.GetAllImages().Values)
        {
            IntPtr ptr = new IntPtr(this.mImageHeaderData.ToInt32() + (num * Marshal.SizeOf(typeof(ImageHeaderData))));
            ImageHeaderData data = (ImageHeaderData) Marshal.PtrToStructure(ptr, typeof(ImageHeaderData));
            impl2.Width = data.width;
            impl2.Height = data.height;
            impl2.Stride = data.stride;
            impl2.BufferWidth = data.bufferWidth;
            impl2.BufferHeight = data.bufferHeight;
            impl2.PixelFormat = (Image.PIXEL_FORMAT) data.format;
            if (data.reallocate == 1)
            {
                impl2.Pixels = new byte[QCARWrapper.Instance.QcarGetBufferSize(impl2.BufferWidth, impl2.BufferHeight, (int) impl2.PixelFormat)];
                Marshal.FreeHGlobal(impl2.UnmanagedData);
                impl2.UnmanagedData = Marshal.AllocHGlobal(QCARWrapper.Instance.QcarGetBufferSize(impl2.BufferWidth, impl2.BufferHeight, (int) impl2.PixelFormat));
            }
            else if (data.updated == 1)
            {
                impl2.CopyPixelsFromUnmanagedBuffer();
            }
            num++;
        }
    }

    private void UpdateImageContainer()
    {
        CameraDeviceImpl instance = (CameraDeviceImpl) CameraDevice.Instance;
        if ((this.mNumImageHeaders != instance.GetAllImages().Count) || ((instance.GetAllImages().Count > 0) && (this.mImageHeaderData == IntPtr.Zero)))
        {
            this.mNumImageHeaders = instance.GetAllImages().Count;
            Marshal.FreeHGlobal(this.mImageHeaderData);
            this.mImageHeaderData = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(ImageHeaderData)) * this.mNumImageHeaders));
        }
        int num = 0;
        foreach (ImageImpl impl2 in instance.GetAllImages().Values)
        {
            IntPtr ptr = new IntPtr(this.mImageHeaderData.ToInt32() + (num * Marshal.SizeOf(typeof(ImageHeaderData))));
            ImageHeaderData structure = new ImageHeaderData {
                width = impl2.Width,
                height = impl2.Height,
                stride = impl2.Stride,
                bufferWidth = impl2.BufferWidth,
                bufferHeight = impl2.BufferHeight,
                format = (int) impl2.PixelFormat,
                reallocate = 0,
                updated = 0,
                data = impl2.UnmanagedData
            };
            Marshal.StructureToPtr(structure, ptr, false);
            num++;
        }
    }

    private void UpdateTrackablesEditor()
    {
        TrackableBehaviour[] behaviourArray = (TrackableBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(TrackableBehaviour));
        foreach (TrackableBehaviour behaviour in behaviourArray)
        {
            if (behaviour.enabled)
            {
                if (behaviour is WordAbstractBehaviour)
                {
                    IEditorWordBehaviour behaviour2 = (IEditorWordBehaviour) behaviour;
                    behaviour2.SetNameForTrackable(behaviour2.IsSpecificWordMode ? behaviour2.SpecificWord : "AnyWord");
                    behaviour2.InitializeWord(new WordImpl(0, behaviour2.TrackableName, new Vector2(500f, 100f)));
                }
                behaviour.OnTrackerUpdate(TrackableBehaviour.Status.TRACKED);
            }
        }
    }

    private void UpdateTrackers(FrameState frameState)
    {
        StateManagerImpl impl;
        for (int i = 0; i < frameState.numTrackableResults; i++)
        {
            IntPtr ptr = new IntPtr(frameState.trackableDataArray.ToInt32() + (i * Marshal.SizeOf(typeof(TrackableResultData))));
            this.mTrackableResultDataArray[i] = (TrackableResultData) Marshal.PtrToStructure(ptr, typeof(TrackableResultData));
        }
        foreach (TrackableResultData data2 in this.mTrackableResultDataArray)
        {
            if (((data2.status == TrackableBehaviour.Status.DETECTED) || (data2.status == TrackableBehaviour.Status.TRACKED)) || (data2.status == TrackableBehaviour.Status.EXTENDED_TRACKED))
            {
                if (!this.mTrackableFoundQueue.Contains(data2.id))
                {
                    this.mTrackableFoundQueue.AddLast(data2.id);
                }
            }
            else if (this.mTrackableFoundQueue.Contains(data2.id))
            {
                this.mTrackableFoundQueue.Remove(data2.id);
            }
        }
        List<int> list = new List<int>(this.mTrackableFoundQueue);
        using (List<int>.Enumerator enumerator = list.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Predicate<TrackableResultData> match = null;
                int id = enumerator.Current;
                if (match == null)
                {
                    match = tr => tr.id == id;
                }
                if (Array.Exists<TrackableResultData>(this.mTrackableResultDataArray, match))
                {
                    goto Label_017B;
                }
                this.mTrackableFoundQueue.Remove(id);
            }
        }
    Label_017B:
        impl = (StateManagerImpl) TrackerManager.Instance.GetStateManager();
        int originTrackableID = -1;
        if (((this.mWorldCenterMode == QCARAbstractBehaviour.WorldCenterMode.SPECIFIC_TARGET) && (this.mWorldCenter != null)) && (this.mWorldCenter.Trackable != null))
        {
            originTrackableID = this.mWorldCenter.Trackable.ID;
        }
        else if (this.mWorldCenterMode == QCARAbstractBehaviour.WorldCenterMode.FIRST_TARGET)
        {
            impl.RemoveDisabledTrackablesFromQueue(ref this.mTrackableFoundQueue);
            if (this.mTrackableFoundQueue.Count > 0)
            {
                originTrackableID = this.mTrackableFoundQueue.First.Value;
            }
        }
        this.UpdateWordTrackables(frameState);
        impl.UpdateCameraPose(this.mARCamera, this.mTrackableResultDataArray, originTrackableID);
        impl.UpdateTrackablePoses(this.mARCamera, this.mTrackableResultDataArray, originTrackableID, frameState.frameIndex);
        impl.UpdateWords(this.mARCamera, this.mWordDataArray, this.mWordResultDataArray);
        impl.UpdateVirtualButtons(frameState.numVirtualButtonResults, frameState.vbDataArray);
    }

    private void UpdateWordTrackables(FrameState frameState)
    {
        this.mWordDataArray = new WordData[frameState.numNewWords];
        for (int i = 0; i < frameState.numNewWords; i++)
        {
            IntPtr ptr = new IntPtr(frameState.newWordDataArray.ToInt32() + (i * Marshal.SizeOf(typeof(WordData))));
            this.mWordDataArray[i] = (WordData) Marshal.PtrToStructure(ptr, typeof(WordData));
        }
        this.mWordResultDataArray = new WordResultData[frameState.numWordResults];
        for (int j = 0; j < frameState.numWordResults; j++)
        {
            IntPtr ptr2 = new IntPtr(frameState.wordResultArray.ToInt32() + (j * Marshal.SizeOf(typeof(WordResultData))));
            this.mWordResultDataArray[j] = (WordResultData) Marshal.PtrToStructure(ptr2, typeof(WordResultData));
        }
    }

    public override Camera ARCamera
    {
        get
        {
            return this.mARCamera;
        }
        set
        {
            this.mARCamera = value;
        }
    }

    public override bool DrawVideoBackground
    {
        get
        {
            return this.mDrawVideobackground;
        }
        set
        {
            this.mDrawVideobackground = value;
        }
    }

    public override bool Initialized
    {
        get
        {
            return this.mInitialized;
        }
    }

    public int QCARFrameIndex
    {
        get
        {
            return this.mFrameState.frameIndex;
        }
    }

    public override TrackableBehaviour WorldCenter
    {
        get
        {
            return this.mWorldCenter;
        }
        set
        {
            this.mWorldCenter = value;
        }
    }

    public override QCARAbstractBehaviour.WorldCenterMode WorldCenterMode
    {
        get
        {
            return this.mWorldCenterMode;
        }
        set
        {
            this.mWorldCenterMode = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AutoRotationState
    {
        public bool setOnPause;
        public bool autorotateToPortrait;
        public bool autorotateToPortraitUpsideDown;
        public bool autorotateToLandscapeLeft;
        public bool autorotateToLandscapeRight;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    private struct FrameState
    {
        public int numTrackableResults;
        public int numVirtualButtonResults;
        public int frameIndex;
        public IntPtr trackableDataArray;
        public IntPtr vbDataArray;
        public int numWordResults;
        public IntPtr wordResultArray;
        public int numNewWords;
        public IntPtr newWordDataArray;
        public IntPtr videoModeData;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct ImageHeaderData
    {
        public int width;
        public int height;
        public int stride;
        public int bufferWidth;
        public int bufferHeight;
        public int format;
        public int reallocate;
        public int updated;
        public IntPtr data;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct Obb2D
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
        public Vector2 center;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
        public Vector2 halfExtents;
        public float rotation;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct PoseData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=3)]
        public Vector3 position;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
        public Quaternion orientation;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct TrackableResultData
    {
        public QCARManagerImpl.PoseData pose;
        public TrackableBehaviour.Status status;
        public int id;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VirtualButtonData
    {
        public int id;
        public int isPressed;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct WordData
    {
        public int id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
        public Vector2 size;
        public IntPtr stringValue;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct WordResultData
    {
        public QCARManagerImpl.PoseData pose;
        public TrackableBehaviour.Status status;
        public int id;
        public QCARManagerImpl.Obb2D orientedBoundingBox;
    }
}

