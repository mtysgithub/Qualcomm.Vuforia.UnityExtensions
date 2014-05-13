using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

internal class TargetFinderImpl : TargetFinder
{
    private Dictionary<int, ImageTarget> mImageTargets;
    private List<TargetFinder.TargetSearchResult> mNewResults;
    private TargetFinderState mTargetFinderState = new TargetFinderState();
    private IntPtr mTargetFinderStatePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TargetFinderState)));

    public TargetFinderImpl()
    {
        Marshal.StructureToPtr(this.mTargetFinderState, this.mTargetFinderStatePtr, false);
        this.mImageTargets = new Dictionary<int, ImageTarget>();
    }

    public override void ClearTrackables([Optional, DefaultParameterValue(true)] bool destroyGameObjects)
    {
        QCARWrapper.Instance.TargetFinderClearTrackables();
        StateManager stateManager = TrackerManager.Instance.GetStateManager();
        foreach (ImageTarget target in this.mImageTargets.Values)
        {
            stateManager.DestroyTrackableBehavioursForTrackable(target, destroyGameObjects);
        }
        this.mImageTargets.Clear();
    }

    public override bool Deinit()
    {
        return (QCARWrapper.Instance.TargetFinderDeinit() == 1);
    }

    public override ImageTargetAbstractBehaviour EnableTracking(TargetFinder.TargetSearchResult result, string gameObjectName)
    {
        GameObject gameObject = new GameObject(gameObjectName);
        return this.EnableTracking(result, gameObject);
    }

    public override ImageTargetAbstractBehaviour EnableTracking(TargetFinder.TargetSearchResult result, GameObject gameObject)
    {
        IntPtr trackableData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImageTargetData)));
        int trackableIdArrayLength = QCARWrapper.Instance.TargetFinderEnableTracking(result.TargetSearchResultPtr, trackableData);
        ImageTargetData data = (ImageTargetData) Marshal.PtrToStructure(trackableData, typeof(ImageTargetData));
        Marshal.FreeHGlobal(trackableData);
        StateManagerImpl stateManager = (StateManagerImpl) TrackerManager.Instance.GetStateManager();
        ImageTargetAbstractBehaviour behaviour = null;
        if (data.id == -1)
        {
            Debug.LogError("TargetSearchResult " + result.TargetName + " could not be enabled for tracking.");
        }
        else
        {
            ImageTarget trackable = new CloudRecoImageTargetImpl(result.TargetName, data.id, data.size);
            this.mImageTargets[data.id] = trackable;
            behaviour = stateManager.FindOrCreateImageTargetBehaviourForTrackable(trackable, gameObject);
        }
        IntPtr trackableIdArray = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(int)) * trackableIdArrayLength));
        QCARWrapper.Instance.TargetFinderGetImageTargets(trackableIdArray, trackableIdArrayLength);
        List<int> list = new List<int>();
        for (int i = 0; i < trackableIdArrayLength; i++)
        {
            IntPtr ptr = new IntPtr(trackableIdArray.ToInt32() + (i * Marshal.SizeOf(typeof(int))));
            int item = Marshal.ReadInt32(ptr);
            list.Add(item);
        }
        Marshal.FreeHGlobal(trackableIdArray);
        foreach (ImageTarget target2 in this.mImageTargets.Values.ToArray<ImageTarget>())
        {
            if (!list.Contains(target2.ID))
            {
                stateManager.DestroyTrackableBehavioursForTrackable(target2, true);
                this.mImageTargets.Remove(target2.ID);
            }
        }
        return behaviour;
    }

    ~TargetFinderImpl()
    {
        Marshal.FreeHGlobal(this.mTargetFinderStatePtr);
        this.mTargetFinderStatePtr = IntPtr.Zero;
    }

    public override IEnumerable<ImageTarget> GetImageTargets()
    {
        return this.mImageTargets.Values;
    }

    public override TargetFinder.InitState GetInitState()
    {
        return (TargetFinder.InitState) QCARWrapper.Instance.TargetFinderGetInitState();
    }

    public override IEnumerable<TargetFinder.TargetSearchResult> GetResults()
    {
        return this.mNewResults;
    }

    public override bool IsRequesting()
    {
        return (this.mTargetFinderState.IsRequesting == 1);
    }

    public override void SetUIPointColor(Color color)
    {
        QCARWrapper.Instance.TargetFinderSetUIPointColor(color.r, color.g, color.b);
    }

    public override void SetUIScanlineColor(Color color)
    {
        QCARWrapper.Instance.TargetFinderSetUIScanlineColor(color.r, color.g, color.b);
    }

    public override bool StartInit(string userAuth, string secretAuth)
    {
        return (QCARWrapper.Instance.TargetFinderStartInit(userAuth, secretAuth) == 1);
    }

    public override bool StartRecognition()
    {
        return (QCARWrapper.Instance.TargetFinderStartRecognition() == 1);
    }

    public override bool Stop()
    {
        return (QCARWrapper.Instance.TargetFinderStop() == 1);
    }

    public override TargetFinder.UpdateState Update()
    {
        QCARWrapper.Instance.TargetFinderUpdate(this.mTargetFinderStatePtr);
        this.mTargetFinderState = (TargetFinderState) Marshal.PtrToStructure(this.mTargetFinderStatePtr, typeof(TargetFinderState));
        if (this.mTargetFinderState.ResultCount > 0)
        {
            IntPtr searchResultArray = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(InternalTargetSearchResult)) * this.mTargetFinderState.ResultCount));
            if (QCARWrapper.Instance.TargetFinderGetResults(searchResultArray, this.mTargetFinderState.ResultCount) != 1)
            {
                Debug.LogError("TargetFinder: Could not retrieve new results!");
                return TargetFinder.UpdateState.UPDATE_NO_MATCH;
            }
            this.mNewResults = new List<TargetFinder.TargetSearchResult>();
            for (int i = 0; i < this.mTargetFinderState.ResultCount; i++)
            {
                IntPtr ptr = new IntPtr(searchResultArray.ToInt32() + (i * Marshal.SizeOf(typeof(QCARManagerImpl.TrackableResultData))));
                InternalTargetSearchResult result = (InternalTargetSearchResult) Marshal.PtrToStructure(ptr, typeof(InternalTargetSearchResult));
                TargetFinder.TargetSearchResult item = new TargetFinder.TargetSearchResult {
                    TargetName = Marshal.PtrToStringAnsi(result.TargetNamePtr),
                    UniqueTargetId = Marshal.PtrToStringAnsi(result.UniqueTargetIdPtr),
                    TargetSize = result.TargetSize,
                    MetaData = Marshal.PtrToStringAnsi(result.MetaDataPtr),
                    TrackingRating = result.TrackingRating,
                    TargetSearchResultPtr = result.TargetSearchResultPtr
                };
                this.mNewResults.Add(item);
            }
            Marshal.FreeHGlobal(searchResultArray);
        }
        return this.mTargetFinderState.UpdateState;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    private struct InternalTargetSearchResult
    {
        public IntPtr TargetNamePtr;
        public IntPtr UniqueTargetIdPtr;
        public float TargetSize;
        public IntPtr MetaDataPtr;
        public IntPtr TargetSearchResultPtr;
        public byte TrackingRating;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    private struct TargetFinderState
    {
        public int IsRequesting;
        [MarshalAs(UnmanagedType.SysInt)]
        public TargetFinder.UpdateState UpdateState;
        public int ResultCount;
    }
}

